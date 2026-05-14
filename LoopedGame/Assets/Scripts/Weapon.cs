using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    [Header("References")]
    public Transform playerCapsule;
    public Transform blockPos;

    [Header("Light Attack")]
    public float swingHalfArc = 60f;
    public float swingDuration = 0.25f;
    public AnimationCurve swingCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Heavy Attack")]
    public float heavyWindupDuration = 0.5f;
    public float heavySwingDuration = 0.12f;
    public float heavySwingHalfArc = 100f;
    public AnimationCurve heavySwingCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    public float heavyKnockbackForce = 12f;

    [Header("Damage")]
    public float damageAmount = 2f;
    [SerializeField] private float heavyDamageAmount = 4f;

    [Header("Block")]
    public float blockMeterMax = 3f;
    public float blockMeterRechargeRate = 1f;
    public float blockMeterDrainRate = 0.4f;
    public float blockCooldown = 0.5f;
    public float blockBreakStunDuration = 1f;
    public float blockBrokenMoveMultiplier = 0.15f;

    [SerializeField] private float blockMeterDrainPerDamage = 0.08f;

    [Header("Parry")]
    public Collider parryHitboxCollider;
    public float parryWindow = 0.25f;
    public float parryMissStunDuration = 0.25f;
    public GameObject parryProjectilePrefab;
    public float parryReflectSpeedMultiplier = 1.4f;
    public float parryMissBlockMeterCost = 1.5f;

    [Header("UI")]
    public Slider blockMeterSlider;
    public Slider attackReadySlider;

    private PlayerInput playerInput;
    private InputAction attackAction;
    private InputAction blockAction;

    private Collider weaponCollider;

    private Vector3 readyLocalPosition;
    private Quaternion readyLocalRotation;

    private bool isSwinging;
    private float swingTimer;
    private int swingDirection = 1;

    private bool isChargingHeavy;
    private bool isHeavySwinging;
    private float heavyWindupTimer;
    private float heavySwingTimer;
    private int heavySwingDirection = 1;
    private float holdTimer;

    private bool isBlocking;
    private bool isStunned;
    private float blockMeter;
    private float blockCooldownTimer;
    private Coroutine stunCoroutine;

    private bool isParrying;
    private float parryTimer;
    private bool parryLanded;

    private float temporaryNextLightSpeedMultiplier = 1f;
    private float nextHeavyWindupMultiplier = 1f;

    private float blockDrainCooldown = 0.3f;
    private float blockDrainCooldownTimer;

    public bool IsBlocking => isBlocking;
    public bool IsStunned => isStunned;
    public bool IsParrying => isParrying;
    public bool IsHeavyAttacking => isChargingHeavy || isHeavySwinging;
    public float BlockMeterNormalized => blockMeterMax <= 0f ? 0f : blockMeter / blockMeterMax;
    public float MoveSpeedMultiplier => isStunned ? blockBrokenMoveMultiplier : 1f;

    private void Awake()
    {
        playerInput = GetComponentInParent<PlayerInput>();

        if (playerInput == null)
        {
            Debug.LogError("[Weapon] No PlayerInput component found in parent.");
            enabled = false;
            return;
        }

        attackAction = playerInput.actions.FindAction("Attack", true);
        blockAction = playerInput.actions.FindAction("Block", true);

        if (attackAction == null)
        {
            Debug.LogError("[Weapon] Input Action 'Attack' not found.");
            enabled = false;
            return;
        }

        if (blockAction == null)
        {
            Debug.LogError("[Weapon] Input Action 'Block' not found.");
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        if (playerCapsule == null)
        {
            Debug.LogError("[Weapon] playerCapsule is not assigned.");
            enabled = false;
            return;
        }

        weaponCollider = GetComponent<Collider>();

        if (weaponCollider == null)
        {
            Debug.LogWarning("[Weapon] No Collider found on Weapon. Hitbox will not work.");
        }
        else
        {
            weaponCollider.isTrigger = true;
            weaponCollider.enabled = false;
        }

        if (parryHitboxCollider != null)
        {
            parryHitboxCollider.isTrigger = true;
            parryHitboxCollider.enabled = false;

            ParryHitbox relay = parryHitboxCollider.GetComponent<ParryHitbox>();

            if (relay == null)
            {
                Debug.LogWarning("[Weapon] parryHitboxCollider has no ParryHitbox component.");
            }
            else
            {
                relay.weapon = this;
            }
        }
        else
        {
            Debug.LogWarning("[Weapon] parryHitboxCollider is not assigned. Parry reflect will not function.");
        }

        readyLocalPosition = transform.localPosition;
        readyLocalRotation = transform.localRotation;

        blockMeter = blockMeterMax;
        heavyDamageAmount = Mathf.Max(heavyDamageAmount, damageAmount * 2f);
    }

    private void Update()
    {
        if (blockCooldownTimer > 0f)
        {
            blockCooldownTimer -= Time.deltaTime;
        }

        if (blockDrainCooldownTimer > 0f)
        {
            blockDrainCooldownTimer -= Time.deltaTime;
        }

        UpdateParryInput();

        if (!isParrying)
        {
            UpdateBlock();
            UpdateAttackInput();
        }

        if (isSwinging)
        {
            UpdateSwing();
        }

        if (isChargingHeavy)
        {
            UpdateHeavyWindup();
        }

        if (isHeavySwinging)
        {
            UpdateHeavySwing();
        }

        if (isParrying)
        {
            UpdateParry();
        }

        if (blockMeterSlider != null)
        {
            blockMeterSlider.value = BlockMeterNormalized;
        }

        if (attackReadySlider != null)
        {
            attackReadySlider.value = AttackSliderNormalized;
        }
    }

    private void UpdateAttackInput()
    {
        bool busy =
            isSwinging ||
            isChargingHeavy ||
            isHeavySwinging ||
            isBlocking ||
            isStunned ||
            isParrying;

        if (attackAction.WasPressedThisFrame() && !busy)
        {
            holdTimer = 0f;
        }

        if (attackAction.IsPressed() && !busy)
        {
            holdTimer += Time.deltaTime;
        }

        if (attackAction.WasReleasedThisFrame() && !busy && holdTimer < heavyWindupDuration)
        {
            StartSwing();
        }

        if (attackAction.IsPressed() && !busy && holdTimer >= heavyWindupDuration)
        {
            StartHeavyWindup();
        }
    }

    private void StartSwing()
    {
        isSwinging = true;
        swingTimer = 0f;
        swingDirection *= -1;

        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
        }

        ApplySwingAngle(-swingHalfArc * swingDirection);
    }

    private void UpdateSwing()
    {
        float effectiveDuration = swingDuration;

        if (temporaryNextLightSpeedMultiplier != 1f)
        {
            effectiveDuration *= Mathf.Clamp(temporaryNextLightSpeedMultiplier, 0.1f, 1f);
        }

        swingTimer += Time.deltaTime;

        float t = Mathf.Clamp01(swingTimer / effectiveDuration);
        float curved = swingCurve.Evaluate(t);
        float angle = Mathf.Lerp(-swingHalfArc, swingHalfArc, curved) * swingDirection;

        ApplySwingAngle(angle);
        Physics.SyncTransforms();

        if (t >= 1f)
        {
            EndSwing();
        }
    }

    private void EndSwing()
    {
        isSwinging = false;
        temporaryNextLightSpeedMultiplier = 1f;

        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }
    }

    private void StartHeavyWindup()
    {
        isChargingHeavy = true;
        heavyWindupTimer = 0f;
    }

    private void UpdateHeavyWindup()
    {
        heavyWindupTimer += Time.deltaTime;

        if (attackAction.WasReleasedThisFrame())
        {
            isChargingHeavy = false;
            nextHeavyWindupMultiplier = 1f;
            return;
        }

        float effectiveWindup = heavyWindupDuration * Mathf.Clamp(nextHeavyWindupMultiplier, 0.1f, 1f);

        if (heavyWindupTimer >= effectiveWindup)
        {
            isChargingHeavy = false;
            nextHeavyWindupMultiplier = 1f;
            StartHeavySwing();
        }
    }

    private void StartHeavySwing()
    {
        isHeavySwinging = true;
        heavySwingTimer = 0f;
        heavySwingDirection *= -1;

        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
        }

        ApplySwingAngle(-heavySwingHalfArc * heavySwingDirection);
    }

    private void UpdateHeavySwing()
    {
        heavySwingTimer += Time.deltaTime;

        float t = Mathf.Clamp01(heavySwingTimer / heavySwingDuration);
        float curved = heavySwingCurve.Evaluate(t);
        float angle = Mathf.Lerp(-heavySwingHalfArc, heavySwingHalfArc, curved) * heavySwingDirection;

        ApplySwingAngle(angle);
        Physics.SyncTransforms();

        if (t >= 1f)
        {
            EndHeavySwing();
        }
    }

    private void EndHeavySwing()
    {
        isHeavySwinging = false;

        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }
    }

    private void ApplySwingAngle(float angleDeg)
    {
        Quaternion rotation = Quaternion.AngleAxis(angleDeg, Vector3.up);
        transform.localPosition = rotation * readyLocalPosition;
        transform.localRotation = rotation * readyLocalRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy"))
        {
            return;
        }

        EnemyHP enemy = other.GetComponentInParent<EnemyHP>();

        if (enemy == null)
        {
            Debug.LogWarning("[Weapon] Object tagged Enemy has no EnemyHP.");
            return;
        }

        bool isLightAttack = isSwinging && !isHeavySwinging;
        bool isHeavyAttack = isHeavySwinging;

        float finalDamage = isHeavyAttack ? heavyDamageAmount : damageAmount;

        PlayerRareCardAbilityController rareCards =
            GetComponentInParent<PlayerRareCardAbilityController>();

        PlayerEpicCardAbilityController epicCards =
            GetComponentInParent<PlayerEpicCardAbilityController>();

        PlayerLegendaryCardAbilityController legendaryCards =
            GetComponentInParent<PlayerLegendaryCardAbilityController>();

        if (rareCards != null)
        {
            finalDamage = rareCards.ModifyOutgoingWeaponDamage(
                finalDamage,
                enemy,
                isLightAttack,
                isHeavyAttack
            );
        }

        if (epicCards != null)
        {
            finalDamage = epicCards.ModifyOutgoingWeaponDamage(
                finalDamage,
                enemy,
                isLightAttack,
                isHeavyAttack
            );
        }

        if (legendaryCards != null)
        {
            finalDamage = legendaryCards.ModifyOutgoingWeaponDamage(
                finalDamage,
                enemy,
                isLightAttack,
                isHeavyAttack
            );
        }

        enemy.TakeDamage(finalDamage, playerCapsule.gameObject);

        if (isHeavyAttack)
        {
            ApplyHeavyKnockback(other);
        }

        if (rareCards != null)
        {
            rareCards.OnWeaponHit(enemy, isLightAttack, isHeavyAttack);
        }

        if (epicCards != null)
        {
            epicCards.OnWeaponHit(enemy, finalDamage, isLightAttack, isHeavyAttack);
        }

        if (legendaryCards != null)
        {
            legendaryCards.OnWeaponHit(enemy, isLightAttack, isHeavyAttack);
        }
    }

    private void ApplyHeavyKnockback(Collider other)
    {
        if (playerCapsule == null)
        {
            return;
        }

        Vector3 knockDirection = other.transform.position - playerCapsule.position;
        knockDirection.y = 0f;

        if (knockDirection.sqrMagnitude <= 0.001f)
        {
            return;
        }

        knockDirection.Normalize();

        EnemyPatrol enemyPatrol = other.GetComponentInParent<EnemyPatrol>();

        if (enemyPatrol != null)
        {
            enemyPatrol.TakeKnockback(knockDirection, heavyKnockbackForce);
        }

        if (ScreenImpactEffect.Instance != null)
        {
            ScreenImpactEffect.Instance.TriggerImpact();
        }
    }

    private void UpdateBlock()
    {
        if (blockAction == null)
        {
            return;
        }

        bool wantsBlock = blockAction.IsPressed();

        bool canBlock =
            !isStunned &&
            !isSwinging &&
            !isChargingHeavy &&
            !isHeavySwinging &&
            !isParrying &&
            blockCooldownTimer <= 0f &&
            blockMeter > 0f;

        if (wantsBlock && canBlock)
        {
            if (!isBlocking)
            {
                StartBlock();
            }

            blockMeter -= blockMeterDrainRate * Time.deltaTime;

            if (blockMeter <= 0f)
            {
                blockMeter = 0f;
                ForceEndBlock(true);
            }
        }
        else if (isBlocking)
        {
            ForceEndBlock(false);
        }

        if (!isBlocking && !isStunned)
        {
            blockMeter = Mathf.Min(blockMeter + blockMeterRechargeRate * Time.deltaTime, blockMeterMax);
        }

        if (isBlocking && blockPos != null)
        {
            transform.localPosition = readyLocalPosition + readyLocalRotation * blockPos.localPosition;
            transform.localRotation = readyLocalRotation * blockPos.localRotation;
        }
    }

    private void StartBlock()
    {
        isBlocking = true;
        isSwinging = false;

        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }
    }

    private void ForceEndBlock(bool stun)
    {
        isBlocking = false;
        blockCooldownTimer = blockCooldown;

        transform.localPosition = readyLocalPosition;
        transform.localRotation = readyLocalRotation;

        if (stun)
        {
            if (stunCoroutine != null)
            {
                StopCoroutine(stunCoroutine);
            }

            stunCoroutine = StartCoroutine(StunRoutine(blockBreakStunDuration));
        }
    }

    private IEnumerator StunRoutine(float duration)
    {
        isStunned = true;

        yield return new WaitForSeconds(duration);

        isStunned = false;
        stunCoroutine = null;
    }

    public void OnBlockedHit(float incomingDamage)
    {
        float drain = incomingDamage * blockMeterDrainPerDamage;
        blockMeter = Mathf.Max(0f, blockMeter - drain);

        PlayerEpicCardAbilityController epicCards =
            GetComponentInParent<PlayerEpicCardAbilityController>();

        PlayerLegendaryCardAbilityController legendaryCards =
            GetComponentInParent<PlayerLegendaryCardAbilityController>();

        if (epicCards != null)
        {
            epicCards.OnBlockHit();
        }

        if (legendaryCards != null)
        {
            legendaryCards.OnBlockHit();
        }

        if (blockMeter <= 0f)
        {
            ForceEndBlock(true);
        }
    }

    public void DrainBlockMeter(float amount)
    {
        if (blockDrainCooldownTimer > 0f)
        {
            return;
        }

        blockMeter = Mathf.Max(blockMeter - amount, 0f);
        blockDrainCooldownTimer = blockDrainCooldown;

        if (blockMeter <= 0f && isBlocking)
        {
            ForceEndBlock(true);
        }
    }

    private void UpdateParryInput()
    {
        if (attackAction == null || blockAction == null)
        {
            return;
        }

        bool parryPressed =
            (attackAction.WasPressedThisFrame() && blockAction.IsPressed()) ||
            (blockAction.WasPressedThisFrame() && attackAction.IsPressed());

        bool busy =
            isSwinging ||
            isChargingHeavy ||
            isHeavySwinging ||
            isStunned ||
            isParrying;

        if (parryPressed && !busy)
        {
            StartParry();
        }
    }

    private void StartParry()
    {
        isParrying = true;
        parryTimer = 0f;
        parryLanded = false;

        isBlocking = false;
        isSwinging = false;
        isChargingHeavy = false;
        isHeavySwinging = false;

        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }

        if (parryHitboxCollider != null)
        {
            parryHitboxCollider.enabled = true;
        }

        if (blockPos != null)
        {
            transform.localPosition = readyLocalPosition + readyLocalRotation * blockPos.localPosition;
            transform.localRotation = readyLocalRotation * blockPos.localRotation;
        }
    }

    private void UpdateParry()
    {
        parryTimer += Time.deltaTime;

        if (parryTimer >= parryWindow)
        {
            EndParry(parryLanded);
        }
    }

    private void EndParry(bool success)
    {
        isParrying = false;

        if (parryHitboxCollider != null)
        {
            parryHitboxCollider.enabled = false;
        }

        if (success)
        {
            blockCooldownTimer = blockCooldown;
        }
        else
        {
            blockMeter = Mathf.Max(0f, blockMeter - parryMissBlockMeterCost);

            if (stunCoroutine != null)
            {
                StopCoroutine(stunCoroutine);
            }

            stunCoroutine = StartCoroutine(StunRoutine(parryMissStunDuration));
            blockCooldownTimer = blockCooldown;
        }

        transform.localPosition = readyLocalPosition;
        transform.localRotation = readyLocalRotation;
    }

    public void OnParryContact(Collider other)
    {
        if (!isParrying)
        {
            return;
        }

        if (!other.CompareTag("EnemyProjectile"))
        {
            return;
        }

        parryLanded = true;

        GameObject reflectedProjectile = ReflectProjectile(other.gameObject);

        PlayerEpicCardAbilityController epicCards =
            GetComponentInParent<PlayerEpicCardAbilityController>();

        PlayerLegendaryCardAbilityController legendaryCards =
            GetComponentInParent<PlayerLegendaryCardAbilityController>();

        if (epicCards != null)
        {
            epicCards.OnParrySuccess();

            if (reflectedProjectile != null)
            {
                epicCards.ModifyParriedProjectile(reflectedProjectile);
            }
        }

        if (legendaryCards != null)
        {
            legendaryCards.OnParrySuccess();
        }

        EndParry(true);
    }

    private GameObject ReflectProjectile(GameObject projectile)
    {
        if (projectile == null) return null;

        if (parryProjectilePrefab == null)
        {
            Debug.LogWarning("[Weapon] parryProjectilePrefab is not assigned.");
            return null;
        }

        // Use actual incoming velocity for direction — more accurate than position delta
        Rigidbody incomingRb = projectile.GetComponent<Rigidbody>();
        Vector3 reflectDirection;

        if (incomingRb != null && incomingRb.linearVelocity.sqrMagnitude > 0.001f)
        {
            reflectDirection = -incomingRb.linearVelocity.normalized;
            reflectDirection.y = 0f;
        }
        else
        {
            Vector3 toPlayer = playerCapsule.position - projectile.transform.position;
            toPlayer.y = 0f;
            reflectDirection = toPlayer.sqrMagnitude > 0.001f ? -toPlayer.normalized : Vector3.zero;
        }

        if (reflectDirection.sqrMagnitude <= 0.001f)
        {
            Vector3 f = playerCapsule.forward;
            f.y = 0f;
            reflectDirection = f.sqrMagnitude > 0.001f ? f.normalized : Vector3.forward;
        }
        else
        {
            reflectDirection.Normalize();
        }


        Quaternion spawnRotation = Quaternion.AngleAxis(180f, Vector3.up) * projectile.transform.rotation;

        Vector3 spawnPosition = projectile.transform.position;

        projectile.SetActive(false);
        Destroy(projectile, 0.1f);

        GameObject reflected = Instantiate(parryProjectilePrefab, spawnPosition, spawnRotation);
        reflected.tag = "Weapon";

        ParriedProjectile pp = reflected.GetComponent<ParriedProjectile>();
        if (pp != null)
        {
            pp.direction = reflectDirection;
            pp.speed = pp.speed * parryReflectSpeedMultiplier;
        }

        Rigidbody rb = reflected.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = reflectDirection * pp.speed;

        reflected.SendMessage("SetReflectedDirection", reflectDirection, SendMessageOptions.DontRequireReceiver);
        reflected.SendMessage("MultiplySpeed", parryReflectSpeedMultiplier, SendMessageOptions.DontRequireReceiver);

        return reflected;
    }

    public float AttackSliderNormalized
    {
        get
        {
            if (isStunned) return 0f;
            if (isParrying) return 0f;

            if (!isChargingHeavy && !isSwinging && !isHeavySwinging && attackAction.IsPressed())
            {
                return 1f - Mathf.Clamp01(holdTimer / heavyWindupDuration);
            }

            if (isChargingHeavy) return 0f;

            if (isSwinging)
            {
                return Mathf.Clamp01(swingTimer / swingDuration);
            }

            if (isHeavySwinging)
            {
                return Mathf.Clamp01(heavySwingTimer / heavySwingDuration);
            }

            return 1f;
        }
    }

    // ------------------------------------------------------------
    // Public methods used by stat cards
    // ------------------------------------------------------------

    public void AddLightDamage(float amount)
    {
        damageAmount += amount;
        heavyDamageAmount = Mathf.Max(heavyDamageAmount, damageAmount * 2f);
    }

    public void AddHeavyDamage(float amount)
    {
        heavyDamageAmount += amount;
    }

    public void ReduceLightSwingDuration(float amount)
    {
        swingDuration = Mathf.Max(0.09f, swingDuration - amount);
    }

    public void ReduceHeavyWindup(float amount)
    {
        heavyWindupDuration = Mathf.Max(0.10f, heavyWindupDuration - amount);
    }

    public void ReduceHeavySwingDuration(float amount)
    {
        heavySwingDuration = Mathf.Max(0.05f, heavySwingDuration - amount);
    }

    public void IncreaseAttackArc(float amount)
    {
        swingHalfArc = Mathf.Min(swingHalfArc + amount, 135f);
        heavySwingHalfArc = Mathf.Min(heavySwingHalfArc + amount, 180f);
    }

    public void IncreaseBlockMeterMax(float amount)
    {
        blockMeterMax += amount;
        blockMeter = Mathf.Min(blockMeter + amount, blockMeterMax);
    }

    public void IncreaseBlockRecharge(float amount)
    {
        blockMeterRechargeRate += amount;
    }

    public void ReduceBlockDrain(float passiveAmount, float impactAmount)
    {
        blockMeterDrainRate = Mathf.Max(0.05f, blockMeterDrainRate - passiveAmount);
        blockMeterDrainPerDamage = Mathf.Max(0.01f, blockMeterDrainPerDamage - impactAmount);
    }

    public void ReduceBlockCooldown(float amount)
    {
        blockCooldown = Mathf.Max(0.05f, blockCooldown - amount);
    }

    public void ReduceBlockBreakStun(float amount)
    {
        blockBreakStunDuration = Mathf.Max(0.10f, blockBreakStunDuration - amount);
    }

    public void IncreaseParryWindow(float amount)
    {
        parryWindow = Mathf.Min(parryWindow + amount, 0.55f);
    }

    public void ReduceParryMissStun(float amount)
    {
        parryMissStunDuration = Mathf.Max(0.25f, parryMissStunDuration - amount);
    }

    public void ReduceParryMissBlockMeterCost(float amount)
    {
        parryMissBlockMeterCost = Mathf.Max(0.25f, parryMissBlockMeterCost - amount);
    }

    public void IncreaseParryReflectSpeedMultiplier(float amount)
    {
        parryReflectSpeedMultiplier = Mathf.Min(parryReflectSpeedMultiplier + amount, 2.90f);
    }

    public void RestoreBlockMeter(float amount)
    {
        blockMeter = Mathf.Min(blockMeter + amount, blockMeterMax);
    }

    public void ClearBlockCooldown()
    {
        blockCooldownTimer = 0f;
    }

    public float GetCurrentBlockMeter()
    {
        return blockMeter;
    }

    public void ConsumeBlockMeter(float amount)
    {
        blockMeter = Mathf.Max(0f, blockMeter - amount);
    }

    public void ApplyTemporaryNextLightSpeedBoost(float multiplier)
    {
        temporaryNextLightSpeedMultiplier = Mathf.Clamp(multiplier, 0.1f, 1f);
    }

    public void ApplyNextHeavyWindupMultiplier(float multiplier)
    {
        nextHeavyWindupMultiplier = Mathf.Clamp(multiplier, 0.1f, 1f);
    }

    public void ResetRunWeaponState()
    {
        isSwinging = false;
        isChargingHeavy = false;
        isHeavySwinging = false;
        isBlocking = false;
        isStunned = false;
        isParrying = false;

        swingTimer = 0f;
        heavyWindupTimer = 0f;
        heavySwingTimer = 0f;
        parryTimer = 0f;

        blockCooldownTimer = 0f;
        blockDrainCooldownTimer = 0f;
        blockMeter = blockMeterMax;

        temporaryNextLightSpeedMultiplier = 1f;
        nextHeavyWindupMultiplier = 1f;

        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }

        if (parryHitboxCollider != null)
        {
            parryHitboxCollider.enabled = false;
        }

        transform.localPosition = readyLocalPosition;
        transform.localRotation = readyLocalRotation;

        Debug.Log("[Weapon] Run weapon state reset.");
    }

}

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public float heavyKnockbackForce = 12f;

    public Transform playerCapsule;
    // degrees from player forward to each side of the arc
    public float swingHalfArc = 60f;
    // how long full swing takes in seconds
    public float swingDuration = 0.25f;
    // curve controlling swing speed - flat = constant, ease-in/out = slow at edges
    public AnimationCurve swingCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    public Transform blockPos;

    private bool isSwinging;
    private float swingTimer;
    private int swingDirection = 1;
    private Vector3 readyLocalPosThing;
    private Quaternion readyLocalRotThing;
    private Collider wpnCollider;
    private bool isBlocking;
    private PlayerInput plrInput;
    private InputAction atkAction;
    private float damageAmount = 2f;

    public Slider blockMeterSlider;
    public Slider attackReadySlider;

    [Header("Block")]
    public float blockMeterMax = 3f;
    public float blockMeterRechargeRate = 1f;
    public float blockMeterDrainRate = 0.4f;
    // minimum time after releasing block before you can block again (prevents spam)
    public float blockCooldown = 0.5f;
    public float stunDuration = 1f;

    // public read-only state for TopDownController and PlayerHP
    public bool IsBlocking => isBlocking;
    public bool IsStunned => isStunned;
    public float BlockMeterNormalized => blockMeter / blockMeterMax;
    // locks movement, dash, and block during heavy windup and swing
    public bool IsHeavyAttacking => isChargingHeavy || isHeavySwinging;

    private bool isStunned;
    private float blockMeter;
    private float blockCooldownTimer;
    private Coroutine stunCoroutine;
    private InputAction blockAction;

    // how long the player must hold still before the heavy swing fires
    public float heavyWindupDuration = 0.5f;
    // faster and wider than a light swing
    public float heavySwingDuration = 0.12f;
    public float heavySwingHalfArc = 100f;
    public AnimationCurve heavySwingCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private bool isChargingHeavy;
    private bool isHeavySwinging;
    private float heavyWindupTimer;
    private float heavySwingTimer;
    private int heavySwingDirection = 1;
    private float heavyDamageAmount;
    private float holdTimer;

    public float blockBrokenMoveMultiplier = 0.15f;

    public float MoveSpeedMultiplier => isStunned ? blockBrokenMoveMultiplier : 1f;
    [SerializeField] private float blockMeterDrainPerDamage = 0.08f;

    // ─── Parry ────────────────────────────────────────────────────────────────
    [Header("Parry")]
    // Assign a child GameObject's Collider here. That child also needs a
    // ParryHitbox component (see ParryHitbox.cs) and should be tagged "Parry".
    public Collider parryHitboxCollider;

    // How long the parry window stays open.
    public float parryWindow = 0.25f;

    // Stun applied when the parry window expires without catching anything.
    // Should be longer/worse than a broken block to punish misfires.
    public float parryMissStunDuration = 0.25f;

    // Projectile prefab to spawn when a parry is successful. The prefab's tag
    // will be overwritten to "Weapon" at runtime so it can damage enemies.
    public GameObject parryProjectilePrefab;

    // Speed multiplier applied to the reflected projectile.
    public float parryReflectSpeedMultiplier = 1.4f;

    // How much of the block meter a failed parry costs (on top of the stun).
    public float parryMissBlockMeterCost = 1.5f;

    private bool isParrying;
    private float parryTimer;
    private bool parryLanded; // did this parry window catch something?

    // Public so UI / other systems can read it.
    public bool IsParrying => isParrying;
    // ──────────────────────────────────────────────────────────────────────────

    void Awake()
    {
        plrInput = GetComponentInParent<PlayerInput>();
        if (plrInput == null)
        {
            Debug.LogError("[WeaponController] No PlayerInput component found.");
            enabled = false;
            return;
        }
        atkAction = plrInput.actions.FindAction("Attack", true);
        if (atkAction == null)
        {
            Debug.LogError("[WeaponController] 'Attack' not found. Check if the Action Name is spelled exactly 'Attack' (case sensitive).", this);
            enabled = false;
        }
        blockAction = plrInput.actions.FindAction("Block", true);
        if (blockAction == null)
        {
            Debug.LogError("[WeaponController] 'Block' not found. Check if the Action Name is spelled exactly 'Block' (case sensitive).", this);
            enabled = false;
        }
    }

    void Start()
    {
        if (playerCapsule == null)
        {
            Debug.LogError("[WeaponController] playerCapsule is not assigned!", this);
            enabled = false;
            return;
        }
        wpnCollider = GetComponent<Collider>();
        if (wpnCollider == null)
            Debug.LogWarning("[WeaponController] No Collider found on Weapon - hitbox will not function.", this);
        if (wpnCollider != null)
        {
            wpnCollider.isTrigger = true;
            wpnCollider.enabled = false;
        }

        if (parryHitboxCollider != null)
        {
            parryHitboxCollider.isTrigger = true;
            parryHitboxCollider.enabled = false;

            // Wire up the relay component so we get callbacks.
            var relay = parryHitboxCollider.GetComponent<ParryHitbox>();
            if (relay == null)
                Debug.LogWarning("[WeaponController] parryHitboxCollider has no ParryHitbox component - add ParryHitbox.cs to that child object.", this);
            else
                relay.weapon = this;
        }
        else
        {
            Debug.LogWarning("[WeaponController] parryHitboxCollider is not assigned - parry reflect will not function.", this);
        }

        // Bake the resting transform exactly once from the scene-placed position.
        readyLocalPosThing = transform.localPosition;
        readyLocalRotThing = transform.localRotation;

        blockMeter = blockMeterMax;
        heavyDamageAmount = damageAmount * 2f;
    }

    void Update()
    {
        if (blockCooldownTimer > 0f)
            blockCooldownTimer -= Time.deltaTime;

        if (blockDrainCooldownTimer > 0f)
            blockDrainCooldownTimer -= Time.deltaTime;

        // Parry must be checked first - it takes priority over both attack and block.
        UpdateParryInput();

        if (!isParrying)
        {
            UpdateBlock();
            UpdateAttackInput();
        }

        if (isSwinging) UpdateSwing();
        if (isChargingHeavy) UpdateHeavyWindup();
        if (isHeavySwinging) UpdateHeavySwing();
        if (isParrying) UpdateParry();

        if (attackReadySlider != null)
            attackReadySlider.value = AttackSliderNormalized;
    }

    // ─── Parry input ──────────────────────────────────────────────────────────

    void UpdateParryInput()
    {
        bool parryTrigger =
            (atkAction.WasPressedThisFrame() && blockAction.IsPressed()) ||
            (blockAction.WasPressedThisFrame() && atkAction.IsPressed());

        // isBlocking intentionally excluded - parry should interrupt an active block
        bool busy = isSwinging || isChargingHeavy || isHeavySwinging ||
                    isStunned || isParrying;

        if (parryTrigger && !busy)
            StartParry();
    }

    void StartParry()
    {
        Debug.Log("[Weapon] Parry started.");
        isParrying = true;
        parryTimer = 0f;
        parryLanded = false;

        // Make sure normal attack and block states are cleared.
        isSwinging = false;
        isBlocking = false;
        isChargingHeavy = false;
        isHeavySwinging = false;

        // Weapon hitbox off, parry hitbox on.
        if (wpnCollider != null) wpnCollider.enabled = false;
        if (parryHitboxCollider != null) parryHitboxCollider.enabled = true;

        // Snap to block position for the visual tell.
        transform.localPosition = readyLocalPosThing + readyLocalRotThing * blockPos.localPosition;
        transform.localRotation = readyLocalRotThing * blockPos.localRotation;
    }

    void UpdateParry()
    {
        parryTimer += Time.deltaTime;

        if (parryTimer >= parryWindow)
            EndParry(success: parryLanded);
    }

    void EndParry(bool success)
    {
        isParrying = false;
        if (parryHitboxCollider != null) parryHitboxCollider.enabled = false;

        if (success)
        {
            Debug.Log("[Weapon] Parry successful!");
            // Clean exit - no penalty, small cooldown so it can't chain immediately.
            blockCooldownTimer = blockCooldown;
        }
        else
        {
            Debug.Log("[Weapon] Parry missed - applying punishment.");

            // Drain block meter so missing a parry has compound consequences.
            blockMeter = Mathf.Max(blockMeter - parryMissBlockMeterCost, 0f);

            // Apply a longer stun than a normal block-break.
            if (stunCoroutine != null) StopCoroutine(stunCoroutine);
            stunCoroutine = StartCoroutine(StunRoutine(parryMissStunDuration));

            blockCooldownTimer = blockCooldown;
        }

        // Return weapon to rest position.
        transform.localPosition = readyLocalPosThing;
        transform.localRotation = readyLocalRotThing;
    }

    // Called by ParryHitbox.cs when something enters the parry collider.
    public void OnParryContact(Collider other)
    {
        if (!isParrying) return;

        Debug.Log($"[Weapon] Parry contact: {other.name} tag: {other.tag}");

        if (other.CompareTag("EnemyProjectile"))
        {
            parryLanded = true;
            ReflectProjectile(other.gameObject);

            // End the parry window immediately on success so the player
            // can act again without waiting out the full window.
            EndParry(success: true);
        }
    }

    void ReflectProjectile(GameObject projectile)
    {
        if (parryProjectilePrefab == null)
        {
            Debug.LogWarning("[Weapon] parryProjectilePrefab is not assigned.");
            return;
        }

        // Reconstruct incoming direction from projectile toward the player.
        Vector3 toPlayer = (playerCapsule.position - projectile.transform.position);
        toPlayer.y = 0f;
        toPlayer.Normalize();

        projectile.SetActive(false);
        Destroy(projectile, 0.1f);

        GameObject reflected = Instantiate(
            parryProjectilePrefab,
            projectile.transform.position,
            projectile.transform.rotation
        );

        reflected.tag = "Weapon";

        // Hand off the reflected direction to the new script.
        ParriedProjectile proj = reflected.GetComponent<ParriedProjectile>();
        if (proj != null)
            proj.direction = -toPlayer; // flip: away from player, toward enemy
        else
            Debug.LogWarning("[Weapon] parryProjectilePrefab is missing a ParriedProjectile component.");

        Debug.Log("[Weapon] Projectile reflected!");
    }

    // ─────────────────────────────────────────────────────────────────────────

    void UpdateAttackInput()
    {
        bool busy = isSwinging || isChargingHeavy || isHeavySwinging ||
                    isBlocking || isStunned || isParrying;

        if (atkAction.WasPressedThisFrame() && !busy)
            holdTimer = 0f;

        if (atkAction.IsPressed() && !busy)
            holdTimer += Time.deltaTime;

        // Light attack
        if (atkAction.WasReleasedThisFrame() && !busy && holdTimer < heavyWindupDuration)
            StartSwing();

        // Begin heavy windup
        if (atkAction.IsPressed() && !busy && holdTimer >= heavyWindupDuration)
            StartHeavyWindup();
    }

    // LIGHT ATK
    void StartSwing()
    {
        Debug.Log($"[Weapon] Collider enabled: {wpnCollider.enabled}, isTrigger: {wpnCollider.isTrigger}");
        isSwinging = true;
        swingTimer = 0f;
        swingDirection *= -1;
        if (wpnCollider != null) wpnCollider.enabled = true;
        ApplySwingAngle(-swingHalfArc * swingDirection);
    }

    void UpdateSwing()
    {
        swingTimer += Time.deltaTime;
        float t = Mathf.Clamp01(swingTimer / swingDuration);
        float curved = swingCurve.Evaluate(t);
        float angle = Mathf.Lerp(-swingHalfArc, swingHalfArc, curved) * swingDirection;
        ApplySwingAngle(angle);
        Physics.SyncTransforms();
        if (t >= 1f) EndSwing();
    }

    void EndSwing()
    {
        isSwinging = false;
        if (wpnCollider != null) wpnCollider.enabled = false;
    }

    // HEAVY ATK
    void StartHeavyWindup()
    {
        isChargingHeavy = true;
        heavyWindupTimer = 0f;
        Debug.Log("[Weapon] Heavy attack windup started.");
    }

    void UpdateHeavyWindup()
    {
        heavyWindupTimer += Time.deltaTime;
        if (atkAction.WasReleasedThisFrame())
        {
            isChargingHeavy = false;
            Debug.Log("[Weapon] Heavy attack cancelled.");
            return;
        }
        if (heavyWindupTimer >= heavyWindupDuration)
        {
            isChargingHeavy = false;
            StartHeavySwing();
        }
    }

    void StartHeavySwing()
    {
        Debug.Log("[Weapon] Heavy swing fired.");
        isHeavySwinging = true;
        heavySwingTimer = 0f;
        heavySwingDirection *= -1;
        if (wpnCollider != null) wpnCollider.enabled = true;
        ApplySwingAngle(-heavySwingHalfArc * heavySwingDirection);
    }

    void UpdateHeavySwing()
    {
        heavySwingTimer += Time.deltaTime;
        float t = Mathf.Clamp01(heavySwingTimer / heavySwingDuration);
        float curved = heavySwingCurve.Evaluate(t);
        float angle = Mathf.Lerp(-heavySwingHalfArc, heavySwingHalfArc, curved) * heavySwingDirection;
        ApplySwingAngle(angle);
        Physics.SyncTransforms();
        if (t >= 1f) EndHeavySwing();
    }

    void EndHeavySwing()
    {
        isHeavySwinging = false;
        if (wpnCollider != null) wpnCollider.enabled = false;
        Debug.Log("[Weapon] Heavy swing ended.");
    }

    void ApplySwingAngle(float angleDeg)
    {
        Quaternion rot = Quaternion.AngleAxis(angleDeg, Vector3.up);
        transform.localPosition = rot * readyLocalPosThing;
        transform.localRotation = rot * readyLocalRotThing;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[Weapon] Trigger entered by: {other.name} tag: {other.tag}");
        if (other.CompareTag("Enemy"))
        {
            float dmg = isHeavySwinging ? heavyDamageAmount : damageAmount;
            other.gameObject.GetComponent<EnemyHP>().TakeDamage(dmg);

            if (isHeavySwinging)
            {
                // Direction away from player, flat on XZ
                Vector3 knockDir = (other.transform.position - playerCapsule.position);
                knockDir.y = 0f;
                knockDir.Normalize();

                EnemyPatrol ep = other.GetComponent<EnemyPatrol>();
                if (ep != null)
                    ep.TakeKnockback(knockDir, heavyKnockbackForce);

                if (ScreenImpactEffect.Instance != null)
                    ScreenImpactEffect.Instance.TriggerImpact();
            }
        }
    }

    // BLOCK
    void UpdateBlock()
    {
        bool wantsBlock = blockAction.IsPressed();
        bool canBlock = !isStunned && !isSwinging && !IsHeavyAttacking &&
                          !isParrying && blockCooldownTimer <= 0f && blockMeter > 0f;

        if (wantsBlock && canBlock)
        {
            if (!isBlocking) StartBlock();

            blockMeter -= blockMeterDrainRate * Time.deltaTime;

            if (blockMeter <= 0f)
            {
                blockMeter = 0f;
                ForceEndBlock(stun: true);
            }
        }
        else if (isBlocking)
        {
            ForceEndBlock(stun: false);
        }

        // Recharge meter while not blocking.
        if (!isBlocking && !isStunned)
            blockMeter = Mathf.Min(blockMeter + blockMeterRechargeRate * Time.deltaTime, blockMeterMax);

        if (isBlocking)
        {
            transform.localPosition = readyLocalPosThing + readyLocalRotThing * blockPos.localPosition;
            transform.localRotation = readyLocalRotThing * blockPos.localRotation;
        }

        if (blockMeterSlider != null)
            blockMeterSlider.value = blockMeter / blockMeterMax;
    }

    void StartBlock()
    {
        isBlocking = true;
        isSwinging = false;
        if (wpnCollider != null) wpnCollider.enabled = false;
    }

    void ForceEndBlock(bool stun)
    {
        isBlocking = false;
        blockCooldownTimer = blockCooldown;

        if (stun)
        {
            if (stunCoroutine != null) StopCoroutine(stunCoroutine);
            stunCoroutine = StartCoroutine(StunRoutine(stunDuration));
        }

        transform.localPosition = readyLocalPosThing;
        transform.localRotation = readyLocalRotThing;
    }

    // Duration-parameterised so block-break and parry-miss can use different lengths.
    IEnumerator StunRoutine(float duration)
    {
        isStunned = true;
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }

    // Called from PlayerHP when a heavy projectile is absorbed.
    private float blockDrainCooldown = 0.3f;
    private float blockDrainCooldownTimer;

    public void DrainBlockMeter(float amount)
    {
        if (blockDrainCooldownTimer > 0f) return;

        blockMeter = Mathf.Max(blockMeter - amount, 0f);
        blockDrainCooldownTimer = blockDrainCooldown;

        if (blockMeter <= 0f && isBlocking)
            ForceEndBlock(stun: true);
    }

    // 0 = busy/stunned, 0-1 = charging heavy, 1 = fully ready or mid-swing progress
    public float AttackSliderNormalized
    {
        get
        {
            if (isStunned) return 0f;
            if (isParrying) return 0f;

            if (!isChargingHeavy && !isSwinging && !isHeavySwinging && atkAction.IsPressed())
                return 1f - Mathf.Clamp01(holdTimer / heavyWindupDuration);

            if (isChargingHeavy) return 0f;

            if (isSwinging)
                return Mathf.Clamp01(swingTimer / swingDuration);

            if (isHeavySwinging)
                return Mathf.Clamp01(heavySwingTimer / heavySwingDuration);

            return 1f;
        }
    }
}
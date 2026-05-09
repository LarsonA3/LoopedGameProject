using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public Transform playerCapsule;
    //degrees from player forward to each side of the arc
    public float swingHalfArc = 60f;
    //how long full swing takes in seconds
    public float swingDuration = 0.25f;
    //curve controlling swing speed � flat = constant, ease-in/out = slow at edges
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
            Debug.LogWarning("[WeaponController] No Collider found on Weapon � hitbox will not function.", this);
        if (wpnCollider != null)
        {
            wpnCollider.isTrigger = true;
            wpnCollider.enabled = false;
        }
        // Bake the resting transform exactly once from the scene-placed position.
        // This is the permanent arc center for every swing.
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

        UpdateBlock();
        UpdateAttackInput();

        if (isSwinging) UpdateSwing();
        if (isChargingHeavy) UpdateHeavyWindup();
        if (isHeavySwinging) UpdateHeavySwing();

        if (attackReadySlider != null)
            attackReadySlider.value = AttackSliderNormalized;
    }

    //this now checks for holding or tapping lmb
    void UpdateAttackInput()
    {
        bool busy = isSwinging || isChargingHeavy || isHeavySwinging || isBlocking || isStunned;

        if (atkAction.WasPressedThisFrame() && !busy)
            holdTimer = 0f;

        if (atkAction.IsPressed() && !busy)
            holdTimer += Time.deltaTime;

        // light attack
        if (atkAction.WasReleasedThisFrame() && !busy && holdTimer < heavyWindupDuration)
            StartSwing();

        // begin heavy windup
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
        // Snap to swing start angle immediately.
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
        if (t >= 1f)
            EndSwing();
    }

    void EndSwing()
    {
        isSwinging = false;
        // Disables hitbox, weapon stays at the arc end position
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
        // player released button during windup � cancel
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
        // Snap to swing start angle immediately.
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
        if (t >= 1f)
            EndHeavySwing();
    }

    void EndHeavySwing()
    {
        isHeavySwinging = false;
        // Disables hitbox, weapon stays at the arc end position
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
            // Handle enemy hit logic here
            print("detected enemy hit");
            float dmg = isHeavySwinging ? heavyDamageAmount : damageAmount;
            other.gameObject.GetComponent<EnemyHP>().TakeDamage(dmg);
        }
    }

    // BLOCK
    void UpdateBlock()
    {
        bool wantsBlock = blockAction.IsPressed();
        bool canBlock = !isStunned && !isSwinging && !IsHeavyAttacking && blockCooldownTimer <= 0f && blockMeter > 0f;

        if (wantsBlock && canBlock)
        {
            if (!isBlocking)
                StartBlock();

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

        // recharge meter while not blocking
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
        // enforce cooldown on release so rapid tapping doesn't bypass the meter drain
        blockCooldownTimer = blockCooldown;

        if (stun)
        {
            if (stunCoroutine != null) StopCoroutine(stunCoroutine);
            stunCoroutine = StartCoroutine(StunRoutine());
        }

        transform.localPosition = readyLocalPosThing;
        transform.localRotation = readyLocalRotThing;
    }

    System.Collections.IEnumerator StunRoutine()
    {
        isStunned = true;
        yield return new WaitForSeconds(stunDuration);
        isStunned = false;
    }

    // called from PlayerHP when a heavy projectile is absorbed
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


    // 0 = busy/stunned, 0�1 = charging heavy, 1 = fully ready or mid-swing progress
    public float AttackSliderNormalized
    {
        get
        {
            if (isStunned) return 0f;

            // Holding button drain to 0 as charge builds toward heavy threshold
            if (!isChargingHeavy && !isSwinging && !isHeavySwinging && atkAction.IsPressed())
                return 1f - Mathf.Clamp01(holdTimer / heavyWindupDuration);

            // Locked into heavy windup hold at 0
            if (isChargingHeavy) return 0f;

            // Light swing fills 0->1 over swingDuration, hits 1 exactly when you can attack again
            if (isSwinging)
                return Mathf.Clamp01(swingTimer / swingDuration);

            // Heavy swing same, over heavySwingDuration
            if (isHeavySwinging)
                return Mathf.Clamp01(heavySwingTimer / heavySwingDuration);

            // Idle and ready
            return 1f;
        }
    }

}
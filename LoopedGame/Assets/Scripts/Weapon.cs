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
    //curve controlling swing speed — flat = constant, ease-in/out = slow at edges
    public AnimationCurve swingCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    public Transform blockPos;
    private Vector3 blockPosLocal;
    private Quaternion blockRotLocal;

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

    private bool isStunned;
    private float blockMeter;
    private float blockCooldownTimer;
    private Coroutine stunCoroutine;
    private InputAction blockAction;

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
            Debug.LogWarning("[WeaponController] No Collider found on Weapon — hitbox will not function.", this);
        if (wpnCollider != null)
        {
            wpnCollider.isTrigger = true;
            wpnCollider.enabled = false;
        }
        // Bake the resting transform exactly once from the scene-placed position.
        // This is the permanent arc center for every swing.
        readyLocalPosThing = transform.localPosition;
        readyLocalRotThing = transform.localRotation;

        // Bake BlockPos's transform into parent space once, before anything moves.
        if (blockPos != null)
        {
            blockPosLocal = transform.parent.InverseTransformPoint(blockPos.position);
            blockRotLocal = Quaternion.Inverse(transform.parent.rotation) * blockPos.rotation;
        }

        blockMeter = blockMeterMax;
    }

    void Update()
    {
        if (blockCooldownTimer > 0f)
            blockCooldownTimer -= Time.deltaTime;

        UpdateBlock();

        if (atkAction.WasPressedThisFrame() && !isSwinging && !isBlocking && !isStunned)
            StartSwing();
        if (isSwinging)
            UpdateSwing();
    }

    void UpdateBlock()
    {
        bool wantsBlock = blockAction.IsPressed();
        bool canBlock = !isStunned && !isSwinging && blockCooldownTimer <= 0f && blockMeter > 0f;

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
            transform.localPosition = blockPosLocal;
            transform.localRotation = blockRotLocal;
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
    public void DrainBlockMeter(float amount)
    {
        blockMeter = Mathf.Max(blockMeter - amount, 0f);
        if (blockMeter <= 0f && isBlocking)
            ForceEndBlock(stun: true);
    }

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
            other.gameObject.GetComponent<EnemyHP>().TakeDamage(damageAmount);
        }
    }
}
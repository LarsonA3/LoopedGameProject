using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{

    public Transform playerCapsule;


    //degrees from player forward to each side of the arc
    public float swingHalfArc = 60f;

    //how long full swing takes in seconds
    public float swingDuration = 0.25f;

    //curve controlling swing speed — flat = constant, ease-in/out = slow at edges
    public AnimationCurve swingCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private bool isSwinging;
    private float swingTimer;
    private int swingDirection = 1;


    private Vector3 readyLocalPosThing;
    private Quaternion readyLocalRotThing;

    private Collider wpnCollider;

    private PlayerInput plrInput;
    private InputAction atkAction;

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
    }

    void Update()
    {
        if (atkAction.WasPressedThisFrame() && !isSwinging)
            StartSwing();

        if (isSwinging)
            UpdateSwing();
    }

    void StartSwing()
    {
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
}

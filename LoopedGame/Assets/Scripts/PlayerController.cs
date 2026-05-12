using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class TopDownController : MonoBehaviour
{
    private Collider playerCollider;
    public float moveSpeed = 6f;
    public float gravity = -20f;
    public float stickRotationSpeed = 10f;
    public float stickDeadzone = 0.2f;
    private float baseMoveSpeed;
    private Coroutine temporaryMoveSpeedRoutine;

    [SerializeField] private float dashDistance = 2f;
    [SerializeField] private float dashCooldown = 1f;

    private CharacterController cc;
    private Camera cam;
    private float verticalVelocity;
    private Vector2 moveInput;
    public float LastDashTime => lastDashTime;
    private float lastDashTime = -99f;

    private bool isGamepad;

    [SerializeField] private Slider dashCooldownSlider;
    [SerializeField] private TMP_Text dashChargesText;

    // tracks when exhausted recharge started for slider
    private float exhaustedStartTime;
    private Weapon weapon;
    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();

    void Start()
    {
        cc = GetComponent<CharacterController>();
        cam = Camera.main;
        weapon = GetComponentInChildren<Weapon>();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerCollider = GetComponent<Collider>();
        currentDashCharges = maxdashcharges;
        ApplySavedUpgrades();
        baseMoveSpeed = moveSpeed;
        UpdateDashUI();
    }

    void Update()
    {
        Move();
        Rotate();

        if (Mouse.current != null && Mouse.current.delta.ReadValue().sqrMagnitude > 0.1f)
            isGamepad = false;

        if (Keyboard.current.leftShiftKey.wasPressedThisFrame) Trytodash();

        Gamepad pad = Gamepad.current;
        if (pad != null && pad.buttonSouth.wasPressedThisFrame) Trytodash();

        UpdateDashUI();
    }

    void Move()
    {
        float speedMultiplier = weapon != null ? weapon.MoveSpeedMultiplier : 1f;
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y).normalized * moveSpeed * speedMultiplier;

        if (cc.isGrounded) verticalVelocity = -2f;
        else verticalVelocity += gravity * Time.deltaTime;

        move.y = verticalVelocity;
        cc.Move(move * Time.deltaTime);
    }

    // dashing --------------

    public int maxdashcharges = 1;
    private bool canDash = true;
    private bool isDashing;
    private int currentDashCharges;
    public float cooldownAfterChargesExhausted = 3f;
    public int absoluteMaxDashCharges = 5; // hard cap prevents any UI overflow
    public int dashChargeProgress;

    void Trytodash()
    {
        if (!canDash) return;
        if (Time.time < lastDashTime + dashCooldown) return;
        isDashing = true;

        Vector3 dashDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        if (dashDir == Vector3.zero) dashDir = transform.forward;

        Vector3 destination = FindDashDestination(dashDir);
        if (destination == transform.position) return;

        cc.enabled = false;
        transform.position = destination;
        cc.enabled = true;

        lastDashTime = Time.time;
        isDashing = false;

        currentDashCharges -= 1;
        if (currentDashCharges <= 0)
        {
            currentDashCharges = 0;
            exhaustedStartTime = Time.time;
            canDash = false;
            isDashing = false;
            print("all dash charges exhausted");
            StartCoroutine(waitForDash());
        }
    }

    public IEnumerator waitForDash()
    {
        yield return new WaitForSeconds(cooldownAfterChargesExhausted);
        currentDashCharges = maxdashcharges;
        canDash = true;
        print("Dashes refreshed");
    }

    Vector3 FindDashDestination(Vector3 dashDir)
    {
        int steps = 10;
        float stepSize = dashDistance / steps;
        Vector3 lastValid = transform.position;

        for (int i = 1; i <= steps; i++)
        {
            Vector3 checkPos = transform.position + dashDir * (stepSize * i);
            if (IsWalkable(checkPos))
                lastValid = checkPos;
            else
                break;
        }

        return lastValid;
    }

    bool IsWalkable(Vector3 pos)
    {
        RaycastHit[] hits = Physics.RaycastAll(pos + Vector3.up * 2f, Vector3.down, 4f);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == playerCollider) continue;
            if (hit.collider.CompareTag("WALKABLE PLAYER FLOOR")) return true;
        }
        return false;
    }

    // ------------------dash ui stuff

    void UpdateDashUI()
    {
        if (dashCooldownSlider != null)
        {
            float fill;

            if (!canDash)
            {
                fill = Mathf.Clamp01((Time.time - exhaustedStartTime) / cooldownAfterChargesExhausted);
            }
            else if (Time.time < lastDashTime + dashCooldown)
            {
                fill = Mathf.Clamp01((Time.time - lastDashTime) / dashCooldown);
            }
            else
            {
                fill = 1f;
            }

            dashCooldownSlider.value = fill;
        }
        //text
        if (dashChargesText != null)
            dashChargesText.text = $"{currentDashCharges}/{maxdashcharges}";
    }

    //  ------- plr rotation

    void Rotate()
    {
        Gamepad pad = Gamepad.current;
        if (pad != null)
        {
            Vector2 stick = pad.rightStick.ReadValue();
            if (stick.magnitude > stickDeadzone)
            {
                isGamepad = true;
                RotateController(stick);
                return;
            }
        }

        if (isGamepad) return;
        RotateMOUSE();
    }

    void RotateMOUSE()
    {
        if (Mouse.current == null) return;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 lookPoint = ray.GetPoint(distance);
            lookPoint.y = transform.position.y;
            Vector3 dir = lookPoint - transform.position;
            if (dir.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    void RotateController(Vector2 stick)
    {
        Quaternion targetRot = Quaternion.LookRotation(new Vector3(stick.x, 0f, stick.y));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, stickRotationSpeed * Time.deltaTime);
    }

    // ugprades ---------------

    public void addMoveSpd(float amount) => moveSpeed += amount;

    public void reduceDashCD(float amount) => dashCooldown = Mathf.Max(0.1f, dashCooldown - amount);

    public void increaseDashDist(float amount) => dashDistance += amount;

    public void addDashCharge(float amount)
    {
        maxdashcharges = Mathf.Clamp(maxdashcharges + (int)amount, 1, absoluteMaxDashCharges);
        currentDashCharges = Mathf.Clamp(currentDashCharges + (int)amount, 0, maxdashcharges);
        UpdateDashUI();
    }

    public void ReduceDashCooldown(float amount)
    {
        reduceDashCD(amount);
    }

    public void IncreaseDashDistance(float amount)
    {
        increaseDashDist(amount);
    }

    public void AddMoveSpeed(float amount)
    {
        addMoveSpd(amount);
    }

    public bool IsDashing => isDashing;

    public void AddDashChargeProgress(int amount)
    {
        dashChargeProgress += amount;
        
        while (dashChargeProgress >= 5 && maxdashcharges < absoluteMaxDashCharges) 
        {
            dashChargeProgress -= 5;
            maxdashcharges++;
            currentDashCharges++;
        }

        maxdashcharges = Mathf.Clamp(maxdashcharges, 1, absoluteMaxDashCharges);
        currentDashCharges = Mathf.Clamp(currentDashCharges, 0, maxdashcharges);
    }

    public void SetTemporaryMoveSpeedMultiplier(float multiplier, float duration)
    {
        if (temporaryMoveSpeedRoutine != null)
        {
            StopCoroutine(temporaryMoveSpeedRoutine);
        }

        temporaryMoveSpeedRoutine = StartCoroutine(TemporaryMoveSpeedRoutine(multiplier, duration));
    }

    private IEnumerator TemporaryMoveSpeedRoutine(float multiplier, float duration)
    {
        moveSpeed *= baseMoveSpeed * Mathf.Max(0.1f, multiplier);
        yield return new WaitForSeconds(duration);
        moveSpeed = baseMoveSpeed;
        temporaryMoveSpeedRoutine = null;
    }

    private void ApplySavedUpgrades()
    {
        if (UpgradeState.Instance == null) return;

        moveSpeed += UpgradeState.Instance.moveSpeedBonus;
        dashDistance += UpgradeState.Instance.dashDistanceBonus;
        dashCooldown = Mathf.Max(0.1f, dashCooldown - UpgradeState.Instance.dashCooldownReduction);
    }

    public void ResetRunMovementState()
    {
        verticalVelocity = 0;
        moveInput = Vector2.zero;

        lastDashTime = -99f;
        currentDashCharges = maxdashcharges;
        canDash = true;
        exhaustedStartTime = 0f;

        isGamepad = false;
        if (temporaryMoveSpeedRoutine != null)
        {
            StopCoroutine(temporaryMoveSpeedRoutine);
            temporaryMoveSpeedRoutine = null;
        }

        if (baseMoveSpeed > 0f)
        {
            moveSpeed = baseMoveSpeed;
        }

        UpdateDashUI();
        Debug.Log("[TopDownController] Run movement state reset.");
    }
}
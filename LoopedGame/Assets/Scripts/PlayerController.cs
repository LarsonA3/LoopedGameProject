using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class TopDownController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float rotationSpeed = 14f;

    [Header("Dash")]
    public float dashDistance = 5f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1f;
    public float minimumDashCooldown = 0.5f;
    public int maxDashCharges = 1;
    public int absoluteMaxDashCharges = 5;

    [Header("Camera")]
    public Camera playerCamera;

    [Header("Optional UI")]
    public Slider dashCooldownSlider;
    public Slider dashChargesSlider;
    public TMP_Text dashChargeText;

    private CharacterController characterController;
    private PlayerInput playerInput;
    private Weapon weapon;

    private InputAction moveAction;
    private InputAction dashAction;

    private Vector3 moveDirection;
    private Vector3 dashDirection;

    private bool isDashing;
    private float dashTimer;
    private float dashCooldownTimer;
    private int currentDashCharges;
    private int dashChargeProgress;

    private float temporaryMoveSpeedMultiplier = 1f;
    private float temporaryMoveSpeedEndTime;

    public float LastDashTime { get; private set; }
    public bool IsDashing => isDashing;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        weapon = GetComponentInChildren<Weapon>();

        if (playerInput == null)
        {
            Debug.LogError("[TopDownController] No PlayerInput found.");
            enabled = false;
            return;
        }

        moveAction = playerInput.actions.FindAction("Move", true);
        dashAction = playerInput.actions.FindAction("Dash", true);

        if (moveAction == null)
        {
            Debug.LogError("[TopDownController] Input Action 'Move' not found.");
            enabled = false;
            return;
        }

        if (dashAction == null)
        {
            Debug.LogError("[TopDownController] Input Action 'Dash' not found.");
            enabled = false;
            return;
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        maxDashCharges = Mathf.Clamp(maxDashCharges, 1, absoluteMaxDashCharges);
        currentDashCharges = maxDashCharges;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ApplySavedUpgrades();
        UpdateDashUI();
    }

    private void Update()
    {
        UpdateTemporaryMoveSpeed();

        if (isDashing)
        {
            UpdateDash();
            UpdateDashUI();
            return;
        }

        UpdateDashCooldown();

        if (weapon != null && (weapon.IsStunned || weapon.IsHeavyAttacking))
        {
            UpdateDashUI();
            return;
        }

        UpdateMovement();
        UpdateDashInput();
        UpdateDashUI();
    }

    private void UpdateMovement()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        moveDirection = new Vector3(input.x, 0f, input.y);

        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }

        float weaponMoveMultiplier = weapon != null ? weapon.MoveSpeedMultiplier : 1f;
        float finalMoveSpeed = moveSpeed * temporaryMoveSpeedMultiplier * weaponMoveMultiplier;

        characterController.Move(moveDirection * finalMoveSpeed * Time.deltaTime);

        if (moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void UpdateDashInput()
    {
        if (!dashAction.WasPressedThisFrame())
        {
            return;
        }

        if (currentDashCharges <= 0)
        {
            return;
        }

        StartDash();
    }

    private void StartDash()
    {
        if (currentDashCharges <= 0)
        {
            return;
        }

        isDashing = true;
        dashTimer = 0f;
        LastDashTime = Time.time;

        currentDashCharges = Mathf.Max(0, currentDashCharges - 1);

        if (currentDashCharges < maxDashCharges)
        {
            dashCooldownTimer = Mathf.Max(dashCooldown, minimumDashCooldown);
        }

        dashDirection = moveDirection;

        if (dashDirection.sqrMagnitude <= 0.001f)
        {
            dashDirection = transform.forward;
        }

        dashDirection.y = 0f;
        dashDirection.Normalize();

        PlayerRareCardAbilityController rareCards =
            GetComponent<PlayerRareCardAbilityController>();

        PlayerEpicCardAbilityController epicCards =
            GetComponent<PlayerEpicCardAbilityController>();

        PlayerLegendaryCardAbilityController legendaryCards =
            GetComponent<PlayerLegendaryCardAbilityController>();

        if (rareCards != null)
        {
            rareCards.OnDash();
        }

        if (epicCards != null)
        {
            epicCards.OnDash();
        }

        if (legendaryCards != null)
        {
            legendaryCards.OnDash();
        }

        DashHitbox dashHitbox = GetComponentInChildren<DashHitbox>();

        if (dashHitbox != null)
        {
            dashHitbox.BeginDashHitbox();
        }
    }

    private void UpdateDash()
    {
        dashTimer += Time.deltaTime;

        float telemetryMultiplier = 1f;

        PlayerLegendaryCardAbilityController legendaryCards =
            GetComponent<PlayerLegendaryCardAbilityController>();

        if (legendaryCards != null)
        {
            telemetryMultiplier = legendaryCards.GetWeaponizedTelemetryDashMultiplier();
        }

        float dashSpeed = (dashDistance * telemetryMultiplier) / dashDuration;

        characterController.Move(dashDirection * dashSpeed * Time.deltaTime);

        if (dashTimer >= dashDuration)
        {
            EndDash();
        }
    }

    private void EndDash()
    {
        isDashing = false;

        DashHitbox dashHitbox = GetComponentInChildren<DashHitbox>();

        if (dashHitbox != null)
        {
            dashHitbox.EndDashHitbox();
        }
    }

    private void UpdateDashCooldown()
    {
        maxDashCharges = Mathf.Clamp(maxDashCharges, 1, absoluteMaxDashCharges);
        currentDashCharges = Mathf.Clamp(currentDashCharges, 0, maxDashCharges);

        if (currentDashCharges >= maxDashCharges)
        {
            dashCooldownTimer = 0f;
            return;
        }

        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
            return;
        }

        currentDashCharges = Mathf.Min(currentDashCharges + 1, maxDashCharges);

        if (currentDashCharges < maxDashCharges)
        {
            dashCooldownTimer = Mathf.Max(dashCooldown, minimumDashCooldown);
        }
        else
        {
            dashCooldownTimer = 0f;
        }
    }

    private void UpdateDashUI()
    {
        if (dashCooldownSlider != null)
        {
            if (currentDashCharges >= maxDashCharges)
            {
                dashCooldownSlider.value = 1f;
            }
            else
            {
                float safeCooldown = Mathf.Max(dashCooldown, minimumDashCooldown);
                dashCooldownSlider.value = 1f - Mathf.Clamp01(dashCooldownTimer / safeCooldown);
            }
        }

        if (dashChargesSlider != null)
        {
            dashChargesSlider.maxValue = maxDashCharges;
            dashChargesSlider.value = currentDashCharges;
        }

        if (dashChargeText != null)
        {
            dashChargeText.text = currentDashCharges + " / " + maxDashCharges;
        }
    }

    public void ReduceCurrentDashCooldown(float amount)
    {
        dashCooldownTimer = Mathf.Max(0f, dashCooldownTimer - amount);
    }

    public void SetTemporaryMoveSpeedMultiplier(float multiplier, float duration)
    {
        temporaryMoveSpeedMultiplier = Mathf.Max(0.1f, multiplier);

        if (duration <= 0f)
        {
            temporaryMoveSpeedEndTime = 0f;
            return;
        }

        temporaryMoveSpeedEndTime = Time.time + duration;
    }

    private void UpdateTemporaryMoveSpeed()
    {
        if (temporaryMoveSpeedMultiplier == 1f)
        {
            return;
        }

        if (temporaryMoveSpeedEndTime <= 0f)
        {
            temporaryMoveSpeedMultiplier = 1f;
            return;
        }

        if (Time.time >= temporaryMoveSpeedEndTime)
        {
            temporaryMoveSpeedMultiplier = 1f;
            temporaryMoveSpeedEndTime = 0f;
        }
    }

    private void ApplySavedUpgrades()
    {
        maxDashCharges = Mathf.Clamp(maxDashCharges, 1, absoluteMaxDashCharges);

        if (UpgradeState.Instance == null)
        {
            currentDashCharges = maxDashCharges;
            return;
        }

        moveSpeed += UpgradeState.Instance.moveSpeedBonus;
        dashDistance += UpgradeState.Instance.dashDistanceBonus;
        dashCooldown = Mathf.Max(minimumDashCooldown, dashCooldown - UpgradeState.Instance.dashCooldownReduction);

        int savedDashProgress = Mathf.FloorToInt(UpgradeState.Instance.dashChargeProgress);

        while (savedDashProgress >= 5 && maxDashCharges < absoluteMaxDashCharges)
        {
            savedDashProgress -= 5;
            maxDashCharges++;
        }

        maxDashCharges = Mathf.Clamp(maxDashCharges, 1, absoluteMaxDashCharges);
        currentDashCharges = maxDashCharges;
    }

    public void AddMoveSpeed(float amount)
    {
        moveSpeed += amount;
    }

    public void IncreaseDashDistance(float amount)
    {
        dashDistance += amount;
    }

    public void ReduceDashCooldown(float amount)
    {
        dashCooldown = Mathf.Max(minimumDashCooldown, dashCooldown - amount);
    }

    public void AddDashChargeProgress(int amount)
    {
        dashChargeProgress += amount;

        while (dashChargeProgress >= 5 && maxDashCharges < absoluteMaxDashCharges)
        {
            dashChargeProgress -= 5;
            maxDashCharges++;
            currentDashCharges++;
        }

        maxDashCharges = Mathf.Clamp(maxDashCharges, 1, absoluteMaxDashCharges);
        currentDashCharges = Mathf.Clamp(currentDashCharges, 0, maxDashCharges);
    }

    public void ResetRunMovementState()
    {
        isDashing = false;
        dashTimer = 0f;
        dashCooldownTimer = 0f;

        maxDashCharges = Mathf.Clamp(maxDashCharges, 1, absoluteMaxDashCharges);
        currentDashCharges = maxDashCharges;

        temporaryMoveSpeedMultiplier = 1f;
        temporaryMoveSpeedEndTime = 0f;

        moveDirection = Vector3.zero;
        dashDirection = Vector3.zero;

        UpdateDashUI();

        Debug.Log("[TopDownController] Run movement state reset.");
    }
}

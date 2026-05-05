using UnityEngine;
using UnityEngine.InputSystem;

public class StartingWeaponSelector : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private GameObject player;

    [Header("Weapon Prefabs")]
    [SerializeField] private GameObject mopPrefab;
    [SerializeField] private GameObject stunMacePrefab;
    [SerializeField] private GameObject bootlegLightsaberPrefab;
    [SerializeField] private GameObject gravityHammerPrefab;
    [SerializeField] private GameObject kineticRiotShieldPrefab;

    [Header("Future UI - Optional For Now")]
    [SerializeField] private GameObject weaponSelectPanel;

    [Header("Temporary No-UI Fallback")]
    [SerializeField] private bool autoEquipMopIfNoUI = true;
    [SerializeField] private bool allowNumberKeySelectionIfNoUI = true;

    private bool choosingWeapon;

    private void Start()
    {
        StartWeaponChoice();
    }

    private void Update()
    {
        if (!choosingWeapon)
        {
            return;
        }

        if (weaponSelectPanel != null)
        {
            return;
        }

        if (!allowNumberKeySelectionIfNoUI)
        {
            return;
        }

        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            TryChooseWeapon(WeaponType.Mop, mopPrefab);
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            TryChooseWeapon(WeaponType.StunMace, stunMacePrefab);
        }

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            TryChooseWeapon(WeaponType.BootlegLightsaber, bootlegLightsaberPrefab);
        }

        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            TryChooseWeapon(WeaponType.GravityHammer, gravityHammerPrefab);
        }

        if (Keyboard.current.digit5Key.wasPressedThisFrame)
        {
            TryChooseWeapon(WeaponType.KineticRiotShield, kineticRiotShieldPrefab);
        }
    }

    public void StartWeaponChoice()
    {
        choosingWeapon = true;

        if (weaponSelectPanel != null)
        {
            weaponSelectPanel.SetActive(true);
            DisablePlayerInput();

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            Debug.Log("[StartingWeaponSelector] UI weapon selection started.");
            return;
        }

        if (autoEquipMopIfNoUI)
        {
            TryChooseWeapon(WeaponType.Mop, mopPrefab);
            return;
        }

        DisablePlayerInput();

        Debug.Log("[StartingWeaponSelector] No UI assigned. Temporary weapon selection:");
        Debug.Log("1 = Mop");
        Debug.Log("2 = Stun Mace if unlocked");
        Debug.Log("3 = Bootleg Lightsaber if unlocked");
        Debug.Log("4 = Gravity Hammer if unlocked");
        Debug.Log("5 = Kinetic Riot Shield if unlocked");
    }

    public void ChooseMop()
    {
        TryChooseWeapon(WeaponType.Mop, mopPrefab);
    }

    public void ChooseStunMace()
    {
        TryChooseWeapon(WeaponType.StunMace, stunMacePrefab);
    }

    public void ChooseBootlegLightsaber()
    {
        TryChooseWeapon(WeaponType.BootlegLightsaber, bootlegLightsaberPrefab);
    }

    public void ChooseGravityHammer()
    {
        TryChooseWeapon(WeaponType.GravityHammer, gravityHammerPrefab);
    }

    public void ChooseKineticRiotShield()
    {
        TryChooseWeapon(WeaponType.KineticRiotShield, kineticRiotShieldPrefab);
    }

    private void TryChooseWeapon(WeaponType weaponType, GameObject weaponPrefab)
    {
        if (WeaponUnlockState.Instance == null)
        {
            Debug.LogWarning("[StartingWeaponSelector] No WeaponUnlockState found.");
            return;
        }

        if (!WeaponUnlockState.Instance.IsUnlocked(weaponType))
        {
            Debug.Log("[StartingWeaponSelector] " + weaponType + " is not unlocked yet.");
            return;
        }

        if (weaponPrefab == null)
        {
            Debug.LogWarning("[StartingWeaponSelector] No prefab assigned for " + weaponType);
            return;
        }

        if (player == null)
        {
            Debug.LogWarning("[StartingWeaponSelector] No player assigned.");
            return;
        }

        PlayerWeaponAttack weaponAttack = player.GetComponent<PlayerWeaponAttack>();

        if (weaponAttack == null)
        {
            Debug.LogWarning("[StartingWeaponSelector] Player has no PlayerWeaponAttack script.");
            return;
        }

        weaponAttack.EquipWeaponPrefab(weaponPrefab);

        choosingWeapon = false;

        if (weaponSelectPanel != null)
        {
            weaponSelectPanel.SetActive(false);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        EnablePlayerInput();

        Debug.Log("[StartingWeaponSelector] Chose weapon: " + weaponType);
    }

    private void DisablePlayerInput()
    {
        if (player == null)
        {
            return;
        }

        PlayerInput input = player.GetComponentInChildren<PlayerInput>();

        if (input != null)
        {
            input.enabled = false;
        }
    }

    private void EnablePlayerInput()
    {
        if (player == null)
        {
            return;
        }

        PlayerInput input = player.GetComponentInChildren<PlayerInput>();

        if (input != null)
        {
            input.enabled = true;
        }
    }
}

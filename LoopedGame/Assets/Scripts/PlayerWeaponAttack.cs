using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponAttack : MonoBehaviour
{
    [Header("Current Weapon")]
    [SerializeField] private WeaponBase currentWeapon;

    public WeaponBase CurrentWeapon => currentWeapon;

    private void Start()
    {
        if (currentWeapon != null)
        {
            currentWeapon.SetOwner(transform);
        }
    }

    private void Update()
    {
        CheckAttackInput();
        CheckAbilityInput();
    }

    private void CheckAttackInput()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Attack();
        }

        Gamepad pad = Gamepad.current;

        if (pad != null && pad.buttonWest.wasPressedThisFrame)
        {
            Attack();
        }
    }

    private void CheckAbilityInput()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            SpecialAttack();
        }

        Gamepad pad = Gamepad.current;

        if (pad != null && pad.buttonNorth.wasPressedThisFrame)
        {
            SpecialAttack();
        }
    }

    private void Attack()
    {
        if (currentWeapon == null)
        {
            Debug.LogWarning("[PlayerWeaponAttack] No current weapon assigned.");
            return;
        }

        currentWeapon.Attack();
    }

    private void SpecialAttack()
    {
        if (currentWeapon == null)
        {
            Debug.LogWarning("[PlayerWeaponAttack] No current weapon assigned.");
            return;
        }

        currentWeapon.SpecialAttack();
    }

    public void EquipWeapon(WeaponBase newWeapon)
    {
        currentWeapon = newWeapon;

        if (currentWeapon != null)
        {
            currentWeapon.SetOwner(transform);
        }
    }
}

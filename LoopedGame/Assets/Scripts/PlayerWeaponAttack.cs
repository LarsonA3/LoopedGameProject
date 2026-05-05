using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponAttack : MonoBehaviour
{
    [Header("Weapon Setup")]
    [SerializeField] private Transform weaponHolder;
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

    public void EquipWeaponPrefab(GameObject weaponPrefab)
    {
        if (weaponPrefab == null)
        {
            Debug.LogWarning("[PlayerWeaponAttack] Tried to equip a null weapon prefab.");
            return;
        }

        if (weaponHolder == null)
        {
            Debug.LogWarning("[PlayerWeaponAttack] No WeaponHolder assigned.");
            return;
        }

        if (currentWeapon != null)
        {
            Destroy(currentWeapon.gameObject);
        }

        GameObject newWeaponObject = Instantiate(
            weaponPrefab,
            weaponHolder.position,
            weaponHolder.rotation,
            weaponHolder
        );

        newWeaponObject.transform.localPosition = Vector3.zero;
        newWeaponObject.transform.localRotation = Quaternion.identity;

        WeaponBase newWeapon = newWeaponObject.GetComponent<WeaponBase>();

        if (newWeapon == null)
        {
            Debug.LogWarning("[PlayerWeaponAttack] Equipped prefab has no WeaponBase or child weapon script.");
            Destroy(newWeaponObject);
            return;
        }

        currentWeapon = newWeapon;
        currentWeapon.SetOwner(transform);
    }
}

using UnityEngine;

public class WeaponUnlockState : MonoBehaviour
{
    public static WeaponUnlockState Instance;

    [Header("Unlocked Weapons")]
    [SerializeField] private bool mopUnlocked = true;
    [SerializeField] private bool stunMaceUnlocked;
    [SerializeField] private bool bootlegLightsaberUnlocked;
    [SerializeField] private bool gravityHammerUnlocked;
    [SerializeField] private bool kineticRiotShieldUnlocked;

    public bool MopUnlocked => mopUnlocked;
    public bool StunMaceUnlocked => stunMaceUnlocked;
    public bool BootlegLightsaberUnlocked => bootlegLightsaberUnlocked;
    public bool GravityHammerUnlocked => gravityHammerUnlocked;
    public bool KineticRiotShieldUnlocked => kineticRiotShieldUnlocked;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadUnlocks();
    }

    public bool IsUnlocked(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.Mop:
                return mopUnlocked;

            case WeaponType.StunMace:
                return stunMaceUnlocked;

            case WeaponType.BootlegLightsaber:
                return bootlegLightsaberUnlocked;

            case WeaponType.GravityHammer:
                return gravityHammerUnlocked;

            case WeaponType.KineticRiotShield:
                return kineticRiotShieldUnlocked;

            default:
                return false;
        }
    }

    public void UnlockWeapon(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.Mop:
                mopUnlocked = true;
                break;

            case WeaponType.StunMace:
                stunMaceUnlocked = true;
                break;

            case WeaponType.BootlegLightsaber:
                bootlegLightsaberUnlocked = true;
                break;

            case WeaponType.GravityHammer:
                gravityHammerUnlocked = true;
                break;

            case WeaponType.KineticRiotShield:
                kineticRiotShieldUnlocked = true;
                break;
        }

        SaveUnlocks();

        Debug.Log("[WeaponUnlockState] Unlocked: " + weaponType);
    }

    public void ResetUnlocksForTesting()
    {
        mopUnlocked = true;
        stunMaceUnlocked = false;
        bootlegLightsaberUnlocked = false;
        gravityHammerUnlocked = false;
        kineticRiotShieldUnlocked = false;

        SaveUnlocks();
    }

    private void SaveUnlocks()
    {
        PlayerPrefs.SetInt("MopUnlocked", mopUnlocked ? 1 : 0);
        PlayerPrefs.SetInt("StunMaceUnlocked", stunMaceUnlocked ? 1 : 0);
        PlayerPrefs.SetInt("BootlegLightsaberUnlocked", bootlegLightsaberUnlocked ? 1 : 0);
        PlayerPrefs.SetInt("GravityHammerUnlocked", gravityHammerUnlocked ? 1 : 0);
        PlayerPrefs.SetInt("KineticRiotShieldUnlocked", kineticRiotShieldUnlocked ? 1 : 0);

        PlayerPrefs.Save();
    }

    private void LoadUnlocks()
    {
        mopUnlocked = PlayerPrefs.GetInt("MopUnlocked", 1) == 1;
        stunMaceUnlocked = PlayerPrefs.GetInt("StunMaceUnlocked", 0) == 1;
        bootlegLightsaberUnlocked = PlayerPrefs.GetInt("BootlegLightsaberUnlocked", 0) == 1;
        gravityHammerUnlocked = PlayerPrefs.GetInt("GravityHammerUnlocked", 0) == 1;
        kineticRiotShieldUnlocked = PlayerPrefs.GetInt("KineticRiotShieldUnlocked", 0) == 1;
    }
}

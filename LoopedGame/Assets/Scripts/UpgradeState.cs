using UnityEngine;

public class UpgradeState : MonoBehaviour
{
    public static UpgradeState Instance;

    [Header("Persistent Upgrade Totals")]
    public float moveSpeedBonus;
    public float dashDistanceBonus;
    public float dashCooldownReduction;

    public float maxHPBonus;
    public float invincibilityBonus;
    public float playerKnockbackResistance;

    public float attackDamageBonus;
    public float weaponAttackCooldownReduction;
    public float specialCooldownReduction;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Load();
    }

    public void AddMoveSpeed(float amount)
    {
        moveSpeedBonus += amount;
        Save();
    }

    public void AddDashDistance(float amount)
    {
        dashDistanceBonus += amount;
        Save();
    }

    public void ReduceDashCooldown(float amount)
    {
        dashCooldownReduction += amount;
        Save();
    }

    public void AddMaxHP(float amount)
    {
        maxHPBonus += amount;
        Save();
    }

    public void AddInvincibility(float amount)
    {
        invincibilityBonus += amount;
        Save();
    }

    public void AddKnockbackResistance(float amount)
    {
        playerKnockbackResistance += amount;
        playerKnockbackResistance = Mathf.Clamp(playerKnockbackResistance, 0f, 0.8f);
        Save();
    }

    public void AddAttackDamage(float amount)
    {
        attackDamageBonus += amount;
        Save();
    }

    public void ReduceWeaponAttackCooldown(float amount)
    {
        weaponAttackCooldownReduction += amount;
        Save();
    }

    public void ReduceSpecialCooldown(float amount)
    {
        specialCooldownReduction += amount;
        Save();
    }

    public void ResetUpgrades()
    {
        moveSpeedBonus = 0f;
        dashDistanceBonus = 0f;
        dashCooldownReduction = 0f;

        maxHPBonus = 0f;
        invincibilityBonus = 0f;
        playerKnockbackResistance = 0f;

        attackDamageBonus = 0f;
        weaponAttackCooldownReduction = 0f;
        specialCooldownReduction = 0f;

        Save();
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("moveSpeedBonus", moveSpeedBonus);
        PlayerPrefs.SetFloat("dashDistanceBonus", dashDistanceBonus);
        PlayerPrefs.SetFloat("dashCooldownReduction", dashCooldownReduction);

        PlayerPrefs.SetFloat("maxHPBonus", maxHPBonus);
        PlayerPrefs.SetFloat("invincibilityBonus", invincibilityBonus);
        PlayerPrefs.SetFloat("playerKnockbackResistance", playerKnockbackResistance);

        PlayerPrefs.SetFloat("attackDamageBonus", attackDamageBonus);
        PlayerPrefs.SetFloat("weaponAttackCooldownReduction", weaponAttackCooldownReduction);
        PlayerPrefs.SetFloat("specialCooldownReduction", specialCooldownReduction);

        PlayerPrefs.Save();
    }

    private void Load()
    {
        moveSpeedBonus = PlayerPrefs.GetFloat("moveSpeedBonus", 0f);
        dashDistanceBonus = PlayerPrefs.GetFloat("dashDistanceBonus", 0f);
        dashCooldownReduction = PlayerPrefs.GetFloat("dashCooldownReduction", 0f);

        maxHPBonus = PlayerPrefs.GetFloat("maxHPBonus", 0f);
        invincibilityBonus = PlayerPrefs.GetFloat("invincibilityBonus", 0f);
        playerKnockbackResistance = PlayerPrefs.GetFloat("playerKnockbackResistance", 0f);

        attackDamageBonus = PlayerPrefs.GetFloat("attackDamageBonus", 0f);
        weaponAttackCooldownReduction = PlayerPrefs.GetFloat("weaponAttackCooldownReduction", 0f);
        specialCooldownReduction = PlayerPrefs.GetFloat("specialCooldownReduction", 0f);
    }
}

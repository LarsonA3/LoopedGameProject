using UnityEngine;

public class UpgradeState : MonoBehaviour
{
    public static UpgradeState Instance;

    [Header("Weapon Stat Totals")]
    public float lightAttackDamageBonus;
    public float heavyAttackDamageBonus;
    public float lightAttackSpeedIncrease;
    public float heavyAttackWindupReduction;
    public float heavyAttackSpeedIncrease;
    public float attackArcBonus;

    [Header("Block Stat Totals")]
    public float maxBlockMeterBonus;
    public float blockRechargeBonus;
    public float blockDrainReduction;
    public float blockCooldownReduction;
    public float blockBreakStunDuration;

    [Header("Player Stat Totals")]
    public float moveSpeedBonus;
    public float dashDistanceBonus;
    public float dashCooldownReduction;
    public float dashChargeProgress;
    public float maxHPBonus;
    public float invicibilityBonus;

    [Header("Parry Stat Totals")]
    public float parryWindowBonus;
    public float parryMissStunReduction;
    public float parryMissMeterCostReduction;
    public float parryReflectSpeed;
    internal float invincibilityBonus;

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

    public float AddStat(StatUpgradeType type, float amount, float cap)
    {
        float current = GetStat(type);
        float newValue = Mathf.Min(current + amount, cap);
        float actuallyAdded = newValue - current;

        SetStat(type, newValue);
        Save();

        return actuallyAdded;
    }

    public float GetStat(StatUpgradeType type)
    {
        switch (type)
        {
            case StatUpgradeType.LightAttackDamage:
                return lightAttackDamageBonus;

            case StatUpgradeType.HeavyAttackDamage:
                return heavyAttackDamageBonus;

            case StatUpgradeType.LightAttackSpeed:
                return lightAttackSpeedIncrease;

            case StatUpgradeType.HeavyAttackWindupSpeed:
                return heavyAttackWindupReduction;

            case StatUpgradeType.HeavyAttackSpeed:
                return heavyAttackSpeedIncrease;

            case StatUpgradeType.AttackArc:
                return attackArcBonus;

            case StatUpgradeType.MaxBlockMeter:
                return maxBlockMeterBonus;

            case StatUpgradeType.BlockMeterRechargeRate:
                return blockRechargeBonus;

            case StatUpgradeType.BlockDrain:
                return blockDrainReduction;

            case StatUpgradeType.BlockCooldownReduction:
                return blockCooldownReduction;

            case StatUpgradeType.BlockBreakStunDuration:
                return blockBreakStunDuration;

            case StatUpgradeType.MovementSpeed:
                return moveSpeedBonus;

            case StatUpgradeType.DashDistance:
                return dashDistanceBonus;

            case StatUpgradeType.DashCooldownReduction:
                return dashCooldownReduction;

            case StatUpgradeType.DashCharges:
                return dashChargeProgress;

            case StatUpgradeType.MaximumHP:
                return maxHPBonus;

            case StatUpgradeType.InvincibilityFrames:
                return invicibilityBonus;

            case StatUpgradeType.ParryWindowExpansion:
                return parryWindowBonus;

            case StatUpgradeType.ParryMissStunReduction:
                return parryMissStunReduction;

            case StatUpgradeType.ParryMissMeterCostReduction:
                return parryMissMeterCostReduction;

            case StatUpgradeType.ParryReflectSpeed:
                return parryReflectSpeed;

            default:
                return 0f;
        }
    }

    private void SetStat(StatUpgradeType type, float value)
    {
        switch (type)
        {
            case StatUpgradeType.LightAttackDamage:
                lightAttackDamageBonus = value;
                break;

            case StatUpgradeType.HeavyAttackDamage:
                heavyAttackDamageBonus = value;
                break;

            case StatUpgradeType.LightAttackSpeed:
                lightAttackSpeedIncrease = value;
                break;

            case StatUpgradeType.HeavyAttackWindupSpeed:
                heavyAttackWindupReduction = value;
                break;

            case StatUpgradeType.HeavyAttackSpeed:
                heavyAttackSpeedIncrease = value;
                break;

            case StatUpgradeType.AttackArc:
                attackArcBonus = value;
                break;

            case StatUpgradeType.MaxBlockMeter:
                maxBlockMeterBonus = value;
                break;

            case StatUpgradeType.BlockMeterRechargeRate:
                blockRechargeBonus = value;
                break;

            case StatUpgradeType.BlockDrain:
                blockDrainReduction = value;
                break;

            case StatUpgradeType.BlockCooldownReduction:
                blockCooldownReduction = value;
                break;

            case StatUpgradeType.BlockBreakStunDuration:
                blockBreakStunDuration = value;
                break;

            case StatUpgradeType.MovementSpeed:
                moveSpeedBonus = value;
                break;

            case StatUpgradeType.DashDistance:
                dashDistanceBonus = value;
                break;

            case StatUpgradeType.DashCooldownReduction:
                dashCooldownReduction = value;
                break;

            case StatUpgradeType.DashCharges:
                dashChargeProgress = value;
                break;

            case StatUpgradeType.MaximumHP:
                maxHPBonus = value;
                break;

            case StatUpgradeType.InvincibilityFrames:
                invicibilityBonus = value;
                break;

            case StatUpgradeType.ParryWindowExpansion:
                parryWindowBonus = value;
                break;

            case StatUpgradeType.ParryMissStunReduction:
                parryMissStunReduction = value;
                break;

            case StatUpgradeType.ParryMissMeterCostReduction:
                parryMissMeterCostReduction = value;
                break;

            case StatUpgradeType.ParryReflectSpeed:
                parryReflectSpeed = value;
                break;
        }
    }

    public void ResetUpgrades()
    {
        lightAttackDamageBonus = 0f;
        heavyAttackDamageBonus = 0f;
        lightAttackSpeedIncrease = 0f;
        heavyAttackWindupReduction = 0f;
        heavyAttackSpeedIncrease = 0f;
        attackArcBonus = 0f;
        maxBlockMeterBonus = 0f;
        blockRechargeBonus = 0f;
        blockDrainReduction = 0f;
        blockCooldownReduction = 0f;
        blockBreakStunDuration = 0f;
        moveSpeedBonus = 0f;
        dashDistanceBonus = 0f;
        dashCooldownReduction = 0f;
        dashChargeProgress = 0f;
        maxHPBonus = 0f;
        invincibilityBonus = 0f;
        parryWindowBonus = 0f;
        parryMissStunReduction = 0f;
        parryMissMeterCostReduction = 0f;
        parryReflectSpeed = 0f;

        Save();
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("lightAttackDamageBonus", lightAttackDamageBonus);
        PlayerPrefs.SetFloat("heavyAttackDamageBonus", heavyAttackDamageBonus);
        PlayerPrefs.SetFloat("lightAttackSpeedIncrease", lightAttackSpeedIncrease);
        PlayerPrefs.SetFloat("heavyAttackWindupReduction", heavyAttackWindupReduction);
        PlayerPrefs.SetFloat("heavyAttackSpeedIncrease", heavyAttackSpeedIncrease);
        PlayerPrefs.SetFloat("attackArcBonus", attackArcBonus);

        PlayerPrefs.SetFloat("maxBlockMeterBonus", maxBlockMeterBonus);
        PlayerPrefs.SetFloat("blockRechargeBonus", blockRechargeBonus);
        PlayerPrefs.SetFloat("blockDrainReduction", blockDrainReduction);
        PlayerPrefs.SetFloat("blockCooldownReduction", blockCooldownReduction);
        PlayerPrefs.SetFloat("blockBreakStunDuration", blockBreakStunDuration);

        PlayerPrefs.SetFloat("moveSpeedBonus", moveSpeedBonus);
        PlayerPrefs.SetFloat("dashDistanceBonus", dashDistanceBonus);
        PlayerPrefs.SetFloat("dashCooldownReduction", dashCooldownReduction);
        PlayerPrefs.SetFloat("dashChargeProgress", dashChargeProgress);

        PlayerPrefs.SetFloat("maxHPBonus", maxHPBonus);
        PlayerPrefs.SetFloat("invicibilityBonus", invicibilityBonus);

        PlayerPrefs.SetFloat("parryWindowBonus", parryWindowBonus);
        PlayerPrefs.SetFloat("parryMissStunReduction", parryMissStunReduction);
        PlayerPrefs.SetFloat("parryMissMeterCostReduction", parryMissMeterCostReduction);
        PlayerPrefs.SetFloat("parryReflectSpeed", parryReflectSpeed);

        PlayerPrefs.Save();
    }

    private void Load()
    {
        lightAttackDamageBonus = PlayerPrefs.GetFloat("lightAttackDamageBonus", 0f);
        heavyAttackDamageBonus = PlayerPrefs.GetFloat("heavyAttackDamageBonus", 0f);
        lightAttackSpeedIncrease = PlayerPrefs.GetFloat("lightAttackSpeedIncrease", 0f);
        heavyAttackWindupReduction = PlayerPrefs.GetFloat("heavyAttackWindupReduction", 0f);
        heavyAttackSpeedIncrease = PlayerPrefs.GetFloat("heavyAttackSpeedIncrease", 0f);
        attackArcBonus = PlayerPrefs.GetFloat("attackArcBonus", 0f);
        maxBlockMeterBonus = PlayerPrefs.GetFloat("maxBlockMeterBonus", 0f);
        blockRechargeBonus = PlayerPrefs.GetFloat("blockRechargeBonus", 0f);
        blockDrainReduction = PlayerPrefs.GetFloat("blockDrainReduction", 0f);
        blockCooldownReduction = PlayerPrefs.GetFloat("blockCooldownReduction", 0f);
        blockBreakStunDuration = PlayerPrefs.GetFloat("blockBreakStunDuration", 0f);
        moveSpeedBonus = PlayerPrefs.GetFloat("moveSpeedBonus", 0f);
        dashDistanceBonus = PlayerPrefs.GetFloat("dashDistanceBonus", 0f);
        dashCooldownReduction = PlayerPrefs.GetFloat("dashCooldownReduction", 0f);
        dashChargeProgress = PlayerPrefs.GetFloat("dashChargeProgress", 0f);
        maxHPBonus = PlayerPrefs.GetFloat("maxHPBonus", 0f);
        invicibilityBonus = PlayerPrefs.GetFloat("invicibilityBonus", 0f);
        parryWindowBonus = PlayerPrefs.GetFloat("parryWindowBonus", 0f);
        parryMissStunReduction = PlayerPrefs.GetFloat("parryMissStunReduction", 0f);
        parryMissMeterCostReduction = PlayerPrefs.GetFloat("parryMissMeterCostReduction", 0f);
        parryReflectSpeed = PlayerPrefs.GetFloat("parryReflectSpeed", 0f);
    }
}
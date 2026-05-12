using UnityEngine;

public enum CardRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

public enum StatUpgradeType
{
    LightAttackDamage,
    HeavyAttackDamage,
    LightAttackSpeed,
    HeavyAttackWindupSpeed,
    HeavyAttackSpeed,
    AttackArc,

    MaxBlockMeter,
    BlockMeterRechargeRate,
    BlockDrain,
    BlockCooldownReduction,
    BlockBreakStunDuration,

    MovementSpeed,
    DashDistance,
    DashCooldownReduction,
    DashCharges,

    MaximumHP,
    InvincibilityFrames,

    ParryWindowExpansion,
    ParryMissStunReduction,
    ParryMissMeterCostReduction,
    ParryReflectSpeed
}

public enum AbilityUpgradeType
{
    // Rare
    TerminationProtocol,
    GrazeCapacitor,
    FailurePointPrioritizer,
    WeakSignalAmplifier,
    ArmorChecksum,
    CombatCacheFlush,
    ImpactMemoryBuffer,
    RetaliationSubroutine,
    EvasiveRecalculation,
    MomentumCache,
    CollisionOverride,
    RecursiveStrikeLogic,
    OverheatVent,
    KineticBrake,
    ReactiveArmorPlating,
    ShieldedDiagnostics,
    ContactDebugger,
    UnstableMotorTiming,

    // Epic
    SelfMaintenanceFunction,
    EnhancedInterrogationTechniques,
    BadSectorSpread,
    RedlineGovernor,
    CapacitorBleed,
    ScrapFeedbackLoop,
    DashShockwave,
    ParryCapacitor,
    ImpactMemory,
    ThermalRunaway,
    GrazeMomentumDriver,
    ProjectileReformatting,
    Counterpulse,
    ParryRebootShortcut,

    // Legendary
    EmergencyRepairs,
    PhaseShiftMatrix,
    WeaponizedTelemetry,
    ExponentialThreatModel,
    CriticalSystemAuthority,
    PerfectCounterchain,
    SyntheticSurvivalInstinct
}

public abstract class CardEffect : ScriptableObject
{
    public string cardName;

    public CardRarity cardRarity;

    [TextArea]
    public string description;

    public abstract void Apply(GameObject player, CardRarity rarity);
}

public abstract class StatCardEffect : CardEffect
{
    protected abstract StatUpgradeType StatType { get; }

    protected abstract float CommonValue { get; }
    protected abstract float RareValue { get; }
    protected abstract float EpicValue { get; }
    protected abstract float LegendaryValue { get; }

    protected abstract float StatCap { get; }

    public override void Apply(GameObject player, CardRarity rarity)
    {
        if (player == null)
        {
            Debug.LogWarning("[StatCardEffect] Player is null. Cannot apply " + name);
            return;
        }

        if (UpgradeState.Instance == null)
        {
            Debug.LogWarning("[StatCardEffect] No UpgradeState found.");
            return;
        }

        float amount = GetAmountByRarity(rarity);
        float actuallyAdded = UpgradeState.Instance.AddStat(StatType, amount, StatCap);

        if (actuallyAdded <= 0f)
        {
            Debug.Log("[StatCardEffect] " + name + " is already maxed. No stat added.");
            return;
        }

        ApplyToRuntime(player, actuallyAdded);

        Debug.Log("[StatCardEffect] Applied " + name + " as " + rarity + ". Added: " + actuallyAdded);
    }

    private float GetAmountByRarity(CardRarity rarity)
    {
        switch (rarity)
        {
            case CardRarity.Common:
                return CommonValue;

            case CardRarity.Rare:
                return RareValue;

            case CardRarity.Epic:
                return EpicValue;

            case CardRarity.Legendary:
                return LegendaryValue;

            default:
                return CommonValue;
        }
    }

    protected abstract void ApplyToRuntime(GameObject player, float amount);
}

public abstract class AbilityCardEffect : CardEffect
{
    protected abstract AbilityUpgradeType AbilityType { get; }

    public override void Apply(GameObject player, CardRarity rarity)
    {
        if (CardAbilityState.Instance == null)
        {
            Debug.LogWarning("[AbilityCardEffect] No CardAbilityState found.");
            return;
        }

        int added = CardAbilityState.Instance.AddStack(AbilityType);

        if (added <= 0)
        {
            Debug.Log("[AbilityCardEffect] " + name + " is already maxed. No stack added.");
            return;
        }

        Debug.Log("[AbilityCardEffect] Added stack to " + name);
    }
}

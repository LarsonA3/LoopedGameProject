using UnityEngine;

[CreateAssetMenu(fileName = "Parry Error Dampener", menuName = "Cards/Stats/Parry Error Dampener")]
public class ParryErrorDampenerCard : StatCardEffect
{
    protected override StatUpgradeType StatType => StatUpgradeType.ParryMissMeterCostReduction;

    protected override float CommonValue => 0.075f;
    protected override float RareValue => 0.150f;
    protected override float EpicValue => 0.225f;
    protected override float LegendaryValue => 0.375f;

    protected override float StatCap => 1.25f;

    private void OnEnable()
    {
        cardName = "Parry Error Dampener";
        description = "Reduces block meter lost when a parry misses.";
    }

    protected override void ApplyToRuntime(GameObject player, float amount)
    {
        Weapon weapon = player.GetComponentInChildren<Weapon>();

        if (weapon != null)
        {
            weapon.ReduceParryMissBlockMeterCost(amount);
        }
    }
}

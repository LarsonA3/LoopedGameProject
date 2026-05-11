using UnityEngine;

[CreateAssetMenu(fileName = "Reinforced Guard Cell", menuName = "Cards/Stats/Reinforced Guard Cell")]
public class ReinforcedGuardCellCard : StatCardEffect
{
    protected override StatUpgradeType StatType => StatUpgradeType.MaxBlockMeter;

    protected override float CommonValue => 0.25f;
    protected override float RareValue => 0.50f;
    protected override float EpicValue => 0.75f;
    protected override float LegendaryValue => 1.25f;

    protected override float StatCap => 10f;

    private void OnEnable()
    {
        cardName = "Reinforced Guard Cell";
        description = "Increases max block meter.";
    }

    protected override void ApplyToRuntime(GameObject player, float amount)
    {
        Weapon weapon = player.GetComponentInChildren<Weapon>();

        if (weapon != null)
        {
            weapon.IncreaseBlockMeterMax(amount);
        }
    }
}

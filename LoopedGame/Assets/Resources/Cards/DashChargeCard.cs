using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Stat/DashChargeCard")]
public class DashChargeCard : StatCardEffect
{
    protected override StatUpgradeType StatType => StatUpgradeType.DashCharges;

    protected override float CommonValue => 1f;
    protected override float RareValue => 1f;
    protected override float EpicValue => 2f;
    protected override float LegendaryValue => 2f;

    protected override float StatCap => 9f;

    protected override void ApplyToRuntime(GameObject player, float amount)
    {
        TopDownController controller = player.GetComponent<TopDownController>();
        if (controller != null)
            controller.addDashCharge(amount);
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "Coolant Recharge Loop", menuName = "Cards/Stats/Coolant Recharge Loop")]
public class CoolantRechargeLoopCard : StatCardEffect
{
    protected override StatUpgradeType StatType => StatUpgradeType.BlockMeterRechargeRate;

    protected override float CommonValue => 0.10f;
    protected override float RareValue => 0.20f;
    protected override float EpicValue => 0.30f;
    protected override float LegendaryValue => 0.50f;

    protected override float StatCap => 5f;

    private void OnEnable()
    {
        cardName = "Coolant Recharge Loop";
        description = "Increases block meter recharge rate.";
    }

    protected override void ApplyToRuntime(GameObject player, float amount)
    {
        Weapon weapon = player.GetComponentInChildren<Weapon>();

        if (weapon != null)
        {
            weapon.IncreaseBlockRecharge(amount);
        }
    }
}


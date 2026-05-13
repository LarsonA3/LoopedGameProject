using UnityEngine;

[CreateAssetMenu(fileName = "Hydraulic Crusher", menuName = "Cards/Stats/Hydraulic Crusher")]
public class HydraulicCrusherCard : StatCardEffect
{
    protected override StatUpgradeType StatType => StatUpgradeType.HeavyAttackDamage;

    protected override float CommonValue => 0.50f;
    protected override float RareValue => 1.00f;
    protected override float EpicValue => 1.50f;
    protected override float LegendaryValue => 2.50f;

    protected override float StatCap => 20f;

    private void OnEnable()
    {
        cardName = "Hydraulic Crusher";
        description = "Increases heavy attack damage.";
    }

    protected override void ApplyToRuntime(GameObject player, float amount)
    {
        Weapon weapon = player.GetComponentInChildren<Weapon>();

        if (weapon != null)
        {
            weapon.AddHeavyDamage(amount);
        }
    }
}

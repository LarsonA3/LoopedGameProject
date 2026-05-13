using UnityEngine;

[CreateAssetMenu(fileName = "Heavy Swing Accelerator", menuName = "Cards/Stats/Heavy Swing Accelerator")]
public class HeavySwingAcceleratorCard : StatCardEffect
{
    protected override StatUpgradeType StatType => StatUpgradeType.HeavyAttackSpeed;

    protected override float CommonValue => 0.003f;
    protected override float RareValue => 0.006f;
    protected override float EpicValue => 0.009f;
    protected override float LegendaryValue => 0.015f;

    protected override float StatCap => 0.060f;

    private void OnEnable()
    {
        cardName = "Heavy Swing Accelerator";
        description = "Reduces heavy swing duration.";
    }

    protected override void ApplyToRuntime(GameObject player, float amount)
    {
        Weapon weapon = player.GetComponentInChildren<Weapon>();

        if (weapon != null)
        {
            weapon.ReduceHeavySwingDuration(amount);
        }
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "Fast Guard Reboot", menuName = "Cards/Stats/Fast Guard Reboot")]
public class FastGuardRebootCard : StatCardEffect
{
    protected override StatUpgradeType StatType => StatUpgradeType.BlockCooldownReduction;

    protected override float CommonValue => 0.025f;
    protected override float RareValue => 0.050f;
    protected override float EpicValue => 0.075f;
    protected override float LegendaryValue => 0.125f;

    protected override float StatCap => 0.45f;

    private void OnEnable()
    {
        cardName = "Fast Guard Reboot";
        description = "Reduces block cooldown.";
    }

    protected override void ApplyToRuntime(GameObject player, float amount)
    {
        Weapon weapon = player.GetComponentInChildren<Weapon>();

        if (weapon != null)
        {
            weapon.ReduceBlockCooldown(amount);
        }
    }
}

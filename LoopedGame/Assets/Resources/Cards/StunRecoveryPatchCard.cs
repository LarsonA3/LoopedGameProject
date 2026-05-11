using UnityEngine;

[CreateAssetMenu(fileName = "Stun Recovery Patch", menuName = "Cards/Stats/Stun Recovery Patch")]
public class StunRecoveryPatchCard : StatCardEffect
{
    protected override StatUpgradeType StatType => StatUpgradeType.BlockBreakStunDuration;

    protected override float CommonValue => 0.05f;
    protected override float RareValue => 0.10f;
    protected override float EpicValue => 0.15f;
    protected override float LegendaryValue => 0.25f;

    protected override float StatCap => 0.90f;

    private void OnEnable()
    {
        cardName = "Stun Recovery Patch";
        description = "Reduces block break stun duration.";
    }

    protected override void ApplyToRuntime(GameObject player, float amount)
    {
        Weapon weapon = player.GetComponentInChildren<Weapon>();

        if (weapon != null)
        {
            weapon.ReduceBlockBreakStun(amount);
        }
    }
}

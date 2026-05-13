using UnityEngine;

[CreateAssetMenu(fileName = "Failed Input Recovery", menuName = "Cards/Stats/Failed Input Recovery")]
public class FailedInputRecoveryCard : StatCardEffect
{
    protected override StatUpgradeType StatType => StatUpgradeType.ParryMissStunReduction;

    protected override float CommonValue => 0.05f;
    protected override float RareValue => 0.10f;
    protected override float EpicValue => 0.15f;
    protected override float LegendaryValue => 0.25f;

    protected override float StatCap => 1.00f;

    private void OnEnable()
    {
        cardName = "Failed Input Recovery";
        description = "Reduces stun duration after a missed parry.";
    }

    protected override void ApplyToRuntime(GameObject player, float amount)
    {
        Weapon weapon = player.GetComponentInChildren<Weapon>();

        if (weapon != null)
        {
            weapon.ReduceParryMissStun(amount);
        }
    }
}

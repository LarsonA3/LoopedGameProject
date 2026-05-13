using UnityEngine;

[CreateAssetMenu(fileName = "Preloaded Hammer Servo", menuName = "Cards/Stats/Preloaded Hammer Servo")]
public class PreloadedHammerServoCard : StatCardEffect
{
    protected override StatUpgradeType StatType => StatUpgradeType.HeavyAttackWindupSpeed;

    protected override float CommonValue => 0.025f;
    protected override float RareValue => 0.050f;
    protected override float EpicValue => 0.075f;
    protected override float LegendaryValue => 0.125f;

    protected override float StatCap => 0.40f;

    private void OnEnable()
    {
        cardName = "Preloaded Hammer Servo";
        description = "Reduces heavy attack windup time.";
    }

    protected override void ApplyToRuntime(GameObject player, float amount)
    {
        Weapon weapon = player.GetComponentInChildren<Weapon>();

        if (weapon != null)
        {
            weapon.ReduceHeavyWindup(amount);
        }
    }
}

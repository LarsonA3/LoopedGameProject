using UnityEngine;

[CreateAssetMenu(fileName = "Wide-Sweep Calibration", menuName = "Cards/Stats/Wide-Sweep Calibration")]
public class WideSweepCalibrationCard : StatCardEffect
{
    protected override StatUpgradeType StatType => StatUpgradeType.AttackArc;

    protected override float CommonValue => 3f;
    protected override float RareValue => 6f;
    protected override float EpicValue => 9f;
    protected override float LegendaryValue => 15f;

    protected override float StatCap => 75f;

    private void OnEnable()
    {
        cardName = "Wide-Sweep Calibration";
        description = "Increases attack arc.";
    }

    protected override void ApplyToRuntime(GameObject player, float amount)
    {
        Weapon weapon = player.GetComponentInChildren<Weapon>();

        if (weapon != null)
        {
            weapon.IncreaseAttackArc(amount);
        }
    }
}

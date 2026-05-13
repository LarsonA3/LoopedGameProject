using UnityEngine;

[CreateAssetMenu(fileName = "Timing Calibration Module", menuName = "Cards/Stats/Timing Calibration Module")]
public class TimingCalibrationModuleCard : StatCardEffect
{
    protected override StatUpgradeType StatType => StatUpgradeType.ParryWindowExpansion;

    protected override float CommonValue => 0.015f;
    protected override float RareValue => 0.030f;
    protected override float EpicValue => 0.045f;
    protected override float LegendaryValue => 0.075f;

    protected override float StatCap => 0.30f;

    private void OnEnable()
    {
        cardName = "Timing Calibration Module";
        description = "Increases parry window duration.";
    }

    protected override void ApplyToRuntime(GameObject player, float amount)
    {
        Weapon weapon = player.GetComponentInChildren<Weapon>();

        if (weapon != null)
        {
            weapon.IncreaseParryWindow(amount);
        }
    }
}

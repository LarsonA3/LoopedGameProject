using UnityEngine;

[CreateAssetMenu(fileName = "Accelerated Swing Actuator", menuName = "Cards/Stats/Accelerated Swing Actuator")]
public class AcceleratedSwingActuatorCard : StatCardEffect
{
    protected override StatUpgradeType StatType => StatUpgradeType.LightAttackSpeed;

    protected override float CommonValue => 0.010f;
    protected override float RareValue => 0.020f;
    protected override float EpicValue => 0.030f;
    protected override float LegendaryValue => 0.050f;

    protected override float StatCap => 0.16f;

    private void OnEnable()
    {
        cardName = "Accelerated Swing Actuator";
        description = "Reduces light attack swing duration.";
    }

    protected override void ApplyToRuntime(GameObject player, float amount)
    {
        Weapon weapon = player.GetComponentInChildren<Weapon>();

        if (weapon != null)
        {
            weapon.ReduceLightSwingDuration(amount);
        }
    }
}

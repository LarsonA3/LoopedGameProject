using UnityEngine;

[CreateAssetMenu(fileName = "Emergency Mobility Driver", menuName = "Cards/Stats/Emergency Mobility Driver")]
public class EmergencyMobilityDriverCard : StatCardEffect
{
    protected override StatUpgradeType StatType => StatUpgradeType.MovementSpeed;

    protected override float CommonValue => 0.15f;
    protected override float RareValue => 0.30f;
    protected override float EpicValue => 0.45f;
    protected override float LegendaryValue => 0.75f;

    protected override float StatCap => 5f;

    private void OnEnable()
    {
        cardName = "Emergency Mobility Driver";
        description = "Increases movement speed.";
    }

    protected override void ApplyToRuntime(GameObject player, float amount)
    {
        TopDownController controller = player.GetComponent<TopDownController>();

        if (controller == null)
        {
            controller = player.GetComponentInChildren<TopDownController>();
        }

        if (controller != null)
        {
            controller.AddMoveSpeed(amount);
        }
    }
}

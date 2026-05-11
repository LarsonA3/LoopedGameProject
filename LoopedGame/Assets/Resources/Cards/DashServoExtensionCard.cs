using UnityEngine;

[CreateAssetMenu(fileName = "Dash Servo Extension", menuName = "Cards/Stats/Dash Servo Extension")]
public class DashServoExtensionCard : StatCardEffect
{
    protected override StatUpgradeType StatType => StatUpgradeType.DashDistance;

    protected override float CommonValue => 0.10f;
    protected override float RareValue => 0.20f;
    protected override float EpicValue => 0.30f;
    protected override float LegendaryValue => 0.50f;

    protected override float StatCap => 3f;

    private void OnEnable()
    {
        cardName = "Dash Servo Extension";
        description = "Increases dash distance.";
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
            controller.IncreaseDashDistance(amount);
        }
    }
}

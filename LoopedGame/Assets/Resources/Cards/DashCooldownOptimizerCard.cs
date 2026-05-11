using UnityEngine;

[CreateAssetMenu(fileName = "Dash Cooldown Optimizer", menuName = "Cards/Stats/Dash Cooldown Optimizer")]
public class DashCooldownOptimizerCard : StatCardEffect
{
    protected override StatUpgradeType StatType => StatUpgradeType.DashCooldownReduction;

    protected override float CommonValue => 0.04f;
    protected override float RareValue => 0.08f;
    protected override float EpicValue => 0.12f;
    protected override float LegendaryValue => 0.20f;

    protected override float StatCap => 0.80f;

    private void OnEnable()
    {
        cardName = "Dash Cooldown Optimizer";
        description = "Reduces dash cooldown.";
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
            controller.ReduceDashCooldown(amount);
        }
    }
}

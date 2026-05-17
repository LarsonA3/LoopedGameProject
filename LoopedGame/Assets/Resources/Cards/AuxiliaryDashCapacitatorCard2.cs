using UnityEngine;

[CreateAssetMenu(fileName = "Auxiliary Dash Capacitor", menuName = "Cards/Stats/Auxiliary Dash Capacitor")]
public class AuxiliaryDashCapacitorCard2 : StatCardEffect
{
    protected override StatUpgradeType StatType => StatUpgradeType.DashCharges;

    protected override float CommonValue => 1f;
    protected override float RareValue => 2f;
    protected override float EpicValue => 3f;
    protected override float LegendaryValue => 5f;

    protected override float StatCap => 25f;

    private void OnEnable()
    {
        cardName = "Auxiliary Dash Capacitor";
        description = "Builds progress toward extra dash charges. Every 5 progress grants 1 extra dash charge.";
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
            controller.AddDashChargeProgress(Mathf.RoundToInt(amount));
        }
    }
}

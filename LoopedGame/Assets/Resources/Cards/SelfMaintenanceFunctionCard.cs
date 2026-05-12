using UnityEngine;

[CreateAssetMenu(fileName = "Self-Maintenance Function", menuName = "Cards/Epic/Self-Maintenance Function")]
public class SelfMaintenanceFunctionCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.SelfMaintenanceFunction;

    private void OnEnable()
    {
        cardName = "Self-Maintenance Function";
        description =
            "When your HP drops below half its maximum, regenerate 1 HP per second until your HP reaches half its maximum.";
    }
}

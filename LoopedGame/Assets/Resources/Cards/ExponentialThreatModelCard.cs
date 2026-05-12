using UnityEngine;

[CreateAssetMenu(fileName = "Exponential Threat Model", menuName = "Cards/Legendary/Exponential Threat Model")]
public class ExponentialThreatModelCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.ExponentialThreatModel;

    private void OnEnable()
    {
        cardName = "Exponential Threat Model";
        description =
            "Every enemy killed in a room increases your damage for the rest of that room.";
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "Emergency Repairs", menuName = "Cards/Legendary/Emergency Repairs")]
public class EmergencyRepairsCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.EmergencyRepairs;

    private void OnEnable()
    {
        cardName = "Emergency Repairs";
        cardRarity = CardRarity.Legendary;
        description =
            "When your HP drops to 0, survive instead and recover to 10 HP. This can occur only once per run.";
    }
}

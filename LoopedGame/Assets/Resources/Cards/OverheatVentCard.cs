using UnityEngine;

[CreateAssetMenu(fileName = "Overheat Vent", menuName = "Cards/Rare/Overheat Vent")]
public class OverheatVentCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.OverheatVent;

    private void OnEnable()
    {
        cardName = "Overheat Vent";
        cardRarity = CardRarity.Rare;
        description = "Every 5 weapon hits releases a heat burst around you.";
    }
}

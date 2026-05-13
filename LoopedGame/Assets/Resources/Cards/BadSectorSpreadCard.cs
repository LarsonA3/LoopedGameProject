using UnityEngine;

[CreateAssetMenu(fileName = "Bad Sector Spread", menuName = "Cards/Epic/Bad Sector Spread")]
public class BadSectorSpreadCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.BadSectorSpread;

    private void OnEnable()
    {
        cardName = "Bad Sector Spread";
        cardRarity = CardRarity.Epic;
        description =
            "When an enemy dies, nearby enemies take damage based on the dead enemy's maximum HP. This effect cannot replicate itself.";
    }
}

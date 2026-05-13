using UnityEngine;

[CreateAssetMenu(fileName = "Graze Momentum Driver", menuName = "Cards/Epic/Graze Momentum Driver")]
public class GrazeMomentumDriverCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.GrazeMomentumDriver;

    private void OnEnable()
    {
        cardName = "Graze Momentum Driver";
        cardRarity = CardRarity.Epic;
        description =
            "Each graze increases movement speed for 3 seconds, stacking up to 5 times.";
    }
}

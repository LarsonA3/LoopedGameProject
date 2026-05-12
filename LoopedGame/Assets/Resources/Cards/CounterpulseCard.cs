using UnityEngine;

[CreateAssetMenu(fileName = "Counterpulse", menuName = "Cards/Epic/Counterpulse")]
public class CounterpulseCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.Counterpulse;

    private void OnEnable()
    {
        cardName = "Counterpulse";
        cardRarity = CardRarity.Epic;
        description =
            "Successful parries emit a damaging pulse around you.";
    }
}

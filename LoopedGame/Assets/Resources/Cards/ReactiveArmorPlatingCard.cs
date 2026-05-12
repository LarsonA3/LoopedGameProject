using UnityEngine;

[CreateAssetMenu(fileName = "Reactive Armor Plating", menuName = "Cards/Rare/Reactive Armor Plating")]
public class ReactiveArmorPlatingCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.ReactiveArmorPlating;

    private void OnEnable()
    {
        cardName = "Reactive Armor Plating";
        description = "After taking damage, the next hit you take is reduced.";
    }
}

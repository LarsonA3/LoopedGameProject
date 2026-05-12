using UnityEngine;

[CreateAssetMenu(fileName = "Thermal Runaway", menuName = "Cards/Epic/Thermal Runaway")]
public class ThermalRunawayCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.ThermalRunaway;

    private void OnEnable()
    {
        cardName = "Thermal Runaway";
        cardRarity = CardRarity.Epic;
        description =
            "Consecutive weapon hits without taking damage increase attack damage. Taking damage resets stacks.";
    }
}

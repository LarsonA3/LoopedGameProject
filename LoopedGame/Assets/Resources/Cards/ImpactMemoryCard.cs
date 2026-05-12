using UnityEngine;

[CreateAssetMenu(fileName = "Impact Memory", menuName = "Cards/Epic/Impact Memory")]
public class ImpactMemoryCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.ImpactMemory;

    private void OnEnable()
    {
        cardName = "Impact Memory";
        cardRarity = CardRarity.Epic;
        description =
            "Each blocked hit increases your next heavy attack damage.";
    }
}

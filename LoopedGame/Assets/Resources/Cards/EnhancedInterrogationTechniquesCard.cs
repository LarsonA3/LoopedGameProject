using UnityEngine;

[CreateAssetMenu(fileName = "Enhanced Interrogation Techniques", menuName = "Cards/Epic/Enhanced Interrogation Techniques")]
public class EnhancedInterrogationTechniquesCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.EnhancedInterrogationTechniques;

    private void OnEnable()
    {
        cardName = "Enhanced Interrogation Techniques";
        cardRarity = CardRarity.Epic;
        description =
            "Weapon hits apply a damage-over-time effect that stacks up to 5 times.";
    }
}

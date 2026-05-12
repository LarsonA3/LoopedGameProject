using UnityEngine;

[CreateAssetMenu(fileName = "Kinetic Brake", menuName = "Cards/Rare/Kinetic Brake")]
public class KineticBrakeCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.KineticBrake;

    private void OnEnable()
    {
        cardName = "Kinetic Brake";
        cardRarity = CardRarity.Rare;
        description = "While blocking, nearby enemies are slowed.";
    }
}

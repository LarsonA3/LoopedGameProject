using UnityEngine;

[CreateAssetMenu(fileName = "Failure Point Prioritizer", menuName = "Cards/Rare/Failure Point Prioritizer")]
public class FailurePointPrioritizerCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.FailurePointPrioritizer;

    private void OnEnable()
    {
        cardName = "Failure Point Prioritizer";
        cardRarity = CardRarity.Rare;
        description = "Weapon attacks have a chance to stun non-boss enemies.";
    }
}

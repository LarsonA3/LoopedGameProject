using UnityEngine;

[CreateAssetMenu(fileName = "Termination Protocol", menuName = "Cards/Rare/Termination Protocol")]
public class TerminationProtocolCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.TerminationProtocol;

    private void OnEnable()
    {
        cardName = "Termination Protocol";
        cardRarity = CardRarity.Rare;
        description = "Killing 2 enemies within 5 seconds boosts weapon damage by 20% for 5 seconds. Killing another enemy during the buff refreshes the duration.";
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "Scrap Feedback Loop", menuName = "Cards/Epic/Scrap Feedback Loop")]
public class ScrapFeedbackLoopCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.ScrapFeedbackLoop;

    private void OnEnable()
    {
        cardName = "Scrap Feedback Loop";
        cardRarity = CardRarity.Epic;
        description =
            "Killing an enemy heals you.";
    }
}

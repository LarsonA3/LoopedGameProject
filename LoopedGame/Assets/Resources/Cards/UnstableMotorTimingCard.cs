using UnityEngine;

[CreateAssetMenu(fileName = "Unstable Motor Timing", menuName = "Cards/Rare/Unstable Motor Timing")]
public class UnstableMotorTimingCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.UnstableMotorTiming;

    private void OnEnable()
    {
        cardName = "Unstable Motor Timing";
        cardRarity = CardRarity.Rare;
        description = "Light attacks have a chance to instantly make the next light attack faster.";
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "Weak Signal Amplifier", menuName = "Cards/Rare/Weak Signal Amplifier")]
public class WeakSignalAmplifierCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.WeakSignalAmplifier;

    private void OnEnable()
    {
        cardName = "Weak Signal Amplifier";
        description = "Enemies below 25% HP take increased weapon damage.";
    }
}

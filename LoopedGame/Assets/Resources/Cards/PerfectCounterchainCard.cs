using UnityEngine;

[CreateAssetMenu(fileName = "Perfect Counterchain", menuName = "Cards/Legendary/Perfect Counterchain")]
public class PerfectCounterchainCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.PerfectCounterchain;

    private void OnEnable()
    {
        cardName = "Perfect Counterchain";
        description =
            "A successful parry empowers your next weapon attack within 2 seconds, causing bonus damage and a damage pulse around the target.";
    }
}

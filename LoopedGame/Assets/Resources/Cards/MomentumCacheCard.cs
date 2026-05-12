using UnityEngine;

[CreateAssetMenu(fileName = "Momentum Cache", menuName = "Cards/Rare/Momentum Cache")]
public class MomentumCacheCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.MomentumCache;

    private void OnEnable()
    {
        cardName = "Momentum Cache";
        cardRarity = CardRarity.Rare;
        description = "If you attack within 1 second after dashing, that attack deals bonus damage.";
    }
}



using UnityEngine;

[CreateAssetMenu(fileName = "Combat Cache Flush", menuName = "Cards/Rare/Combat Cache Flush")]
public class CombatCacheFlushCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.CombatCacheFlush;

    private void OnEnable()
    {
        cardName = "Combat Cache Flush";
        description = "Clearing a room restores HP.";
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "Collision Override", menuName = "Cards/Rare/Collision Override")]
public class CollisionOverrideCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.CollisionOverride;

    private void OnEnable()
    {
        cardName = "Collision Override";
        description = "Dashing into an enemy deals damage and lightly knocks them back.";
    }
}

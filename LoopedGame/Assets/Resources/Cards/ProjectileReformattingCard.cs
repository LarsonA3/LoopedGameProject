using UnityEngine;

[CreateAssetMenu(fileName = "Projectile Reformatting", menuName = "Cards/Epic/Projectile Reformatting")]
public class ProjectileReformattingCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.ProjectileReformatting;

    private void OnEnable()
    {
        cardName = "Projectile Reformatting";
        cardRarity = CardRarity.Epic;
        description =
            "Parried projectiles target the nearest enemy instead of only reflecting straight forward.";
    }
}

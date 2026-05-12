using UnityEngine;

[CreateAssetMenu(fileName = "Capacitor Bleed", menuName = "Cards/Epic/Capacitor Bleed")]
public class CapacitorBleedCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.CapacitorBleed;

    private void OnEnable()
    {
        cardName = "Capacitor Bleed";
        description =
            "When you take damage, release an electrical pulse that damages nearby enemies.";
    }
}

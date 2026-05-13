using UnityEngine;

[CreateAssetMenu(fileName = "Graze Capacitor", menuName = "Cards/Rare/Graze Capacitor")]
public class GrazeCapacitorCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.GrazeCapacitor;

    private void OnEnable()
    {
        cardName = "Graze Capacitor";
        cardRarity = CardRarity.Rare;
        description = "Grazing a projectile stores 1 charge. At 5 charges, the charges are consumed and your next light attack deals +5% bonus damage.";
    }
}

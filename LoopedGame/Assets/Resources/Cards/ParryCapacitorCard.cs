using UnityEngine;

[CreateAssetMenu(fileName = "Parry Capacitor", menuName = "Cards/Epic/Parry Capacitor")]
public class ParryCapacitorCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.ParryCapacitor;

    private void OnEnable()
    {
        cardName = "Parry Capacitor";
        description =
            "Successful parries restore block meter.";
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "Evasive Recalculation", menuName = "Cards/Rare/Evasive Recalculation")]
public class EvasiveRecalculationCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.EvasiveRecalculation;

    private void OnEnable()
    {
        cardName = "Evasive Recalculation";
        description = "After dashing, gain temporary damage reduction.";
    }
}


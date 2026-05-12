using UnityEngine;

[CreateAssetMenu(fileName = "Synthetic Survival Instinct", menuName = "Cards/Legendary/Synthetic Survival Instinct")]
public class SyntheticSurvivalInstinctCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.SyntheticSurvivalInstinct;

    private void OnEnable()
    {
        cardName = "Synthetic Survival Instinct";
        description =
            "When you would take lethal damage, automatically consume block meter to reduce the damage.";
    }
}

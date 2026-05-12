using UnityEngine;

[CreateAssetMenu(fileName = "Redline Governor", menuName = "Cards/Epic/Redline Governor")]
public class RedlineGovernorCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.RedlineGovernor;

    private void OnEnable()
    {
        cardName = "Redline Governor";
        description =
            "While below 25% HP, weapon damage increases.";
    }
}

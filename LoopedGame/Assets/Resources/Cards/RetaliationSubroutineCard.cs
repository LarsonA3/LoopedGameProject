using UnityEngine;

[CreateAssetMenu(fileName = "Retaliation Subroutine", menuName = "Cards/Rare/Retaliation Subroutine")]
public class RetaliationSubroutineCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.RetaliationSubroutine;

    private void OnEnable()
    {
        cardName = "Retaliation Subroutine";
        description = "After taking damage, your next weapon attack within 3 seconds deals bonus damage.";
    }
}

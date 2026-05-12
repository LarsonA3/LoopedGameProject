using UnityEngine;

[CreateAssetMenu(fileName = "Recursive Strike Logic", menuName = "Cards/Rare/Recursive Strike Logic")]
public class RecursiveStrikeLogicCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.RecursiveStrikeLogic;

    private void OnEnable()
    {
        cardName = "Recursive Strike Logic";
        description = "Hitting the same enemy 3 times in a row makes the third hit deal bonus damage.";
    }
}

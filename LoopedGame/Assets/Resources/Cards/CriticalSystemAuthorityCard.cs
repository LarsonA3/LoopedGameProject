using UnityEngine;

[CreateAssetMenu(fileName = "Critical System Authority", menuName = "Cards/Legendary/Critical System Authority")]
public class CriticalSystemAuthorityCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.CriticalSystemAuthority;

    private void OnEnable()
    {
        cardName = "Critical System Authority";
        description =
            "While below 25% HP, all damage you deal is increased and all damage you take is reduced.";
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "Contact Debugger", menuName = "Cards/Rare/Contact Debugger")]
public class ContactDebuggerCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.ContactDebugger;

    private void OnEnable()
    {
        cardName = "Contact Debugger";
        description = "Light attacks mark enemies. Heavy attacks against marked enemies deal bonus damage.";
    }
}

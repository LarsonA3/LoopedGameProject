using UnityEngine;

[CreateAssetMenu(fileName = "Parry Reboot Shortcut", menuName = "Cards/Epic/Parry Reboot Shortcut")]
public class ParryRebootShortcutCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.ParryRebootShortcut;

    private void OnEnable()
    {
        cardName = "Parry Reboot Shortcut";
        cardRarity = CardRarity.Epic;
        description =
            "A successful parry instantly clears block cooldown and restores block meter.";
    }
}

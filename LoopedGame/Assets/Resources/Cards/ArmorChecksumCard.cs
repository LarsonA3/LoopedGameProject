using UnityEngine;

[CreateAssetMenu(fileName = "Armor Checksum", menuName = "Cards/Rare/Armor Checksum")]
public class ArmorChecksumCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.ArmorChecksum;

    private void OnEnable()
    {
        cardName = "Armor Checksum";
        description = "If you have not taken damage for 10 seconds, your next hit taken is reduced.";
    }
}

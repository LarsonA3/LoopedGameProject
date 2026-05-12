using UnityEngine;

[CreateAssetMenu(fileName = "Shielded Diagnostics", menuName = "Cards/Rare/Shielded Diagnostics")]
public class ShieldedDiagnosticsCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.ShieldedDiagnostics;

    private void OnEnable()
    {
        cardName = "Shielded Diagnostics";
        cardRarity = CardRarity.Rare;
        description = "While blocking, slowly repair yourself if you have not taken damage recently.";
    }
}

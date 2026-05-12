using UnityEngine;

[CreateAssetMenu(fileName = "Weaponized Telemetry", menuName = "Cards/Legendary/Weaponized Telemetry")]
public class WeaponizedTelemetryCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.WeaponizedTelemetry;

    private void OnEnable()
    {
        cardName = "Weaponized Telemetry";
        description =
            "The game tracks your most-used combat action in a room. On room clear, that action gets empowered for the next room.";
    }
}

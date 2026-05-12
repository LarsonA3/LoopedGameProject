using UnityEngine;

[CreateAssetMenu(fileName = "Phase Shift Matrix", menuName = "Cards/Legendary/Phase Shift Matrix")]
public class PhaseShiftMatrixCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.PhaseShiftMatrix;

    private void OnEnable()
    {
        cardName = "Phase Shift Matrix";
        description =
            "When your HP drops below 10%, become intangible for 1 second, rendering you immune to attacks.";
    }
}

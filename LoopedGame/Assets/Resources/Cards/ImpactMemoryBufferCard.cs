using UnityEngine;

[CreateAssetMenu(fileName = "Impact Memory Buffer", menuName = "Cards/Rare/Impact Memory Buffer")]
public class ImpactMemoryBufferCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.ImpactMemoryBuffer;

    private void OnEnable()
    {
        cardName = "Impact Memory Buffer";
        description = "Taking damage stores part of that damage as bonus damage for your next weapon attack.";
    }
}

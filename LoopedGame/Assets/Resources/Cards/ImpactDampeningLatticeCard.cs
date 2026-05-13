using UnityEngine;

[CreateAssetMenu(fileName = "Impact Dampening Lattice", menuName = "Cards/Stats/Impact Dampening Lattice")]
public class ImpactDampeningLatticeCard : StatCardEffect
{
    protected override StatUpgradeType StatType => StatUpgradeType.BlockDrain;

    protected override float CommonValue => 1f;
    protected override float RareValue => 2f;
    protected override float EpicValue => 3f;
    protected override float LegendaryValue => 5f;

    protected override float StatCap => 20f;

    private void OnEnable()
    {
        cardName = "Impact Dampening Lattice";
        description = "Reduces block meter drain.";
    }

    protected override void ApplyToRuntime(GameObject player, float amount)
    {
        Weapon weapon = player.GetComponentInChildren<Weapon>();

        if (weapon != null)
        {
            float passiveDrainReduction = 0.015f * amount;
            float impactDrainReduction = 0.003f * amount;

            weapon.ReduceBlockDrain(passiveDrainReduction, impactDrainReduction);
        }
    }
}

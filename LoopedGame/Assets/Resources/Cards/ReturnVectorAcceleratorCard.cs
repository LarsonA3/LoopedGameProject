using UnityEngine;

[CreateAssetMenu(fileName = "Return Vector Accelerator", menuName = "Cards/Stats/Return Vector Accelerator")]
public class ReturnVectorAcceleratorCard : StatCardEffect
{
    protected override StatUpgradeType StatType => StatUpgradeType.ParryReflectSpeed;

    protected override float CommonValue => 0.05f;
    protected override float RareValue => 0.10f;
    protected override float EpicValue => 0.15f;
    protected override float LegendaryValue => 0.25f;

    protected override float StatCap => 1.50f;

    private void OnEnable()
    {
        cardName = "Return Vector Accelerator";
        description = "Increases reflected projectile speed.";
    }

    protected override void ApplyToRuntime(GameObject player, float amount)
    {
        Weapon weapon = player.GetComponentInChildren<Weapon>();

        if (weapon != null)
        {
            weapon.IncreaseParryReflectSpeedMultiplier(amount);
        }
    }
}

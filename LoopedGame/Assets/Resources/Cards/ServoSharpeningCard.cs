using UnityEngine;

[CreateAssetMenu(fileName = "Servo Sharpening", menuName = "Cards/Stats/Servo Sharpening")]
public class ServoSharpeningCard : StatCardEffect
{
    protected override StatUpgradeType StatType => StatUpgradeType.LightAttackDamage;

    protected override float CommonValue => 0.25f;
    protected override float RareValue => 0.50f;
    protected override float EpicValue => 0.75f;
    protected override float LegendaryValue => 1.25f;

    protected override float StatCap => 10f;

    private void OnEnable()
    {
        cardName = "Servo Sharpening";
        description = "Increases light attack damage.";
    }

    protected override void ApplyToRuntime(GameObject player, float amount)
    {
        Weapon weapon = player.GetComponentInChildren<Weapon>();

        if (weapon != null)
        {
            weapon.AddLightDamage(amount);
        }
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "Reinforced Chassis", menuName = "Cards/Stats/Reinforced Chassis")]
public class ReinforcedChassisCard : StatCardEffect
{
    protected override StatUpgradeType StatType => StatUpgradeType.MaximumHP;

    protected override float CommonValue => 5f;
    protected override float RareValue => 10f;
    protected override float EpicValue => 15f;
    protected override float LegendaryValue => 25f;

    protected override float StatCap => 200f;

    private void OnEnable()
    {
        cardName = "Reinforced Chassis";
        description = "Increases maximum HP.";
    }

    protected override void ApplyToRuntime(GameObject player, float amount)
    {
        PlayerHP hp = player.GetComponent<PlayerHP>();

        if (hp == null)
        {
            hp = player.GetComponentInChildren<PlayerHP>();
        }

        if (hp != null)
        {
            hp.AddMaxHealth(amount);
        }
    }
}

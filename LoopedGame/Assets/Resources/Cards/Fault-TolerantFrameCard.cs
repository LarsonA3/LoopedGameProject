using UnityEngine;

[CreateAssetMenu(fileName = "Fault-Tolerant Frame", menuName = "Cards/Stats/Fault-Tolerant Frame")]
public class FaultTolerantFrameCard : StatCardEffect
{
    protected override StatUpgradeType StatType => StatUpgradeType.InvincibilityFrames;

    protected override float CommonValue => 0.025f;
    protected override float RareValue => 0.050f;
    protected override float EpicValue => 0.075f;
    protected override float LegendaryValue => 0.125f;

    protected override float StatCap => 0.75f;

    private void OnEnable()
    {
        cardName = "Fault-Tolerant Frame";
        description = "Increases invincibility time after taking damage.";
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
            hp.AddInvincibilityFrames(amount);
        }
    }
}

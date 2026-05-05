using UnityEngine;

[CreateAssetMenu(fileName = "Dash Distance Buff", menuName = "Cards/Dash Distance Buff")]
public class DashDistBuff : CardEffect
{
    public override void Apply(GameObject player, bool isRare)
    {
        TopDownController controller = player.GetComponentInChildren<TopDownController>();

        float amount = isRare ? 0.15f : 0.1f;

        Debug.Log("used dash distance buff card");

        if (UpgradeState.Instance != null)
        {
            UpgradeState.Instance.AddDashDistance(amount);
        }

        if (controller != null)
        {
            controller.increaseDashDist(amount);
        }
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "Dash CD Buff", menuName = "Cards/Dash CD Buff")]
public class DashCDBuff : CardEffect
{
    public override void Apply(GameObject player, bool isRare)
    {
        TopDownController controller = player.GetComponentInChildren<TopDownController>();

        float amount = isRare ? 0.1f : 0.05f;

        Debug.Log("used dash cd buff card");

        if (UpgradeState.Instance != null)
        {
            UpgradeState.Instance.ReduceDashCooldown(amount);
        }

        if (controller != null)
        {
            controller.reduceDashCD(amount);
        }
    }
}

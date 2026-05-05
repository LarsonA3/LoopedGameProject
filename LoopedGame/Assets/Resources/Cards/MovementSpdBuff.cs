using UnityEngine;

[CreateAssetMenu(fileName = "New Movement Speed Buff", menuName = "Cards/Movement Buff")]
public class MovementSpdBuff : CardEffect
{
    public override void Apply(GameObject player, bool isRare)
    {
        TopDownController controller = player.GetComponentInChildren<TopDownController>();

        float amount = isRare ? 1.5f : 1f;

        Debug.Log("used movement spd buff card");

        if (UpgradeState.Instance != null)
        {
            UpgradeState.Instance.AddMoveSpeed(amount);
        }

        if (controller != null)
        {
            controller.addMoveSpd(amount);
        }
    }
}

using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[CreateAssetMenu(fileName = "New Movement Speed Buff", menuName = "Cards/Movement Buff")]
public class MovementSpdBuff : CardEffect
{
    public override void Apply(GameObject player, bool isRare)
    {
        TopDownController controller = player.GetComponentInChildren<TopDownController>();
        Debug.Log("used movement spd buff card");
        if (isRare)
        {
            //rare
            controller.addMoveSpd(1.5f);

        } else
        {
            controller.addMoveSpd(1f);
        }
    }
}

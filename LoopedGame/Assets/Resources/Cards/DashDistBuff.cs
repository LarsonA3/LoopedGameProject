using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[CreateAssetMenu(fileName = "Dash Distance Buff", menuName = "Cards/Dash Distance Buff")]
public class DashDistBuff : CardEffect
{
    public override void Apply(GameObject player, bool isRare)
    {
        TopDownController controller = player.GetComponentInChildren<TopDownController>();
        Debug.Log("used dash cd buff card");
        if (isRare)
        {
            //rare
            controller.increaseDashDist(0.15f);

        } else
        {
            controller.increaseDashDist(0.1f);
        }
    }
}

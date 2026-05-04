using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[CreateAssetMenu(fileName = "Dash CD Buff", menuName = "Cards/Dash CD Buff")]
public class DashCDBuff : CardEffect
{
    public override void Apply(GameObject player, bool isRare)
    {
        TopDownController controller = player.GetComponentInChildren<TopDownController>();
        Debug.Log("used dash cd buff card");
        if (isRare)
        {
            //rare
            controller.reduceDashCD(0.1f);

        } else
        {
            controller.reduceDashCD(0.05f);
        }
    }
}

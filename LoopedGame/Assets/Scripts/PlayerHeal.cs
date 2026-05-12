using Unity.VisualScripting;
using UnityEngine;

public class PlayerHeal : MonoBehaviour
{

    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Health"))
        {
            Destroy(collision.gameObject);
            SendMessage("Heal", 6);
            print("Player healed for 6");
        }
    }
}

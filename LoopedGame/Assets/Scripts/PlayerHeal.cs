using Unity.VisualScripting;
using UnityEngine;

public class PlayerHeal : MonoBehaviour
{

    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Health"))
        {
            Destroy(collision.gameObject);
            float healAmount = DifficultySettings.Selected switch
            {
                Difficulty.Easy => 6f,
                Difficulty.Medium => 3f,
                Difficulty.Hard => 1f,
                Difficulty.Nightmare => 0.25f,
                _ => 6f
            };

            SendMessage("Heal", healAmount);
            print("Player healed for " + healAmount);
        }
    }
}

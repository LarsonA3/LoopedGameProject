using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyHP : MonoBehaviour
{
    public float health;
    public bool isFinal = false;

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
            
            // remove this later if final boss script is done in time
            if (isFinal)
            {
                SceneManager.LoadScene("WinGame");
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
    }
}

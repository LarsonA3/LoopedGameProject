using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyHP : MonoBehaviour
{
    public float health;
    public GameObject healthPickup;
    private float startingHealth;
    public bool isFinal = false;

    void Start()
    {
        startingHealth = health;
    }
    void Update()
    {
        if (health <= 0)
        {
            GameObject healthDropInst = Instantiate(healthPickup, position:gameObject.transform.position, rotation:gameObject.transform.rotation);
            Destroy(gameObject);
            HScore.pScore += (int) startingHealth*5;
            
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
        print("Enemy took " + damage + " and has " + health + " health remaining");
    }
}

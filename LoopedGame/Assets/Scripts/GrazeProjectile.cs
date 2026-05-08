using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class GrazeProjectile : MonoBehaviour
{   
    public int grazePoints;
    private float oldHP;
    private float newHP;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) {
            oldHP = PlayerHP.currentHealth;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) { 
            newHP = PlayerHP.currentHealth;
            if (oldHP == newHP)
            {
                other.SendMessage("IncreaseScore", grazePoints, SendMessageOptions.DontRequireReceiver);        
            } else if (oldHP > newHP)
            {
                other.SendMessage("IncreaseScore", -(grazePoints/4), SendMessageOptions.DontRequireReceiver);
            }

        }
    }
}
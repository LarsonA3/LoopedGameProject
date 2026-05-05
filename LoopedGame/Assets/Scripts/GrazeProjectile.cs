using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class GrazeProjectile : MonoBehaviour
{   
    public int grazePoints;
    

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) {
            other.SendMessage("IncreaseScore", grazePoints, SendMessageOptions.DontRequireReceiver);
        }
    }
}

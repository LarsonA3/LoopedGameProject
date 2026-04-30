using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorGoNextZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Zone1Manager.Instance.nextZone();
            print("calling next zone...");
        }
    }
}

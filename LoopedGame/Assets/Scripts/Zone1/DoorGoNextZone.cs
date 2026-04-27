using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorGoNextZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            print("Entering zone 2");
            SceneManager.LoadScene("Zone2");
        }
    }
}

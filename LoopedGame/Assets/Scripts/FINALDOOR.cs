using UnityEngine;
using UnityEngine.SceneManagement;

public class FINALDOOR : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player REACHED FINAL DOOR");
            // Here you would add your code to transition to the next scene, e.g.:
            // SceneManager.LoadScene("NextSceneName");
            SceneManager.LoadScene("WinGame");
        }
    }
}

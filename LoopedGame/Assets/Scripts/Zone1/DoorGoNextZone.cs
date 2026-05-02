using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorGoNextZone : MonoBehaviour
{

    public bool allowed = true;
    private void OnTriggerEnter(Collider other)
    {
        if (allowed)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Zone1Manager.Instance.nextZone();
                print("calling next zone...");
            }
        }
    }

    public void Open()
    {
        allowed = true;
        print("opened door");
    }

    public void Close()
    {
        allowed = false;
        print("closed door");
    }
}

using UnityEngine;

public class DoorNextRoom : MonoBehaviour
{
    void Start()
    {
        
    }

    private bool done = false;
    private void OnTriggerEnter(Collider other)
    {
        if (done) { return; }
        if (other.CompareTag("Player"))
        {
            print("door detected player");
            Zone1Manager.Instance.nextRoom();
            done = true;
        }
    }
}

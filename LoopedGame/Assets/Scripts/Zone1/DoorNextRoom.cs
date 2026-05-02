using UnityEngine;

public class DoorNextRoom : MonoBehaviour
{
    public bool allowed = true;

    void Start()
    {
        
    }

    private bool done = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!allowed) { return; }
        if (done) { return; }
        if (other.CompareTag("Player"))
        {
            print("door detected player");
            Zone1Manager.Instance.nextRoom();
            done = true;
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

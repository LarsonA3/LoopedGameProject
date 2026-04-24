using UnityEngine;

public class CameraMoveWithPlayer : MonoBehaviour
{
    public GameObject plrCapsule;
    public float height = 10.0f; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(plrCapsule.transform.position.x, height, plrCapsule.transform.position.z);
    }
}

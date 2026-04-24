using UnityEngine;

public class CameraMoveWithPlayer : MonoBehaviour
{
    public GameObject plrCapsule;
    public float height = 10.0f;
    public float smoothSpeed = 8f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 target = new Vector3(plrCapsule.transform.position.x, height, plrCapsule.transform.position.z);
        transform.position = Vector3.Lerp(transform.position, target, smoothSpeed * Time.deltaTime);
    }
}

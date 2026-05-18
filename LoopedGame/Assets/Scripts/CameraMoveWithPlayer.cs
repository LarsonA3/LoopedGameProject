using UnityEngine;
using System.Collections;

public class CameraMoveWithPlayer : MonoBehaviour
{
    public GameObject plrCapsule;
    public float height = 10.0f;
    public float smoothSpeed = 8f;
    public float zOffset = -1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 target = new Vector3(plrCapsule.transform.position.x, height, plrCapsule.transform.position.z + zOffset);
        transform.position = Vector3.Lerp(transform.position, target, smoothSpeed * Time.deltaTime);
    }

    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.15f;
    private bool isShaking = false;

    public void TriggerShake()
    {
        if (!isShaking) StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float z = Random.Range(-1f, 1f) * shakeMagnitude;
            transform.position += new Vector3(x, 0f, z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isShaking = false;
    }
}

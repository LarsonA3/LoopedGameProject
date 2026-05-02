using UnityEngine;
using UnityEngine.InputSystem; // Required for PlayerInput
using System.Collections;

public class DoorGoNextZone : MonoBehaviour
{
    private GameObject cardPicker;
    public bool allowed = true;
    private bool transitioning = false;

    void Start()
    {
        var pickerScript = Object.FindFirstObjectByType<UpgradeManager>(FindObjectsInactive.Include);
        if (pickerScript != null) cardPicker = pickerScript.gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (allowed && !transitioning && other.CompareTag("Player"))
        {
            StartCoroutine(HandleZoneTransition(other.gameObject));
        }
    }

    IEnumerator HandleZoneTransition(GameObject player)
    {
        transitioning = true;

        // Find the Player Input component on the Capsule child
        var input = player.GetComponentInChildren<PlayerInput>();
        if (input != null) input.enabled = false;

        if (cardPicker != null) cardPicker.SetActive(true);

        yield return new WaitUntil(() => cardPicker.activeSelf == false);

        if (input != null) input.enabled = true;

        Zone1Manager.Instance.nextZone();
    }

    public void Open() { allowed = true; }
    public void Close() { allowed = false; }
}
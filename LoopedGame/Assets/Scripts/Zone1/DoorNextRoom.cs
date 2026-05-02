using UnityEngine;
using UnityEngine.InputSystem; // Required for PlayerInput
using System.Collections;

public class DoorNextRoom : MonoBehaviour
{
    public bool allowed = true;
    private GameObject cardPicker;
    private bool done = false;

    void Start()
    {
        var pickerScript = Object.FindFirstObjectByType<UpgradeManager>(FindObjectsInactive.Include);
        if (pickerScript != null) cardPicker = pickerScript.gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!allowed || done || !other.CompareTag("Player")) return;
        StartCoroutine(HandleRoomTransition(other.gameObject));
    }

    IEnumerator HandleRoomTransition(GameObject player)
    {
        done = true;

        // Target the Input System specifically
        var input = player.GetComponentInChildren<PlayerInput>();
        if (input != null) input.enabled = false;

        if (cardPicker != null) cardPicker.SetActive(true);

        yield return new WaitUntil(() => cardPicker.activeSelf == false);

        if (input != null) input.enabled = true;

        Zone1Manager.Instance.nextRoom();
    }

    public void Open() { allowed = true; }
    public void Close() { allowed = false; }
}
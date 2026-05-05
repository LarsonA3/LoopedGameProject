using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class DoorNextRoom : MonoBehaviour
{
    public bool allowed = true;

    private GameObject cardPicker;
    private bool done = false;

    private void Start()
    {
        var pickerScript = Object.FindFirstObjectByType<UpgradeManager>(FindObjectsInactive.Include);

        if (pickerScript != null)
        {
            cardPicker = pickerScript.gameObject;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!allowed)
        {
            return;
        }

        if (done)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        StartCoroutine(HandleRoomTransition(other.gameObject));
    }

    private IEnumerator HandleRoomTransition(GameObject player)
    {
        done = true;

        PlayerInput input = player.GetComponentInChildren<PlayerInput>();

        if (input != null)
        {
            input.enabled = false;
        }

        if (cardPicker != null)
        {
            cardPicker.SetActive(true);
            yield return new WaitUntil(() => cardPicker.activeSelf == false);
        }

        if (input != null)
        {
            input.enabled = true;
        }

        Zone1Manager.Instance.nextRoom();
    }

    public void Open()
    {
        allowed = true;
    }

    public void Close()
    {
        allowed = false;
    }
}

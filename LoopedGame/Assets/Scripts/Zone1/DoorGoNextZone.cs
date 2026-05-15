using UnityEngine;
using UnityEngine.InputSystem;
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
        if (!allowed || AreEnemiesPresent()) { Debug.Log("Door is locked! Enemies still remain."); return; }
        if (allowed && !transitioning && other.CompareTag("Player"))
            StartCoroutine(HandleZoneTransition(other.gameObject));
    }

    private bool AreEnemiesPresent()
    {
        return GameObject.FindWithTag("Enemy") != null;
    }

    IEnumerator HandleZoneTransition(GameObject player)
    {
        transitioning = true;

        var input = player.GetComponentInChildren<PlayerInput>();
        if (input != null) input.enabled = false;

        if (cardPicker != null)
        {
            cardPicker.SetActive(true);
            yield return new WaitUntil(() => !cardPicker.activeSelf);
        }

        if (input != null) input.enabled = true;

        HScore.pScore += 3000;

        SoundManager.PlaySound("transition");
        FadeTransition.Instance.StartFade(() => Zone1Manager.Instance.nextZone());
    }

    public void Open() { allowed = true; }
    public void Close() { allowed = false; }
}
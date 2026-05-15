using System.Collections;
using TMPro;
using UnityEngine;

public class ZoneBanner : MonoBehaviour
{
    public static ZoneBanner Instance;


    public TextMeshProUGUI zoneNumberText; // ZONE X
    public TextMeshProUGUI zoneNameText; // ACTUAL NAME OF ZONE

    public float displayDuration = 2f;

    private static readonly string[] ZoneNames = { "TESTING", "OFFICES", "SECURE", "FREEDOM" };

    private Coroutine _activeCoroutine;

    void Awake()
    {
        Instance = this;
        SetChildren(false);
    }

    public void Show(int zone)
    {
        if (_activeCoroutine != null) StopCoroutine(_activeCoroutine);
        _activeCoroutine = StartCoroutine(ShowRoutine(zone));
    }

    private IEnumerator ShowRoutine(int zone)
    {
        zoneNumberText.text = "ZONE " + zone;
        zoneNameText.text = zone >= 1 && zone <= 4 ? ZoneNames[zone - 1] : "";
        SetChildren(true);
        yield return new WaitForSeconds(displayDuration);
        SetChildren(false);
    }

    private void SetChildren(bool active)
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(active);
    }
}
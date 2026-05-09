using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ScreenImpactEffect : MonoBehaviour
{
    public static ScreenImpactEffect Instance;

    [Header("Hit Stop")]
    public float hitStopDuration = 0.06f;

    [Header("White Flash")]
    public float whiteDuration = 0.05f;

    [Header("B&W")]
    public float bwDuration = 0.12f;
    public Volume globalVolume;

    private ColorAdjustments colorAdj;
    private Coroutine activeRoutine;
    private Image flashImage;

    void Awake()
    {
        Instance = this;

        // Build a full-screen white overlay in code -- no Canvas setup needed
        GameObject canvasGo = new GameObject("ImpactFlashCanvas");
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        DontDestroyOnLoad(canvasGo);

        GameObject imageGo = new GameObject("FlashImage");
        imageGo.transform.SetParent(canvasGo.transform, false);
        flashImage = imageGo.AddComponent<Image>();
        flashImage.color = new Color(1f, 1f, 1f, 0f);

        RectTransform rt = flashImage.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    void Start()
    {
        if (globalVolume == null)
        {
            Debug.LogWarning("[ScreenImpactEffect] No Volume assigned.");
            return;
        }
        if (!globalVolume.profile.TryGet(out colorAdj))
            Debug.LogWarning("[ScreenImpactEffect] Volume has no ColorAdjustments override.");
    }

    public void TriggerImpact()
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(ImpactRoutine());
    }

    IEnumerator ImpactRoutine()
    {
        // Hard freeze
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(hitStopDuration);
        Time.timeScale = 1f;

        // Solid white flash
        flashImage.color = new Color(1f, 1f, 1f, 1f);
        yield return new WaitForSecondsRealtime(whiteDuration);

        // Snap to high-contrast B&W
        flashImage.color = new Color(1f, 1f, 1f, 0f);
        if (colorAdj != null)
        {
            colorAdj.saturation.Override(-100f);
            colorAdj.contrast.Override(70f);
        }
        yield return new WaitForSecondsRealtime(bwDuration);

        // Restore
        if (colorAdj != null)
        {
            colorAdj.saturation.Override(0f);
            colorAdj.contrast.Override(0f);
        }

        activeRoutine = null;
    }
}
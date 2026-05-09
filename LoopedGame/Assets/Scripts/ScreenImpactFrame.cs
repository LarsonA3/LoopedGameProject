using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenImpactEffect : MonoBehaviour
{
    public static ScreenImpactEffect Instance;

    [Header("Hit Stop")]
    public float hitStopDuration = 0.06f;

    [Header("B&W Flash")]
    public float bwDuration = 0.18f;
    public Volume globalVolume;

    private ColorAdjustments colorAdj;
    private Coroutine activeRoutine;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (globalVolume == null)
        {
            Debug.LogWarning("[ScreenImpactEffect] No Volume assigned.");
            return;
        }
        if (!globalVolume.profile.TryGet(out colorAdj))
            Debug.LogWarning("[ScreenImpactEffect] Volume has no ColorAdjustments override - add one and enable Saturation.");
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

        // Desaturate
        if (colorAdj != null)
        {
            colorAdj.saturation.Override(-100f);
            yield return new WaitForSecondsRealtime(bwDuration);
            colorAdj.saturation.Override(0f);
        }

        activeRoutine = null;
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingEffects : MonoBehaviour
{
    public static PostProcessingEffects Instance;

    [Header("Volume Reference")]
    public Volume volume;

    [Header("Dash - Chromatic Aberration")]
    public float dashCAIntensity = 1f;
    public float dashCADuration = 0.3f;

    [Header("Parry - Bloom")]
    public float parryBloomIntensity = 8f;
    public float parryBloomDuration = 0.4f;

    private ChromaticAberration ca;
    private Bloom bloom;

    private float baseCAIntensity;
    private float baseBloomIntensity;

    private Coroutine caRoutine;
    private Coroutine bloomRoutine;

    private void Awake()
    {
        Instance = this;

        if (volume == null)
        {
            Debug.LogError("[PostProcessingEffects] No Volume assigned.");
            return;
        }

        volume.profile.TryGet(out ca);
        volume.profile.TryGet(out bloom);

        if (ca != null) baseCAIntensity = ca.intensity.value;
        if (bloom != null) baseBloomIntensity = bloom.intensity.value;
    }

    public void TriggerDashCA()
    {
        if (ca == null) return;
        if (caRoutine != null) StopCoroutine(caRoutine);
        caRoutine = StartCoroutine(PulseCA());
    }

    public void TriggerParryBloom()
    {
        if (bloom == null) return;
        if (bloomRoutine != null) StopCoroutine(bloomRoutine);
        bloomRoutine = StartCoroutine(PulseBloom());
    }

    private IEnumerator PulseCA()
    {
        ca.intensity.value = dashCAIntensity;

        float elapsed = 0f;
        while (elapsed < dashCADuration)
        {
            elapsed += Time.deltaTime;
            ca.intensity.value = Mathf.Lerp(dashCAIntensity, baseCAIntensity, elapsed / dashCADuration);
            yield return null;
        }

        ca.intensity.value = baseCAIntensity;
        caRoutine = null;
    }

    private IEnumerator PulseBloom()
    {
        bloom.intensity.value = parryBloomIntensity;

        float elapsed = 0f;
        while (elapsed < parryBloomDuration)
        {
            elapsed += Time.deltaTime;
            bloom.intensity.value = Mathf.Lerp(parryBloomIntensity, baseBloomIntensity, elapsed / parryBloomDuration);
            yield return null;
        }

        bloom.intensity.value = baseBloomIntensity;
        bloomRoutine = null;
    }
}
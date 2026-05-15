using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeTransition : MonoBehaviour
{
    public static FadeTransition Instance;

    [Header("References")]
    public Image blackImage; //img black

    public float fadeDuration = 0.3f;

    void Awake()
    {
        Instance = this;
        SetAlpha(0f);
    }

    public void StartFade(Action onBlack)
    {
        StartCoroutine(FadeRoutine(onBlack));
    }

    private IEnumerator FadeRoutine(Action onBlack)
    {
        // Fade to black
        yield return Fade(0f, 1f);

        // Room/zone switch happens here — door may be destroyed after this call
        onBlack?.Invoke();

        // Brief pause so the new room has a frame to render
        yield return null;
        yield return null;

        // Fade from black
        yield return Fade(1f, 0f);
    }

    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        SetAlpha(from);
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(from, to, elapsed / fadeDuration));
            yield return null;
        }
        SetAlpha(to);
    }

    private void SetAlpha(float a)
    {
        if (blackImage == null) return;
        Color c = blackImage.color;
        c.a = a;
        blackImage.color = c;
    }
}
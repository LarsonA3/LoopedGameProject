using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    private AudioSource _source;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        _source = gameObject.AddComponent<AudioSource>();
    }

    public static void PlaySound(string soundName, float pitchVariance = 0.1f, float volumeVariance = 0.1f)
    {
        if (Instance == null)
        {
            Debug.LogWarning("[SoundManager] No instance in scene.");
            return;
        }

        AudioClip clip = Resources.Load<AudioClip>("Sounds/" + soundName);

        if (clip == null)
        {
            Debug.LogWarning("[SoundManager] Could not find Resources/Sounds/" + soundName);
            return;
        }

        Instance._source.pitch = Random.Range(1f - pitchVariance, 1f + pitchVariance);
        float volume = Random.Range(1f - volumeVariance, 1f);
        Instance._source.PlayOneShot(clip, volume);
    }
}
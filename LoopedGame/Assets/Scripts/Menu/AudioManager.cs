using UnityEngine;
using UnityEngine.UI;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private Slider masterSlider;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);



        float saved = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        AudioListener.volume = saved;

        if (masterSlider != null)
        {
            masterSlider.value = saved;
            masterSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    //currently reference to slider breaks when goign to different scene, will fix later
}
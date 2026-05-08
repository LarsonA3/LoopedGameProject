using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHealthBar : MonoBehaviour
{

    [SerializeField] private Slider slider;

    [SerializeField] private TMP_Text labelText;

    void OnEnable()
    {
        PlayerHP.OnHealthChanged += HandleHealthChanged;
    }

    void OnDisable()
    {
        PlayerHP.OnHealthChanged -= HandleHealthChanged;
    }

    void HandleHealthChanged(float current, float max)
    {
        if (slider != null)
        {
            slider.minValue = 0f;
            slider.maxValue = max;
            slider.value = current;
        }

        if (labelText != null)
            labelText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }
}
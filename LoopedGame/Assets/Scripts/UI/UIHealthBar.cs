using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public PlayerHP playerHP;
    public Slider healthSlider;

    private void Start()
    {
        if (playerHP == null)
        {
            playerHP = FindObjectOfType<PlayerHP>();
        }

        if (playerHP == null)
        {
            Debug.LogWarning("[PlayerHealthUI] No PlayerHP found.");
            return;
        }

        playerHP.OnHealthChanged += UpdateHealthUI;
        UpdateHealthUI(playerHP.CurrentHP, playerHP.MaxHP);
    }

    private void OnDestroy()
    {
        if (playerHP != null)
        {
            playerHP.OnHealthChanged -= UpdateHealthUI;
        }
    }

    private void UpdateHealthUI(float currentHP, float maxHP)
    {
        if (healthSlider == null)
        {
            return;
        }

        healthSlider.maxValue = maxHP;
        healthSlider.value = currentHP;
    }
}

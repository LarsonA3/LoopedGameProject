using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyManagerTitleScreen : MonoBehaviour
{
    [Header("References")]
    public TMP_Dropdown difficultyDropdown;
    public string mainSceneName = "MainScene";

    private void Start()
    {
        // Default to Easy
        difficultyDropdown.value = 0;
        difficultyDropdown.onValueChanged.AddListener(OnDifficultyChanged);
    }

    private void OnDifficultyChanged(int index)
    {
        DifficultySettings.Selected = (Difficulty)index;
    }

    public void StartGame()
    {
        // Call this from your Play button's OnClick
        DifficultySettings.Selected = (Difficulty)difficultyDropdown.value;
        SceneManager.LoadScene(mainSceneName);
    }
}
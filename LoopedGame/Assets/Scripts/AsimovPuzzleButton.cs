using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AsimovPuzzleButton : MonoBehaviour
{
    public Button button;
    public Image buttonImage;
    public TMP_Text labelText;

    [HideInInspector] public string word;
    [HideInInspector] public string matchID;
    [HideInInspector] public bool isLeftSide;
    [HideInInspector] public bool isLockedIn;

    private AsimovPuzzleManager puzzle;
    private Color defaultColor;

    public void Setup(string newWord, string newMatchID, bool leftSide, AsimovPuzzleManager owner)
    {
        word = newWord;
        matchID = newMatchID;
        isLeftSide = leftSide;
        puzzle = owner;
        isLockedIn = false;

        if (buttonImage != null)
            defaultColor = buttonImage.color;

        if (labelText != null)
            labelText.text = word;

        if (button != null)
        {
            button.interactable = true;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClicked);
        }

        SetDefaultVisual();
    }

    private void OnClicked()
    {
        if (isLockedIn) return;

        if (puzzle != null)
            puzzle.SelectWord(this);
    }

    public void SetSelectedVisual()
    {
        if (buttonImage != null)
            buttonImage.color = Color.yellow;
    }

    public void SetLockedVisual()
    {
        isLockedIn = true;

        if (buttonImage != null)
            buttonImage.color = Color.gray;
    }

    public void SetDefaultVisual()
    {
        if (buttonImage != null)
            buttonImage.color = defaultColor;

        if (labelText != null)
            labelText.text = word;
    }

    public void FlashCorrectVisual()
    {
        if (buttonImage != null)
            buttonImage.color = Color.blue;

        if (labelText != null)
            labelText.text = "✓ " + word;
    }

    public void FlashWrongVisual()
    {
        if (buttonImage != null)
            buttonImage.color = Color.red;

        if (labelText != null)
            labelText.text = "✕ " + word;
    }
}

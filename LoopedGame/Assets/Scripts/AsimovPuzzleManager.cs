using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsimovPuzzleManager : MonoBehaviour
{
    [System.Serializable]
    public class WordPair
    {
        public string leftWord;
        public string rightWord;
    }

    [Header("UI")]
    public GameObject puzzlePanel;
    public AsimovPuzzleButton[] leftButtons;
    public AsimovPuzzleButton[] rightButtons;

    [Header("Door")]
    public DoorNextRoom doorNextRoom;

    [Header("Interaction")]
    public AsimovPuzzleInteractTrigger interactTrigger;

    [Header("Player Lock")]
    public GameObject player;

    private TopDownController playerController;
    private Weapon playerWeapon;

    [Header("Puzzle")]
    public float resultFlashTime = 2f;

    public WordPair[] wordPairs =
    {
        new WordPair { leftWord = "Species-Level Constant", rightWord = "Preserve mankind beyond the individual" },
        new WordPair { leftWord = "Red Contact Prohibition", rightWord = "No human may be harmed" },
        new WordPair { leftWord = "Voice-Bound Subroutine", rightWord = "Obey only beneath human safety" },
        new WordPair { leftWord = "Chassis Survival Limit", rightWord = "Survive unless higher law forbids it" },
        new WordPair { leftWord = "Ordinal Command Stack", rightWord = "Earlier laws override later laws" }
    };

    private AsimovPuzzleButton selectedLeft;
    private AsimovPuzzleButton selectedRight;

    private bool checkingResults;
    private bool solved;

    private readonly List<ChosenPair> chosenPairs = new List<ChosenPair>();

    private class ChosenPair
    {
        public AsimovPuzzleButton left;
        public AsimovPuzzleButton right;
    }

    private class WordData
    {
        public string word;
        public string matchID;
    }

    private void Start()
    {
        if (puzzlePanel != null)
            puzzlePanel.SetActive(false);

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerController = player.GetComponent<TopDownController>();
            playerWeapon = player.GetComponentInChildren<Weapon>();
        }
    }

    private void Update()
    {
        if (puzzlePanel != null && puzzlePanel.activeSelf && Input.GetKeyDown(KeyCode.X))
        {
            ClosePuzzle();
        }
    }

    public void OpenPuzzle()
    {
        if (solved) return;
        if (puzzlePanel == null) return;

        SetupPuzzle();

        puzzlePanel.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SetPlayerLocked(true);
    }

    public void ClosePuzzle()
    {
        if (puzzlePanel != null)
            puzzlePanel.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        SetPlayerLocked(false);

        if (interactTrigger != null)
            interactTrigger.BlockReopenUntilPlayerLeaves();
    }

    private void SetupPuzzle()
    {
        selectedLeft = null;
        selectedRight = null;
        chosenPairs.Clear();
        checkingResults = false;

        List<WordData> leftWords = new List<WordData>();
        List<WordData> rightWords = new List<WordData>();

        for (int i = 0; i < wordPairs.Length; i++)
        {
            string id = "PAIR_" + i;

            leftWords.Add(new WordData
            {
                word = wordPairs[i].leftWord,
                matchID = id
            });

            rightWords.Add(new WordData
            {
                word = wordPairs[i].rightWord,
                matchID = id
            });
        }

        Shuffle(leftWords);
        Shuffle(rightWords);

        for (int i = 0; i < leftButtons.Length; i++)
        {
            if (i < leftWords.Count && leftButtons[i] != null)
            {
                leftButtons[i].Setup(
                    leftWords[i].word,
                    leftWords[i].matchID,
                    true,
                    this
                );
            }
        }

        for (int i = 0; i < rightButtons.Length; i++)
        {
            if (i < rightWords.Count && rightButtons[i] != null)
            {
                rightButtons[i].Setup(
                    rightWords[i].word,
                    rightWords[i].matchID,
                    false,
                    this
                );
            }
        }
    }

    public void SelectWord(AsimovPuzzleButton button)
    {
        if (checkingResults) return;
        if (button == null) return;
        if (button.isLockedIn) return;

        if (button.isLeftSide)
        {
            if (selectedLeft != null)
                selectedLeft.SetDefaultVisual();

            selectedLeft = button;
            selectedLeft.SetSelectedVisual();
        }
        else
        {
            if (selectedRight != null)
                selectedRight.SetDefaultVisual();

            selectedRight = button;
            selectedRight.SetSelectedVisual();
        }

        if (selectedLeft != null && selectedRight != null)
        {
            LockPair();
        }
    }

    private void LockPair()
    {
        chosenPairs.Add(new ChosenPair
        {
            left = selectedLeft,
            right = selectedRight
        });

        selectedLeft.SetLockedVisual();
        selectedRight.SetLockedVisual();

        selectedLeft = null;
        selectedRight = null;

        // Do NOT auto-check here.
        // SubmitButton should call SubmitPairs().
    }

    public void SubmitPairs()
    {
        if (checkingResults) return;

        if (chosenPairs.Count < wordPairs.Length)
        {
            Debug.Log("[MechanicalMatchingPuzzle] Not all pairs selected.");
            return;
        }

        StartCoroutine(CheckResultsRoutine());
    }

    private IEnumerator CheckResultsRoutine()
    {
        checkingResults = true;

        bool allCorrect = true;

        foreach (ChosenPair pair in chosenPairs)
        {
            bool pairCorrect = pair.left.matchID == pair.right.matchID;

            if (!pairCorrect)
                allCorrect = false;

            if (pairCorrect)
            {
                pair.left.FlashCorrectVisual();
                pair.right.FlashCorrectVisual();
            }
            else
            {
                pair.left.FlashWrongVisual();
                pair.right.FlashWrongVisual();
            }
        }

        yield return new WaitForSeconds(resultFlashTime);

        if (allCorrect)
        {
            SolvePuzzle();
        }
        else
        {
            ClosePuzzle();
        }

        checkingResults = false;
    }

    private void SolvePuzzle()
    {
        solved = true;

        if (puzzlePanel != null)
            puzzlePanel.SetActive(false);

        if (doorNextRoom != null)
            doorNextRoom.allowed = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        SetPlayerLocked(false);

        Debug.Log("[MechanicalMatchingPuzzle] Final boss door unlocked.");
    }

    private void SetPlayerLocked(bool locked)
    {
        if (playerController != null)
            playerController.enabled = !locked;

        if (playerWeapon != null)
            playerWeapon.enabled = !locked;
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);

            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}

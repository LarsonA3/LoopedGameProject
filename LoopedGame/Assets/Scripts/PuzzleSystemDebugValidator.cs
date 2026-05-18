using UnityEngine;
using UnityEngine.UI;

public class PuzzleSystemDebugValidator : MonoBehaviour
{
    [Header("Main References")]
    public AsimovPuzzleManager asimovPuzzleManager;
    public FinalBossDoorInteract asimovPuzzleInteractTrigger;

    [Header("Optional")]
    public bool runOnStart = true;
    public bool checkButtonReferences = true;

    private void Start()
    {
        if (!runOnStart)
        {
            return;
        }

        ValidatePuzzleSystem();
    }

    public void ValidatePuzzleSystem()
    {
        Debug.Log("[Puzzle Debug] Play mode started. Checking Asimov puzzle setup...");

        bool allGood = true;

        if (asimovPuzzleManager == null)
        {
            asimovPuzzleManager = FindObjectOfType<AsimovPuzzleManager>();
        }

        if (asimovPuzzleInteractTrigger == null)
        {
            asimovPuzzleInteractTrigger = FindObjectOfType<FinalBossDoorInteract>();
        }

        if (asimovPuzzleManager == null)
        {
            Debug.LogError("[Puzzle Debug] AsimovPuzzleManager missing. Add it to FinalBossPuzzleManager.");
            allGood = false;
        }
        else
        {
            Debug.Log("[Puzzle Debug] Found AsimovPuzzleManager on: " + asimovPuzzleManager.gameObject.name);
            ValidatePuzzle(asimovPuzzleManager, ref allGood);
        }

        if (asimovPuzzleInteractTrigger == null)
        {
            Debug.LogError("[Puzzle Debug] FinalBossDoorInteract missing. Add it to PuzzleInteractTrigger.");
            allGood = false;
        }
        else
        {
            Debug.Log("[Puzzle Debug] Found FinalBossDoorInteract on: " + asimovPuzzleInteractTrigger.gameObject.name);
            ValidateInteractTrigger(asimovPuzzleInteractTrigger, ref allGood);
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("[Puzzle Debug] No GameObject tagged Player found. Puzzle can still be tested manually, but trigger/player locking may fail.");
        }
        else
        {
            Debug.Log("[Puzzle Debug] Found Player tagged object: " + player.name);
        }

        if (allGood)
        {
            Debug.Log("[Puzzle Debug] SUCCESS: Puzzle system appears correctly wired.");
        }
        else
        {
            Debug.LogError("[Puzzle Debug] Puzzle system has setup problems. Check errors above.");
        }
    }

    private void ValidatePuzzle(AsimovPuzzleManager puzzleToCheck, ref bool allGood)
    {
        if (puzzleToCheck.puzzlePanel == null)
        {
            Debug.LogError("[Puzzle Debug] Puzzle Panel is not assigned on AsimovPuzzleManager.");
            allGood = false;
        }
        else
        {
            Debug.Log("[Puzzle Debug] Puzzle Panel assigned: " + puzzleToCheck.puzzlePanel.name);
        }

        if (puzzleToCheck.leftButtons == null || puzzleToCheck.leftButtons.Length != 5)
        {
            Debug.LogError("[Puzzle Debug] Left Buttons should have exactly 5 elements.");
            allGood = false;
        }
        else
        {
            Debug.Log("[Puzzle Debug] Left Buttons count OK: " + puzzleToCheck.leftButtons.Length);
            ValidatePuzzleButtons("Left", puzzleToCheck.leftButtons, ref allGood);
        }

        if (puzzleToCheck.rightButtons == null || puzzleToCheck.rightButtons.Length != 5)
        {
            Debug.LogError("[Puzzle Debug] Right Buttons should have exactly 5 elements.");
            allGood = false;
        }
        else
        {
            Debug.Log("[Puzzle Debug] Right Buttons count OK: " + puzzleToCheck.rightButtons.Length);
            ValidatePuzzleButtons("Right", puzzleToCheck.rightButtons, ref allGood);
        }

        if (puzzleToCheck.wordPairs == null || puzzleToCheck.wordPairs.Length != 5)
        {
            Debug.LogError("[Puzzle Debug] Word Pairs should have exactly 5 elements.");
            allGood = false;
        }
        else
        {
            Debug.Log("[Puzzle Debug] Word Pairs count OK: " + puzzleToCheck.wordPairs.Length);

            for (int i = 0; i < puzzleToCheck.wordPairs.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(puzzleToCheck.wordPairs[i].leftWord))
                {
                    Debug.LogError("[Puzzle Debug] Word Pair " + i + " has an empty left word.");
                    allGood = false;
                }

                if (string.IsNullOrWhiteSpace(puzzleToCheck.wordPairs[i].rightWord))
                {
                    Debug.LogError("[Puzzle Debug] Word Pair " + i + " has an empty right word.");
                    allGood = false;
                }
            }
        }

        if (puzzleToCheck.doorNextRoom == null)
        {
            Debug.LogWarning("[Puzzle Debug] Door Next Room is not assigned. Puzzle can be tested, but it will not unlock the final door.");
        }
        else
        {
            Debug.Log("[Puzzle Debug] Door Next Room assigned: " + puzzleToCheck.doorNextRoom.gameObject.name);
        }

        if (puzzleToCheck.interactTrigger == null)
        {
            Debug.LogWarning("[Puzzle Debug] Interact Trigger is not assigned on MechanicalMatchingPuzzle. Non-instant re-entry blocking may not work.");
        }
        else
        {
            Debug.Log("[Puzzle Debug] Interact Trigger assigned: " + puzzleToCheck.interactTrigger.gameObject.name);
        }

        if (puzzleToCheck.player == null)
        {
            Debug.LogWarning("[Puzzle Debug] Player is not assigned on MechanicalMatchingPuzzle. It will try to find Player by tag at runtime.");
        }
        else
        {
            Debug.Log("[Puzzle Debug] Player assigned: " + puzzleToCheck.player.name);
        }
    }

    private void ValidateInteractTrigger(FinalBossDoorInteract triggerToCheck, ref bool allGood)
    {
        Collider triggerCollider = triggerToCheck.GetComponent<Collider>();

        if (triggerCollider == null)
        {
            Debug.LogError("[Puzzle Debug] PuzzleInteractTrigger has no Collider.");
            allGood = false;
        }
        else if (!triggerCollider.isTrigger)
        {
            Debug.LogError("[Puzzle Debug] PuzzleInteractTrigger Collider exists, but Is Trigger is not checked.");
            allGood = false;
        }
        else
        {
            Debug.Log("[Puzzle Debug] PuzzleInteractTrigger Collider is valid.");
        }

        if (triggerToCheck.puzzle == null)
        {
            Debug.LogError("[Puzzle Debug] FinalBossDoorInteract has no Puzzle assigned.");
            allGood = false;
        }
        else
        {
            Debug.Log("[Puzzle Debug] FinalBossDoorInteract Puzzle assigned: " + triggerToCheck.puzzle.gameObject.name);
        }

        if (string.IsNullOrWhiteSpace(triggerToCheck.playerTag))
        {
            Debug.LogError("[Puzzle Debug] FinalBossDoorInteract Player Tag is empty.");
            allGood = false;
        }
        else
        {
            Debug.Log("[Puzzle Debug] FinalBossDoorInteract Player Tag: " + triggerToCheck.playerTag);
        }
    }

    private void ValidatePuzzleButtons(string sideName, AsimovPuzzleButton[] buttons, ref bool allGood)
    {
        if (!checkButtonReferences)
        {
            return;
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            AsimovPuzzleButton wordButton = buttons[i];

            if (wordButton == null)
            {
                Debug.LogError("[Puzzle Debug] " + sideName + " Button " + i + " is missing from the array.");
                allGood = false;
                continue;
            }

            if (wordButton.button == null)
            {
                Debug.LogError("[Puzzle Debug] " + sideName + " Button " + i + " is missing its Button reference.");
                allGood = false;
            }

            if (wordButton.buttonImage == null)
            {
                Debug.LogError("[Puzzle Debug] " + sideName + " Button " + i + " is missing its Button Image reference.");
                allGood = false;
            }

            if (wordButton.labelText == null)
            {
                Debug.LogError("[Puzzle Debug] " + sideName + " Button " + i + " is missing its Label Text reference.");
                allGood = false;
            }

            Button unityButton = wordButton.GetComponent<Button>();

            if (unityButton == null)
            {
                Debug.LogError("[Puzzle Debug] " + sideName + " Button " + i + " does not have a Unity Button component.");
                allGood = false;
            }

            Image image = wordButton.GetComponent<Image>();

            if (image == null)
            {
                Debug.LogError("[Puzzle Debug] " + sideName + " Button " + i + " does not have an Image component.");
                allGood = false;
            }
        }
    }
}

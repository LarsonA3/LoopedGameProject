using UnityEngine;
using UnityEngine.InputSystem;

public class AsimovPuzzleInteractTrigger : MonoBehaviour
{
    public AsimovPuzzleManager puzzle;
    public string playerTag = "Player";

    private bool playerInRange;
    private bool blockedUntilPlayerLeaves;

    private PlayerInput playerInput;
    private InputAction interactAction;

    private void Update()
    {
        if (!playerInRange) return;
        if (blockedUntilPlayerLeaves) return;

        if (interactAction != null && interactAction.WasPressedThisFrame())
        {
            Debug.Log("[AsimovPuzzleInteractTrigger] Interact pressed.");

            if (puzzle != null)
            {
                puzzle.OpenPuzzle();
            }
            else
            {
                Debug.LogWarning("[AsimovPuzzleInteractTrigger] Puzzle reference is missing.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        Debug.Log("[AsimovPuzzleInteractTrigger] Player entered puzzle trigger.");

        playerInRange = true;

        playerInput = other.GetComponent<PlayerInput>();

        if (playerInput == null)
            playerInput = other.GetComponentInParent<PlayerInput>();

        if (playerInput != null)
        {
            interactAction = playerInput.actions.FindAction("Interact", true);
            Debug.Log("[AsimovPuzzleInteractTrigger] Interact action found.");
        }
        else
        {
            Debug.LogWarning("[AsimovPuzzleInteractTrigger] No PlayerInput found.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        Debug.Log("[AsimovPuzzleInteractTrigger] Player exited puzzle trigger.");

        playerInRange = false;
        blockedUntilPlayerLeaves = false;

        playerInput = null;
        interactAction = null;

        if (puzzle != null)
            puzzle.ClosePuzzle();
    }

    public void BlockReopenUntilPlayerLeaves()
    {
        blockedUntilPlayerLeaves = true;
    }
}

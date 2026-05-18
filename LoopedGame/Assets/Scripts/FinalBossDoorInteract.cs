using UnityEngine;
using UnityEngine.InputSystem;

public class FinalBossDoorInteract : MonoBehaviour
{
    public AsimovPuzzleManager puzzle;
    public string playerTag = "Player";

    private bool playerInRange;
    private PlayerInput playerInput;
    private InputAction interactAction;

    private void Update()
    {
        if (!playerInRange) return;

        if (interactAction != null && interactAction.WasPressedThisFrame())
        {
            if (puzzle != null)
                puzzle.OpenPuzzle();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerInRange = true;

        playerInput = other.GetComponent<PlayerInput>();

        if (playerInput == null)
            playerInput = other.GetComponentInParent<PlayerInput>();

        if (playerInput != null)
            interactAction = playerInput.actions.FindAction("Interact", true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerInRange = false;
        playerInput = null;
        interactAction = null;

        if (puzzle != null)
            puzzle.ClosePuzzle();
    }
}


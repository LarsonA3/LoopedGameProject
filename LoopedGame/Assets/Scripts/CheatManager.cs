using UnityEngine;
using UnityEngine.InputSystem;

public class CheatManager : MonoBehaviour
{
    private InputAction _cheat1;
    private InputAction _cheat2;

    private void Awake()
    {
        var asset = GetComponent<PlayerInput>()?.actions
                    ?? FindObjectOfType<PlayerInput>()?.actions;

        if (asset == null)
        {
            Debug.LogError("[CheatManager] No PlayerInput found.");
            enabled = false;
            return;
        }

        _cheat1 = asset.FindAction("UI/Cheat1", true);
        _cheat2 = asset.FindAction("UI/Cheat2", true);
    }

    private void OnEnable()
    {
        _cheat1?.Enable();
        _cheat2?.Enable();

        if (_cheat1 != null) _cheat1.performed += OnCheat1;
        if (_cheat2 != null) _cheat2.performed += OnCheat2;
    }

    private void OnDisable()
    {
        if (_cheat1 != null) _cheat1.performed -= OnCheat1;
        if (_cheat2 != null) _cheat2.performed -= OnCheat2;
    }

    private void OnCheat1(InputAction.CallbackContext ctx)
    {
        Debug.Log("[CHEAT] Full reset triggered.");
        Zone1Manager.Instance.ResetAfterPlayerDeath();
    }

    private void OnCheat2(InputAction.CallbackContext ctx)
    {
        Debug.Log("[CHEAT] Skip to pre-final room triggered.");
        Zone1Manager.Instance.CheatSkipToPreFinal();
    }
}
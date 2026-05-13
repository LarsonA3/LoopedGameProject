using UnityEngine;

public class DeathManager : MonoBehaviour
{
    private bool handlingDeath;

    public void HandlePlayerDeath(PlayerHP player)
    {
        if (handlingDeath)
        {
            return;
        }

        handlingDeath = true;

        if (Zone1Manager.Instance != null)
        {
            Zone1Manager.Instance.ResetAfterPlayerDeath();
        }
        else
        {
            Debug.LogWarning("[DeathManager] No Zone1Manager instance found.");
        }

        handlingDeath = false;
    }
}

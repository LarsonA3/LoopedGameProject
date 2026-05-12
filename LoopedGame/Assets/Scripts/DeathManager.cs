using UnityEngine;

public class DeathManager : MonoBehaviour
{
    [Header("Options")]
    public bool resetRunInsteadOfGameOver = true;

    [Header("Optional Fallback Respawn")]
    public Transform fallbackRespawnPoint;
    public float fallbackRespawnHealth = 100f;

    [Header("Optional UI")]
    public GameObject gameOverPanel;

    private bool handlingDeath;

    public void HandlePlayerDeath(PlayerHP player)
    {
        if (handlingDeath)
        {
            return;
        }

        if (player == null)
        {
            Debug.LogWarning("[DeathManager] Tried to handle player death, but PlayerHP was null.");
            return;
        }

        handlingDeath = true;

        if (resetRunInsteadOfGameOver)
        {
            ResetRun(player);
        }
        else
        {
            GameOver();
        }

        handlingDeath = false;
    }

    private void ResetRun(PlayerHP player)
    {
        Time.timeScale = 1f;

        if (Zone1Manager.Instance != null)
        {
            Zone1Manager.Instance.resetRun();
            Debug.Log("[DeathManager] Run reset through Zone1Manager.");
            return;
        }

        Debug.LogWarning("[DeathManager] No Zone1Manager instance found. Using fallback respawn.");

        FallbackRespawn(player);
    }

    private void FallbackRespawn(PlayerHP player)
    {
        Vector3 respawnPosition = player.transform.position;

        if (fallbackRespawnPoint != null)
        {
            respawnPosition = fallbackRespawnPoint.position;
        }

        player.ReviveAt(respawnPosition, fallbackRespawnHealth);

        TopDownController controller = player.GetComponent<TopDownController>();

        if (controller != null)
        {
            controller.ResetRunMovementState();
        }

        Weapon weapon = player.GetComponentInChildren<Weapon>();

        if (weapon != null)
        {
            weapon.ResetRunWeaponState();
        }

        ResetTemporaryCardEffects(player);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        Debug.Log("[DeathManager] Player fallback-respawned.");
    }

    private void ResetTemporaryCardEffects(PlayerHP player)
    {
        if (player == null)
        {
            return;
        }

        PlayerRareCardAbilityController rareCards =
            player.GetComponent<PlayerRareCardAbilityController>();

        if (rareCards != null)
        {
            rareCards.ResetRunState();
        }

        PlayerEpicCardAbilityController epicCards =
            player.GetComponent<PlayerEpicCardAbilityController>();

        if (epicCards != null)
        {
            epicCards.ResetRunState();
        }

        PlayerLegendaryCardAbilityController legendaryCards =
            player.GetComponent<PlayerLegendaryCardAbilityController>();

        if (legendaryCards != null)
        {
            legendaryCards.ResetRunLegendaryFlags();
        }
    }

    private void GameOver()
    {
        Debug.Log("[DeathManager] Game Over.");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }
}

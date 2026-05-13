using UnityEngine;

public class GrazeProjectile : MonoBehaviour
{
    public int grazePoints = 10;

    private float oldHP;
    private PlayerHP playerHP;
    private GameObject playerObject;

    private bool touchingPlayer;
    private bool grazeFailed;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerObject = other.gameObject;

        playerHP = other.GetComponent<PlayerHP>();

        if (playerHP == null)
        {
            playerHP = other.GetComponentInParent<PlayerHP>();
        }

        if (playerHP == null)
        {
            Debug.LogWarning("[GrazeProjectile] Player tag found, but no PlayerHP component found.");
            return;
        }

        oldHP = playerHP.CurrentHP;
        touchingPlayer = true;
        grazeFailed = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!touchingPlayer)
        {
            return;
        }

        if (grazeFailed)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (playerHP == null)
        {
            return;
        }

        float newHP = playerHP.CurrentHP;

        if (oldHP > newHP)
        {
            grazeFailed = true;
            PenalizeGraze(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!touchingPlayer)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (playerHP == null)
        {
            ResetGrazeState();
            return;
        }

        float newHP = playerHP.CurrentHP;

        if (!grazeFailed && oldHP == newHP)
        {
            AwardGraze(other.gameObject);
        }
        else if (!grazeFailed && oldHP > newHP)
        {
            PenalizeGraze(other.gameObject);
        }

        ResetGrazeState();
    }

    private void AwardGraze(GameObject playerObject)
    {
        playerObject.SendMessage(
            "IncreaseScore",
            grazePoints,
            SendMessageOptions.DontRequireReceiver
        );

        PlayerRareCardAbilityController rareCards =
            playerObject.GetComponent<PlayerRareCardAbilityController>();

        if (rareCards == null)
        {
            rareCards = playerObject.GetComponentInParent<PlayerRareCardAbilityController>();
        }

        if (rareCards != null)
        {
            rareCards.OnGraze(gameObject);
        }

        PlayerEpicCardAbilityController epicCards =
            playerObject.GetComponent<PlayerEpicCardAbilityController>();

        if (epicCards == null)
        {
            epicCards = playerObject.GetComponentInParent<PlayerEpicCardAbilityController>();
        }

        if (epicCards != null)
        {
            epicCards.OnGraze();
        }

        PlayerLegendaryCardAbilityController legendaryCards =
            playerObject.GetComponent<PlayerLegendaryCardAbilityController>();

        if (legendaryCards == null)
        {
            legendaryCards = playerObject.GetComponentInParent<PlayerLegendaryCardAbilityController>();
        }

        if (legendaryCards != null)
        {
            legendaryCards.OnGraze();
        }

        Debug.Log("[GrazeProjectile] Successful graze.");
    }

    private void PenalizeGraze(GameObject playerObject)
    {
        playerObject.SendMessage(
            "IncreaseScore",
            -(grazePoints / 4),
            SendMessageOptions.DontRequireReceiver
        );

        Debug.Log("[GrazeProjectile] Graze failed because player took damage.");
    }

    private void ResetGrazeState()
    {
        touchingPlayer = false;
        grazeFailed = false;
        playerHP = null;
        playerObject = null;
    }
}

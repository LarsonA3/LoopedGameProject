using UnityEngine;

public class FinalRoomHandler : MonoBehaviour
{
    [Header("References")]
    public GameObject bossPrefab;
    public GameObject bossBlocker;

    private GameObject _bossInstance;
    private bool _bossDefeated = false;
    private GameObject player;
    private GameObject nodeHost;

    void Start()
    {
        // get player
        foreach (GameObject root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (root.name == "PLAYER")
            {
                // transform.Find looks for a child by name
                Transform capsuleTransform = root.transform.Find("PlayerCapsule");

                if (capsuleTransform != null)
                {
                    player = capsuleTransform.gameObject;
                }
                else
                {
                    Debug.LogError("Found PLAYER root, but couldn't find 'PlayerCapsule' child!");
                }
                break;
            }
        }
        if (player == null) print("PLAYER not found at scene root");

        // get this room's nodes
        Transform nodesTransform = transform.Find("NODES");
        if (nodesTransform != null)
            nodeHost = nodesTransform.gameObject;
        else
            print("No NODES child found under this room");

        if (bossPrefab != null)
        {
            Vector3 spawnPos = new Vector3(0.72f, 1f, 25.32f);
            _bossInstance = Instantiate(bossPrefab, spawnPos, Quaternion.identity, transform);
            _bossInstance.SetActive(true);
            EnemyPatrol _bossPatrol = _bossInstance.GetComponent<EnemyPatrol>();
            _bossPatrol.target = player;
            _bossPatrol.nodeHost = nodeHost;
            
        }

    }

    void Update()
    {
        if (_bossDefeated) return;

        if (_bossInstance == null)
        {
            _bossDefeated = true;

            if (bossBlocker != null)
            {
                Destroy(bossBlocker);
                bossBlocker = null;
            }
        }
    }
}
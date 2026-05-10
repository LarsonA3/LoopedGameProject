using UnityEngine;

public class FinalRoomHandler : MonoBehaviour
{
    [Header("References")]
    public GameObject bossPrefab;
    public GameObject bossBlocker;

    private GameObject _bossInstance;
    private bool _bossDefeated = false;

    void Start()
    {
        if (bossPrefab != null)
        {
            Vector3 spawnPos = new Vector3(0.72f, 1f, 25.32f);
            _bossInstance = Instantiate(bossPrefab, spawnPos, Quaternion.identity, transform);
            _bossInstance.SetActive(true);
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
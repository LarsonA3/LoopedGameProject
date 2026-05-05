using UnityEngine;

[CreateAssetMenu(fileName = "EnemyRegistry", menuName = "Game/Enemy Registry")]
public class EnemyRegistry : ScriptableObject
{
    public GameObject slimePrefab;
    public GameObject cannonPrefab;
    public GameObject launcherPrefab;
}   
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnData", menuName = "Spawn/EnemySpawnData")]
public class EnemySpawnData : ScriptableObject
{
    public int SpawnEnemyIndex;
    public int SpawnerCount;
    public int MinSpawn;
    public int MaxSpawn;
}

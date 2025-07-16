using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wave/WaveData")]
public class WaveData : ScriptableObject
{
    [Header("스폰 가능한 적 프리팹들")]
    public List<GameObject> enemyPrefabs;

    [Header("이 웨이브에서 생성할 적 수")]
    public int minSpawnCount = 3;
    public int maxSpawnCount = 6;
}


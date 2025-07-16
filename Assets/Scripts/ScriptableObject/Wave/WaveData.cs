using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wave/WaveData")]
public class WaveData : ScriptableObject
{
    [Header("���� ������ �� �����յ�")]
    public List<GameObject> enemyPrefabs;

    [Header("�� ���̺꿡�� ������ �� ��")]
    public int minSpawnCount = 3;
    public int maxSpawnCount = 6;
}


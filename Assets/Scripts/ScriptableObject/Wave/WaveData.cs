// WaveData.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Waves/WaveData")]
public class WaveData : ScriptableObject
{
    [Header("�� ���̺꿡�� ���� ������ �׷� �����յ�")]
    public List<GameObject> spawnerGroupPrefabs;
}

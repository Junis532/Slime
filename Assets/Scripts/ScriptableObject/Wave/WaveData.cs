// WaveData.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Waves/WaveData")]
public class WaveData : ScriptableObject
{
    [Header("이 웨이브에서 사용될 스포너 그룹 프리팹들")]
    public List<GameObject> spawnerGroupPrefabs;
}

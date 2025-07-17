using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveData : ScriptableObject
{
    [Header("스킬별 몬스터 프리팹 리스트 (예: 0=1번 스킬, 1=2번...)")]
    public List<SkillMonsterList> skillMonsterLists; // 8개(스킬별) 리스트, 각 리스트 안에 여러 프리팹

    public int minSpawnCount = 3;
    public int maxSpawnCount = 5;
}

[System.Serializable]
public class SkillMonsterList
{
    public List<GameObject> monsters;
}

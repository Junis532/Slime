using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveData : ScriptableObject
{
    [Header("��ų�� ���� ������ ����Ʈ (��: 0=1�� ��ų, 1=2��...)")]
    public List<SkillMonsterList> skillMonsterLists; // 8��(��ų��) ����Ʈ, �� ����Ʈ �ȿ� ���� ������

    public int minSpawnCount = 3;
    public int maxSpawnCount = 5;
}

[System.Serializable]
public class SkillMonsterList
{
    public List<GameObject> monsters;
}

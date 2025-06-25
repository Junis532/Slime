using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "LongRangeEnemyStats", menuName = "Custom/LongRange Enemy  Stats")]
public class LongRangeEnemyStats : ScriptableObject
{
    public int id;
    public int level;
    public int maxHP;
    public int currentHP;
    public float speed;
    public int attack;
    public int magic;
    public float attackSpeed;
    public float defense;


    public void ResetStats()
    {
        speed = 4f;
        maxHP = 20;
        currentHP = maxHP;
        speed = 2.5f;
        attack = 1;
    }

}
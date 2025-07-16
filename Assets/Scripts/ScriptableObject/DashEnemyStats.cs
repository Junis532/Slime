using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "DashEnemyStats", menuName = "Custom/Dash Enemy  Stats")]
public class DashEnemyStats : ScriptableObject
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
        speed = 3.8f;
        maxHP = 20;
        currentHP = maxHP;
        speed = 3.4f;
        attack = 1;
    }
}
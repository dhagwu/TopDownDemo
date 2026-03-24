using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "TopDownDemo/Configs/Enemy Config")]
public class EnemyConfigSO : ScriptableObject
{
    [Header("Move")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public float patrolWaitTime = 0.5f;

    [Header("Detect / Attack")]
    public float detectRange = 6f;
    public float attackRange = 1.2f;
    public float attackCooldown = 1f;
    public int contactDamage = 1;

    [Header("HP")]
    public int maxHp = 3;
}
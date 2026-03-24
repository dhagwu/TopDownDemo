using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "TopDownDemo/Configs/Player Config")]
public class PlayerConfigSO : ScriptableObject
{
    [Header("Move")]
    public float moveSpeed = 5f;
    public float turnSpeed = 12f;
    public float gravity = -20f;

    [Header("Dash")]
    public float dashSpeed = 12f;
    public float dashDuration = 0.12f;
    public float dashCooldown = 0.4f;

    [Header("Combat")]
    public int attackDamage = 1;
    public float attackRadius = 1.2f;
    public float attackCooldown = 0.35f;

    [Header("HP")]
    public int maxHp = 5;
}
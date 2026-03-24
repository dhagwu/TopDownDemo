using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamageable
{
    [Header("Config")]
    [SerializeField] private EnemyConfigSO config;

    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform player;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private HUDController hudController;

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = true;

    private int currentHp;
    private int patrolIndex;
    private float patrolWaitTimer;
    private float attackCooldownTimer;

    private enum EnemyState
    {
        Patrol,
        Chase
    }

    private EnemyState currentState;

    private float PatrolSpeed => config != null ? config.patrolSpeed : 2f;
    private float ChaseSpeed => config != null ? config.chaseSpeed : 3.5f;
    private float PatrolWaitTime => config != null ? config.patrolWaitTime : 0.5f;
    private float DetectRange => config != null ? config.detectRange : 6f;
    private float AttackRange => config != null ? config.attackRange : 1.2f;
    private float AttackCooldown => config != null ? config.attackCooldown : 1f;
    private int ContactDamage => config != null ? config.contactDamage : 1;
    private int MaxHp => config != null ? config.maxHp : 3;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        TryResolveSceneReferences();

        currentHp = MaxHp;
        currentState = EnemyState.Patrol;
        patrolWaitTimer = PatrolWaitTime;

        ApplyPatrolSettings();
        GoToCurrentPatrolPoint();
        UpdateAnimatorSpeed();
    }

    private void Update()
    {
        if (attackCooldownTimer > 0f)
            attackCooldownTimer -= Time.deltaTime;

        TryResolveSceneReferences();

        bool hasPlayer = player != null;
        float distanceToPlayer = hasPlayer
            ? Vector3.Distance(transform.position, player.position)
            : float.MaxValue;

        if (hasPlayer && distanceToPlayer <= DetectRange)
        {
            UpdateChase(distanceToPlayer);
        }
        else
        {
            UpdatePatrol();
        }

        UpdateAnimatorSpeed();
    }

    private void TryResolveSceneReferences()
    {
        if (playerStats == null)
            playerStats = FindFirstObjectByType<PlayerStats>();

        if (player == null && playerStats != null)
            player = playerStats.transform;

        if (hudController == null)
            hudController = FindFirstObjectByType<HUDController>();
    }

    private void UpdatePatrol()
    {
        if (currentState != EnemyState.Patrol)
        {
            EnterPatrolState();
        }

        if (agent == null || !agent.isOnNavMesh)
            return;

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            agent.isStopped = true;
            return;
        }

        if (agent.pathPending)
            return;

        bool reachedPoint = !agent.hasPath ||
                            agent.remainingDistance <= Mathf.Max(agent.stoppingDistance, 0.05f);

        if (!reachedPoint)
        {
            patrolWaitTimer = PatrolWaitTime;
            return;
        }

        patrolWaitTimer -= Time.deltaTime;

        if (patrolWaitTimer <= 0f)
        {
            patrolIndex++;
            if (patrolIndex >= patrolPoints.Length)
                patrolIndex = 0;

            GoToCurrentPatrolPoint();
            patrolWaitTimer = PatrolWaitTime;
        }
    }

    private void UpdateChase(float distanceToPlayer)
    {
        if (currentState != EnemyState.Chase)
        {
            EnterChaseState();
        }

        if (agent == null || !agent.isOnNavMesh || player == null)
            return;

        if (distanceToPlayer > AttackRange)
        {
            if (agent.isStopped)
                agent.isStopped = false;

            agent.stoppingDistance = AttackRange;
            agent.SetDestination(player.position);
            return;
        }

        if (!agent.isStopped)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        TryAttack();
    }

    private void EnterPatrolState()
    {
        currentState = EnemyState.Patrol;
        ApplyPatrolSettings();

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            if (patrolIndex >= patrolPoints.Length)
                patrolIndex = 0;

            GoToCurrentPatrolPoint();
        }

        patrolWaitTimer = PatrolWaitTime;
    }

    private void EnterChaseState()
    {
        currentState = EnemyState.Chase;
        ApplyChaseSettings();
    }

    private void ApplyPatrolSettings()
    {
        if (agent == null)
            return;

        agent.speed = PatrolSpeed;
        agent.stoppingDistance = 0f;
        agent.isStopped = false;
    }

    private void ApplyChaseSettings()
    {
        if (agent == null)
            return;

        agent.speed = ChaseSpeed;
        agent.stoppingDistance = AttackRange;
        agent.isStopped = false;
    }

    private void GoToCurrentPatrolPoint()
    {
        if (agent == null || !agent.isOnNavMesh)
            return;

        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        Transform targetPoint = patrolPoints[patrolIndex];
        if (targetPoint == null)
            return;

        agent.isStopped = false;
        agent.SetDestination(targetPoint.position);
    }

    private void TryAttack()
    {
        if (attackCooldownTimer > 0f)
            return;

        attackCooldownTimer = AttackCooldown;

        if (animator != null)
            animator.SetTrigger("Attack");

        if (playerStats != null)
            playerStats.TakeDamage(ContactDamage);
    }

    public void TakeDamage(int value)
    {
        currentHp = Mathf.Clamp(currentHp - value, 0, MaxHp);

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (hudController != null)
            hudController.AddKill();

        Destroy(gameObject);
    }

    private void UpdateAnimatorSpeed()
    {
        if (animator == null)
            return;

        float speedValue = 0f;

        if (agent != null)
            speedValue = agent.velocity.magnitude;

        animator.SetFloat("Speed", speedValue);
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos)
            return;

        float detect = config != null ? config.detectRange : 6f;
        float attack = config != null ? config.attackRange : 1.2f;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detect);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attack);
    }
}
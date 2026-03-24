using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;

    [Header("Stats")]
    [SerializeField] private int maxHp = 3;

    [Header("Detection")]
    [SerializeField] private float detectRange = 8f;
    [SerializeField] private float attackRange = 1.5f;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private int contactDamage = 1;

    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;

    private int currentHp;
    private int patrolIndex;
    private float attackTimer;

    private NavMeshAgent agent;
    private PlayerStats playerStats;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        currentHp = maxHp;
    }

    private void Start()
    {
        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning($"{gameObject.name} is not on NavMesh.");
            return;
        }

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning($"{gameObject.name} could not find Player by tag.");
            }
        }

        if (player != null)
        {
            playerStats = player.GetComponent<PlayerStats>();
        }

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            patrolIndex = 0;
            agent.SetDestination(patrolPoints[patrolIndex].position);
        }
    }

    private void Update()
    {
        if (!agent.isOnNavMesh) return;

        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;

        if (player == null)
        {
            Patrol();
            UpdateAnimatorSpeed();
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);

            if (distance <= attackRange)
            {
                agent.isStopped = true;

                if (attackTimer <= 0f && playerStats != null)
                {
                    playerStats.TakeDamage(contactDamage);
                    attackTimer = attackCooldown;

                    if (animator != null)
                    {
                        animator.SetTrigger("Attack");
                    }
                }
            }
        }
        else
        {
            Patrol();
        }

        UpdateAnimatorSpeed();
    }

    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        agent.isStopped = false;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[patrolIndex].position);
        }
    }

    private void UpdateAnimatorSpeed()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    public void TakeDamage(int value)
    {
        currentHp -= value;
        Debug.Log($"{gameObject.name} HP = {currentHp}");

        if (currentHp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
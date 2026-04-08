using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference attackAction;

    [Header("Config")]
    [SerializeField] private PlayerConfigSO config;

    [Header("References")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask enemyLayer;

    private float attackCooldownTimer;

    private void OnEnable()
    {
        if (attackAction != null)
        {
            attackAction.action.Enable();
            attackAction.action.performed += OnAttack;
        }
    }

    private void OnDisable()
    {
        if (attackAction != null)
        {
            attackAction.action.performed -= OnAttack;
            attackAction.action.Disable();
        }
    }

    private void Update()
    {
        if (attackCooldownTimer > 0f)
            attackCooldownTimer -= Time.deltaTime;
    }

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        if (config == null || attackPoint == null)
            return;

        if (attackCooldownTimer > 0f)
            return;

        attackCooldownTimer = config.attackCooldown;

        if (animator != null)
            animator.SetTrigger("Attack");

        GameAudioManager.Instance?.PlayPlayerAttack();

        Collider[] hits = Physics.OverlapSphere(
            attackPoint.position,
            config.attackRadius,
            enemyLayer
        );

        foreach (Collider hit in hits)
        {
            IDamageable damageable = hit.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(GetDamage());
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        float radius = config != null ? config.attackRadius : 1f;
        Gizmos.DrawWireSphere(attackPoint.position, radius);
    }

    public virtual int GetDamage()
    {
        return GetBaseConfigDamage();
    }

    public int GetBaseConfigDamage()
    {
        return config != null ? config.attackDamage : 0;
    }
}
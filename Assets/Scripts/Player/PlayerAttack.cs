using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private InputActionReference attackAction;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Animator animator;
    [SerializeField] private float attackRadius = 1.2f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float cooldown = 0.35f;
    [SerializeField] private LayerMask enemyMask;

    private float cooldownTimer;

    private void OnEnable()
    {
        attackAction.action.Enable();
        attackAction.action.performed += OnAttack;
    }

    private void OnDisable()
    {
        attackAction.action.performed -= OnAttack;
        attackAction.action.Disable();
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        if (cooldownTimer > 0f) return;

        cooldownTimer = cooldown;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        Collider[] hits = Physics.OverlapSphere(
            attackPoint.position,
            attackRadius,
            enemyMask
        );

        foreach (Collider hit in hits)
        {
            MonoBehaviour[] components = hit.GetComponentsInParent<MonoBehaviour>();

            foreach (MonoBehaviour component in components)
            {
                if (component is IDamageable damageable)
                {
                    damageable.TakeDamage(damage);
                    break;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
using UnityEngine;

public class EnemyDummy : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHp = 3;

    private int currentHp;

    private void Awake()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(int value)
    {
        currentHp -= value;
        Debug.Log($"{gameObject.name} took {value} damage. HP = {currentHp}");

        if (currentHp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
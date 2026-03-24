using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int maxHp = 5;

    public int CurrentHp { get; private set; }

    private void Awake()
    {
        CurrentHp = maxHp;
    }

    public void TakeDamage(int value)
    {
        CurrentHp = Mathf.Max(0, CurrentHp - value);
        Debug.Log($"Player HP: {CurrentHp}");

        if (CurrentHp <= 0)
        {
            Debug.Log("Player Dead");
        }
    }
}
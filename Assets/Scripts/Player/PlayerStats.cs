using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private PlayerConfigSO config;

    public event Action<int, int> OnHpChanged;

    public int CurrentHp { get; private set; }
    public int MaxHp => config != null ? config.maxHp : 1;

    private void Awake()
    {
        CurrentHp = MaxHp;
    }

    private void Start()
    {
        OnHpChanged?.Invoke(CurrentHp, MaxHp);
    }

    public void ResetHpFromConfig()
    {
        CurrentHp = MaxHp;
        OnHpChanged?.Invoke(CurrentHp, MaxHp);
    }

    public void SetHpFromSave(int value)
    {
        CurrentHp = Mathf.Clamp(value, 0, MaxHp);
        OnHpChanged?.Invoke(CurrentHp, MaxHp);
    }

    public void TakeDamage(int value)
    {
        if (value <= 0)
            return;

        int previousHp = CurrentHp;
        CurrentHp = Mathf.Clamp(CurrentHp - value, 0, MaxHp);

        if (CurrentHp < previousHp)
            GameAudioManager.Instance?.PlayPlayerHit();

        OnHpChanged?.Invoke(CurrentHp, MaxHp);

        if (CurrentHp <= 0)
        {
            Debug.Log("Player dead");
        }
    }

    public void Heal(int value)
    {
        CurrentHp = Mathf.Clamp(CurrentHp + value, 0, MaxHp);
        OnHpChanged?.Invoke(CurrentHp, MaxHp);
    }
}
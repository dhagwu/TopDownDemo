using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private LevelConfigSO levelConfig;

    [Header("References")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TMP_Text hpValueText;
    [SerializeField] private TMP_Text killText;
    [SerializeField] private TMP_Text objectiveText;

    public int CurrentKillCount { get; private set; }

    private void Start()
    {
        if (playerStats == null)
            playerStats = FindFirstObjectByType<PlayerStats>();

        if (playerStats != null)
        {
            playerStats.OnHpChanged -= RefreshHp;
            playerStats.OnHpChanged += RefreshHp;
            RefreshHp(playerStats.CurrentHp, playerStats.MaxHp);
        }

        RefreshObjectiveText();
        RefreshKillText();
    }

    private void OnDestroy()
    {
        if (playerStats != null)
            playerStats.OnHpChanged -= RefreshHp;
    }

    private void RefreshHp(int currentHp, int maxHp)
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHp;
            hpSlider.value = currentHp;
        }

        if (hpValueText != null)
            hpValueText.text = $"HP: {currentHp}/{maxHp}";
    }

    private void RefreshKillText()
    {
        if (killText != null)
            killText.text = $"Kills: {CurrentKillCount}";
    }

    private void RefreshObjectiveText()
    {
        if (objectiveText == null || levelConfig == null)
            return;

        bool completed =
            levelConfig.targetKillCount > 0 &&
            CurrentKillCount >= levelConfig.targetKillCount;

        objectiveText.text = completed
            ? levelConfig.completedObjectiveText
            : levelConfig.objectiveText;
    }

    public void AddKill()
    {
        CurrentKillCount++;
        RefreshKillText();
        RefreshObjectiveText();
    }

    public void SetKillCount(int value)
    {
        CurrentKillCount = Mathf.Max(0, value);
        RefreshKillText();
        RefreshObjectiveText();
    }

    public void SetObjective(string message)
    {
        if (objectiveText != null)
            objectiveText.text = message;
    }
}
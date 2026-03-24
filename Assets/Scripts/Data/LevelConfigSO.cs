using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "TopDownDemo/Configs/Level Config")]
public class LevelConfigSO : ScriptableObject
{
    [Header("Meta")]
    public string levelDisplayName = "Level 01";

    [Header("Objective")]
    [TextArea(2, 4)]
    public string objectiveText = "Objective: Eliminate enemies";

    [TextArea(2, 4)]
    public string completedObjectiveText = "Objective: Area clear";

    public int targetKillCount = 1;
}
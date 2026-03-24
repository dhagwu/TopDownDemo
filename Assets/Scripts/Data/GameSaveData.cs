using System;

[Serializable]
public class GameSaveData
{
    public string sceneName;

    public float playerPosX;
    public float playerPosY;
    public float playerPosZ;

    public int playerCurrentHp;
    public int killCount;
}
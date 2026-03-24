using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private const string SaveFileName = "save.json";

    public static string SavePath
    {
        get
        {
            return Path.Combine(Application.persistentDataPath, SaveFileName);
        }
    }

    public static void Save(GameSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"Save success: {SavePath}");
    }

    public static GameSaveData Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning($"Save file not found: {SavePath}");
            return null;
        }

        string json = File.ReadAllText(SavePath);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

        Debug.Log($"Load success: {SavePath}");
        return data;
    }

    public static bool HasSave()
    {
        return File.Exists(SavePath);
    }

    public static void DeleteSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log($"Save deleted: {SavePath}");
        }
    }
}
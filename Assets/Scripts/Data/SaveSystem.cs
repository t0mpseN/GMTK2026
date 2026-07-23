using System.IO;
using UnityEngine;

public static class SaveSystem
{
    // FIELDS & PROPERTIES
    private static string FilePath => Path.Combine(Application.persistentDataPath, "save.json");


    // METHODS
    public static void Save(PlayerData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(FilePath, json);
    }
    
    public static PlayerData Load()
    {
        if (!File.Exists(FilePath))
            return new PlayerData();

        string json = File.ReadAllText(FilePath);
        return JsonUtility.FromJson<PlayerData>(json) ?? new PlayerData();
    }
}
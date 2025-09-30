using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string SAVE_FOLDER = Application.persistentDataPath + "/Saves/";
    private static readonly string SAVE_EXTENSION = ".json";
    
    public static void Initialize()
    {
        if (!Directory.Exists(SAVE_FOLDER))
        {
            Directory.CreateDirectory(SAVE_FOLDER);
            Debug.Log("Created save directory at: " + SAVE_FOLDER);
        }
    }
    
    public static void SaveData<T>(string saveFileName, T data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SAVE_FOLDER + saveFileName + SAVE_EXTENSION, json);
        Debug.Log($"Saved data to {saveFileName}{SAVE_EXTENSION}");
    }
    
    public static T LoadData<T>(string saveFileName) where T : new()
    {
        string path = SAVE_FOLDER + saveFileName + SAVE_EXTENSION;
        
        if (!File.Exists(path))
        {
            Debug.LogWarning($"Save file {path} not found. Creating new data.");
            return new T();
        }
        
        string json = File.ReadAllText(path);
        T data = JsonUtility.FromJson<T>(json);
        return data;
    }
}
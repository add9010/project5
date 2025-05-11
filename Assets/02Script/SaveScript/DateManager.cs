using System.IO;
using UnityEngine;

public class DateManager : MonoBehaviour
{
    private static DateManager instance;
    public static DateManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject container = new GameObject("DateManager");
                instance = container.AddComponent<DateManager>();
                DontDestroyOnLoad(container); // 씬이 변경되어도 유지
            }
            return instance;
        }
    }

    private string GameDateFileName = "GameData.json";

    public Data LoadData()
    {
        string filePath = Application.persistentDataPath + "/" + GameDateFileName;

        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            Data loaded = JsonUtility.FromJson<Data>(jsonData);
            loaded.RestoreAfterLoad(); // 🔁 복원
            return loaded;
        }

        return new Data();
    }

    public void SaveData(Data data)
    {
        data.PrepareForSave(); // 🔁 변환
        string jsonData = JsonUtility.ToJson(data, true);
        string filePath = Application.persistentDataPath + "/" + GameDateFileName;
        File.WriteAllText(filePath, jsonData);
        Debug.Log("저장 완료!");
    }

}

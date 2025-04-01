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

    // 파일에서 데이터 불러오기
    public Data LoadData()
    {
        string filePath = Application.persistentDataPath + "/" + GameDateFileName;

        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            return JsonUtility.FromJson<Data>(jsonData);
        }
        return new Data(); // 파일이 없으면 새로운 데이터 반환
    }

    // 데이터를 파일에 저장
    public void SaveData(Data data)
    {
        string jsonData = JsonUtility.ToJson(data, true);
        string filePath = Application.persistentDataPath + "/" + GameDateFileName;
        File.WriteAllText(filePath, jsonData);
        Debug.Log("저장 완료!");
    }
}

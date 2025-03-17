using System.IO;
using UnityEngine;

public class DateManager : MonoBehaviour
{
    static GameObject container;
    static DateManager instance;

    public static DateManager Instance
    {
        get
        {
            if (!instance)
            {
                container = new GameObject();
                container.name = "DateManager";
                instance = container.AddComponent(typeof(DateManager))as DateManager;
            }
            return instance;
        }
    }

    string GameDateFileName = "GameDate.josn";

    public Data data = new Data();

    public void LoadGameData()
    {
        string filePath = Application.persistentDataPath + "/" + GameDateFileName;

        if (File.Exists(filePath))
        {
            string FromJsonData = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<Data>(FromJsonData);
            print("불러오기 완료");
        }
    }

    public void SaveGameData()
    {
        string ToJsonData = JsonUtility.ToJson(data, true);
        string filePath = Application.persistentDataPath + "/" + GameDateFileName;

        File.WriteAllText(filePath, ToJsonData);

        print("저장완료");
        for (int i = 0; i < data.isUnlock.Length; i++)
        {
            print($"{i}번 챕터 잠금 해제 여부 : " + data.isUnlock[i]);
        }

    }

}

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
                DontDestroyOnLoad(container); // ���� ����Ǿ ����
            }
            return instance;
        }
    }

    private string GameDateFileName = "GameData.json";

    // ���Ͽ��� ������ �ҷ�����
    public Data LoadData()
    {
        string filePath = Application.persistentDataPath + "/" + GameDateFileName;

        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            return JsonUtility.FromJson<Data>(jsonData);
        }
        return new Data(); // ������ ������ ���ο� ������ ��ȯ
    }

    // �����͸� ���Ͽ� ����
    public void SaveData(Data data)
    {
        string jsonData = JsonUtility.ToJson(data, true);
        string filePath = Application.persistentDataPath + "/" + GameDateFileName;
        File.WriteAllText(filePath, jsonData);
        Debug.Log("���� �Ϸ�!");
    }
}

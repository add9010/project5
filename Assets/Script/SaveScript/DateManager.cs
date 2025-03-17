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
            print("�ҷ����� �Ϸ�");
        }
    }

    public void SaveGameData()
    {
        string ToJsonData = JsonUtility.ToJson(data, true);
        string filePath = Application.persistentDataPath + "/" + GameDateFileName;

        File.WriteAllText(filePath, ToJsonData);

        print("����Ϸ�");
        for (int i = 0; i < data.isUnlock.Length; i++)
        {
            print($"{i}�� é�� ��� ���� ���� : " + data.isUnlock[i]);
        }

    }

}

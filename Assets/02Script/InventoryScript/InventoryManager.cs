using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("인벤토리 데이터")]
    public InventoryData data;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (data == null)
            Debug.LogWarning("InventoryManager.data에 인벤토리 데이터가 할당되지 않았습니다!");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            var item = LoadItemByName("강철 검");
            if (item != null)
            {
                data.storageItems.Add(item);
                InventoryUIManager.Instance?.RefreshAll();
            }
        }
    }

    ItemData LoadItemByName(string name)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Data/Items");
        if (jsonFile == null)
        {
            Debug.LogError("Items.json 파일을 찾을 수 없습니다.");
            return null;
        }

        string wrapped = "{\"items\":" + jsonFile.text + "}";
        ItemListWrapper wrapper = JsonUtility.FromJson<ItemListWrapper>(wrapped);
        if (wrapper?.items == null) return null;

        foreach (var item in wrapper.items)
        {
            if (item.itemName == name)
            {
                item.icon = Resources.Load<Sprite>(item.iconPath);
                return item;
            }
        }

        Debug.LogWarning($"{name} 아이템을 찾을 수 없습니다.");
        return null;
    }

    [System.Serializable]
    private class ItemListWrapper
    {
        public List<ItemData> items;
    }
}

using System.Collections.Generic;
using UnityEngine;

public class BoxInventoryManager : MonoBehaviour
{
    public static BoxInventoryManager Instance { get; private set; }

    [Header("창고에 들어있는 아이템 목록")]
    public List<ItemData> boxItems = new List<ItemData>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// 박스에서 꺼내기(플레이어 인벤토리에 추가)
    /// </summary>
    public void TakeOut(ItemData item)
    {
        if (boxItems.Remove(item))
        {
            // 기존 플레이어 인벤토리에 추가
            InventoryManager.Instance.data.storageItems.Add(item);
            // UI 갱신
            InventoryUIManager.Instance.RefreshAll();
            BoxUIManager.Instance.RefreshAll();
        }
    }
}

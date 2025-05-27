using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("인벤토리 데이터")]
    public InventoryData data;    // Inspector에서 드래그&드롭

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (data == null)
            Debug.LogWarning("InventoryManager.data에 인벤토리 데이터가 할당되지 않았습니다!");
    }
}

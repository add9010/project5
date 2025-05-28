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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            AddSwordToInventory();
        }
    }

    void AddSwordToInventory()
    {
        if (data == null) return;

        var sword = new ItemData
        {
            itemName = "강철 검",
            description = "단단한 철로 만들어진 검입니다.",
            type = EquipmentType.Weapon,
            icon = Resources.Load<Sprite>("Icons/IronSword") // Resources 폴더에 있어야 함
        };

        data.storageItems.Add(sword); // 인벤토리 보관함에 추가만 함

        // UI 갱신
        InventoryUIManager.Instance?.RefreshAll();
    }


}

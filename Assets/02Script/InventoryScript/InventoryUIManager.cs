using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject inventoryUI;        // 전체 패널
    public Transform equippedParent;      // 장착 슬롯 부모
    public Transform storageContent;      // 보관함 Content
    public GameObject itemSlotPrefab;     // 슬롯 프리팹

    [Header("Detail Panel")]
    public TMP_Text itemNameText;
    public Image itemIconImage;
    public TMP_Text itemDescText;
    public Button equipButton;

    private InventoryData inventory;
    private ItemData selectedItem;

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

        // InventoryManager에서 데이터 가져오기
        if (InventoryManager.Instance != null)
            inventory = InventoryManager.Instance.data;
        else
            inventory = FindAnyObjectByType<InventoryManager>()?.data;

        if (inventory == null)
            Debug.LogError("InventoryUIManager: InventoryData를 찾을 수 없습니다.");
    }

    void Start()
    {
        // 처음엔 숨기기
        if (inventoryUI != null)
            inventoryUI.SetActive(false);

        // 장착 슬롯 클릭 리스너
        foreach (Transform slot in equippedParent)
        {
            var btn = slot.GetComponent<Button>();
            var eq = slot.GetComponent<EquipmentSlot>();
            if (btn != null && eq != null)
                btn.onClick.AddListener(() => ShowItemDetail(eq.currentItem));
        }

        // 장착 버튼 리스너
        if (equipButton != null)
            equipButton.onClick.AddListener(() =>
            {
                if (selectedItem != null)
                    EquipSelectedItem();
            });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && CanOpenInventory())
            ToggleInventoryUI();
    }

    bool CanOpenInventory()
    {
        if (Camera.main == null) return false;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out var hit, 3f))
            return hit.collider.CompareTag("InventoryTrigger");
        return false;
    }

    public void ToggleInventoryUI()
    {
        if (inventoryUI == null) return;

        bool isOn = !inventoryUI.activeSelf;
        inventoryUI.SetActive(isOn);
        if (isOn) RefreshAll();
    }

    public void RefreshAll()
    {
        RefreshEquipped();
        RefreshStorage();
        ClearDetail();
    }

    void RefreshEquipped()
    {
        foreach (Transform slot in equippedParent)
        {
            var eq = slot.GetComponent<EquipmentSlot>();
            if (eq == null) continue;

            ItemData data = eq.type switch
            {
                EquipmentType.Weapon => inventory.equippedWeapon,
                EquipmentType.Armor => inventory.equippedArmor,
                EquipmentType.Accessory => inventory.equippedAccessory,
                _ => null
            };
            eq.SetItem(data);
        }
    }

    void RefreshStorage()
    {
        // 기존 슬롯 정리
        foreach (Transform t in storageContent)
            Destroy(t.gameObject);

        // 새로운 슬롯 생성
        foreach (var item in inventory.storageItems)
        {
            var slot = Instantiate(itemSlotPrefab, storageContent);
            var img = slot.GetComponent<Image>();
            var btn = slot.GetComponent<Button>();
            if (img != null) img.sprite = item.icon;
            if (btn != null)
            {
                var captured = item;
                btn.onClick.AddListener(() => ShowItemDetail(captured));
            }
        }
    }

    void ClearDetail()
    {
        selectedItem = null;
        if (itemNameText != null) itemNameText.text = "";
        if (itemIconImage != null) itemIconImage.sprite = null;
        if (itemDescText != null) itemDescText.text = "";
        if (equipButton != null) equipButton.gameObject.SetActive(false);
    }

    void ShowItemDetail(ItemData item)
    {
        selectedItem = item;
        if (itemNameText != null) itemNameText.text = item.itemName;
        if (itemIconImage != null) itemIconImage.sprite = item.icon;
        if (itemDescText != null) itemDescText.text = item.description;
        if (equipButton != null) equipButton.gameObject.SetActive(item.canEquip);
    }

    void EquipSelectedItem()
    {
        inventory.EquipItem(selectedItem);
        RefreshAll();
        ShowItemDetail(selectedItem);
    }
}

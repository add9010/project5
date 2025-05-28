using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoxUIManager : MonoBehaviour
{
    public static BoxUIManager Instance { get; private set; }

    [Header("UI 전체 패널")]
    public GameObject boxUI;               // F 눌렀을 때 토글할 Canvas

    [Header("장착 슬롯 부모")]
    public Transform equippedParent;       // Weapon/Armor/Accessory 슬롯

    [Header("박스 아이템 Content")]
    public Transform boxContent;           // GridLayoutGroup 이 붙은 Content
    public GameObject itemSlotPrefab;      // 슬롯 프리팹

    [Header("Detail Panel")]
    public TMP_Text itemNameText;
    public Image itemIconImage;
    public TMP_Text itemDescText;
    public Button takeButton;            // 꺼내기 버튼

    [Header("Player Scan(2D Raycast)")]
    private GameObject scanObject;         // 스캔된 박스 오브젝트
    private Vector2 dirVec;                // 플레이어 바라보는 방향

    // 기존 참조
    InventoryData playerInv => InventoryManager.Instance.data;
    BoxInventoryManager boxInv => BoxInventoryManager.Instance;
    private ItemData selected;        // 디테일로 선택된 아이템

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        boxUI.SetActive(false);
        // 꺼내기 버튼 리스너
        takeButton.onClick.AddListener(() =>
        {
            if (selected != null)
                boxInv.TakeOut(selected);
        });
    }

    void Update()
    {
        // 매 프레임 박스 스캔
        HandleScan();

        // F 키로 열기/닫기
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (boxUI.activeSelf)
            {
                // UI 열려 있으면 언제나 닫기
                ToggleUI();
            }
            else if (scanObject != null && scanObject.CompareTag("Box"))
            {
                // UI 닫혀 있으면 박스 앞에서만 열기
                ToggleUI();
            }
        }
    }

    /// <summary>
    /// 2D Raycast로 Box 앞 스캔
    /// </summary>
    void HandleScan()
    {
        var rb = PlayerManager.Instance.rb;
        dirVec = rb.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Debug.DrawRay(rb.position, dirVec * 0.7f, Color.green);
        int mask = LayerMask.GetMask("Object");
        RaycastHit2D hit = Physics2D.Raycast(rb.position, dirVec, 0.7f, mask);
        if (hit.collider != null && hit.collider.CompareTag("Box"))
            scanObject = hit.collider.gameObject;
        else
            scanObject = null;
    }

    /// <summary>
    /// UI 토글 및 갱신
    /// </summary>
    void ToggleUI()
    {
        bool on = !boxUI.activeSelf;
        boxUI.SetActive(on);

        if (on) RefreshAll();
        else ClearDetail();
    }

    public void RefreshAll()
    {
        RefreshEquipped();
        RefreshBoxStorage();
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
                EquipmentType.Weapon => playerInv.equippedWeapon,
                EquipmentType.Armor => playerInv.equippedArmor,
                EquipmentType.Accessory => playerInv.equippedAccessory,
                _ => null
            };
            eq.SetItem(data);
        }
    }

    void RefreshBoxStorage()
    {
        // 기존 슬롯 정리
        foreach (Transform t in boxContent) Destroy(t.gameObject);

        // 박스 아이템 만큼 슬롯 생성
        foreach (var item in boxInv.boxItems)
        {
            var slot = Instantiate(itemSlotPrefab, boxContent);
            var img = slot.GetComponent<Image>();
            var btn = slot.GetComponent<Button>();
            if (img != null) img.sprite = item.icon;
            if (btn != null)
            {
                var cap = item;
                btn.onClick.AddListener(() => ShowDetail(cap));
            }
        }
    }

    void ShowDetail(ItemData item)
    {
        selected = item;
        itemNameText.text = item.itemName;
        itemIconImage.sprite = item.icon;
        itemDescText.text = item.description;
        takeButton.gameObject.SetActive(true);
    }

    void ClearDetail()
    {
        selected = null;
        itemNameText.text = "";
        itemIconImage.sprite = null;
        itemDescText.text = "";
        takeButton.gameObject.SetActive(false);
    }
}

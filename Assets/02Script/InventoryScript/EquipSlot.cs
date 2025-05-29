using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    [Header("Slot 설정")]
    public EquipmentType type;        // Weapon / Armor / Accessory 구분
    public Image iconImage;           // 슬롯에 표시할 아이콘
    [HideInInspector] public ItemData currentItem;

    void Awake()
    {
        if (iconImage == null)
            iconImage = GetComponentInChildren<Image>();

        if (iconImage == null)
            Debug.LogWarning($"{name}: iconImage가 아직 연결되지 않았습니다 (실제 null이 아님일 수 있음).");
    }
    /// <summary>
    /// 슬롯에 아이템을 표시하거나 비울 때 호출
    /// </summary>
    public void SetItem(ItemData item)
    {
        currentItem = item;

        if (item != null)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
    }
}

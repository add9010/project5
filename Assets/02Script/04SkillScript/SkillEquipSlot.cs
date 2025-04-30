using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillEquipSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private Image iconImage;
    public SkillData EquippedSkill { get; private set; }

    private void Awake()
    {
        if (iconImage == null)
            iconImage = GetComponent<Image>(); // 또는 GetComponentInChildren<Image>()
    }

    public void Equip(SkillData skill)
    {
        EquippedSkill = skill;
        iconImage.sprite = skill.icon;
        iconImage.enabled = true;
    }

    public void Clear()
    {
        EquippedSkill = null;
        iconImage.sprite = null;
        iconImage.enabled = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        SkillDragItem draggedItem = eventData.pointerDrag?.GetComponent<SkillDragItem>();

        if (draggedItem != null)
        {
            Equip(draggedItem.skillData);

            // UI 정확하게 정렬
            RectTransform dragRect = draggedItem.GetComponent<RectTransform>();
            RectTransform slotRect = this.GetComponent<RectTransform>();

            draggedItem.transform.SetParent(this.transform, false); // 부모 변경 (레이아웃 영향 방지)
            dragRect.anchorMin = new Vector2(0.5f, 0.5f);
            dragRect.anchorMax = new Vector2(0.5f, 0.5f);
            dragRect.pivot = new Vector2(0.5f, 0.5f); // 가운데 기준
            dragRect.anchoredPosition = Vector2.zero; // 정확히 중앙 위치
            dragRect.localScale = Vector3.one; // 크기 초기화

            draggedItem.wasDroppedOnSlot = true;

            Debug.Log($"스킬 {draggedItem.skillData.skillName} 장착됨!");
        }
    }
}

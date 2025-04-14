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
            draggedItem.transform.SetParent(this.transform, false); // worldPositionStays = false
            draggedItem.transform.localPosition = Vector3.zero;

            Debug.Log($"스킬 {draggedItem.skillData.skillName} 장착됨!");
        }
    }
}

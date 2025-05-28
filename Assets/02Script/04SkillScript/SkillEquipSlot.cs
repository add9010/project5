using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillEquipSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image cooldownOverlay;
    public SkillData EquippedSkill { get; private set; }

    private float cooldownTime;
    private float lastUsedTime = -999f;

    private void Awake()
    {
        if (iconImage == null)
            iconImage = GetComponent<Image>();
        if (cooldownOverlay == null)
            cooldownOverlay = transform.parent.Find(name + " cool")?.GetComponent<Image>();
    }
    private void Start()
    {
        if (SkillManager.Instance == null) return;

        if (name.Contains("A"))
        {
            SkillManager.Instance.SetSlotA(this);
            Debug.Log($"[Slot] {name} → SkillManager.SetSlotA 등록");
        }
        else if (name.Contains("S"))
        {
            SkillManager.Instance.SetSlotS(this);
            Debug.Log($"[Slot] {name} → SkillManager.SetSlotS 등록");
        }
        else if (name.Contains("D"))
        {
            SkillManager.Instance.SetSlotD(this);
            Debug.Log($"[Slot] {name} → SkillManager.SetSlotD 등록");
        }
    }
    private void Update()
    {
        if (!Application.isPlaying || EquippedSkill == null) return;

        float remaining = Mathf.Clamp((lastUsedTime + cooldownTime - Time.time), 0f, cooldownTime);
        float percent = remaining / cooldownTime;

        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = percent;
    }

   public void Equip(SkillData skill)
{
    // 💡 안전하게 iconImage가 null일 경우 대비
    if (iconImage == null)
        iconImage = GetComponent<Image>();

    EquippedSkill = skill;
    cooldownTime = skill.cooldown;

    if (iconImage != null)
    {
        iconImage.sprite = skill.icon;
        iconImage.enabled = true;
    }

    if (cooldownOverlay != null)
        cooldownOverlay.fillAmount = 0f;

    var dragItem = GetComponentInChildren<SkillDragItem>();
    if (dragItem != null)
        dragItem.isLocked = true;

    // 저장
    if (SkillManager.Instance != null)
    {
        if (gameObject.name.Contains("A")) SkillManager.Instance.SaveSlotData('A', skill);
        if (gameObject.name.Contains("S")) SkillManager.Instance.SaveSlotData('S', skill);
        if (gameObject.name.Contains("D")) SkillManager.Instance.SaveSlotData('D', skill);
    }
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
        if (draggedItem == null) return;

        Equip(draggedItem.skillData);
        Destroy(draggedItem.gameObject);
    }

    public bool IsReady()
    {
        return Time.time >= lastUsedTime + cooldownTime;
    }

    public void MarkUsed()
    {
        lastUsedTime = Time.time;

        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = 1f;
    }

    public float GetLastUsedTime() => lastUsedTime;
}

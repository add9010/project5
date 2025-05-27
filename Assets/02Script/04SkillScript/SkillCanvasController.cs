using UnityEngine;

public class SkillCanvasController : MonoBehaviour
{
    [Header("스킬 캔버스 프리팹")]
    [SerializeField] private GameObject skillCanvasPrefab;

    private GameObject skillCanvasInstance;
    private void Start()
    {
        if (skillCanvasInstance == null)
        {
            skillCanvasInstance = Instantiate(skillCanvasPrefab);
            skillCanvasInstance.SetActive(false); // 눈에는 안 보임
            AutoAssignSlots();
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ToggleSkillCanvas();
        }
    }

    private void ToggleSkillCanvas()
    {
        if (skillCanvasInstance == null)
        {
            skillCanvasInstance = Instantiate(skillCanvasPrefab);
            AutoAssignSlots();
        }
        else
        {
            skillCanvasInstance.SetActive(!skillCanvasInstance.activeSelf);
        }
    }

    private void AutoAssignSlots()
    {
        if (skillCanvasInstance == null) return;

        SkillEquipSlot[] slots = skillCanvasInstance.GetComponentsInChildren<SkillEquipSlot>();

        foreach (var slot in slots)
        {
            string name = slot.gameObject.name;

            if (name.Contains("A")) SkillManager.Instance.SetSlotA(slot);
            else if (name.Contains("S")) SkillManager.Instance.SetSlotS(slot);
            else if (name.Contains("D")) SkillManager.Instance.SetSlotD(slot);
        }

        // ✅ 플레이어가 있는 씬에서만 복원
        if (GameObject.FindWithTag("Player") != null)
        {
            SkillManager.Instance.RestoreEquippedSkills();
        }
        else
        {
            Debug.Log("[SkillCanvasController] 플레이어가 없어 스킬 복원 생략됨");
        }
    }
}

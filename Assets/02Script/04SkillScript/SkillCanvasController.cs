using UnityEngine;

public class SkillCanvasController : MonoBehaviour
{
    [SerializeField] private GameObject skillCanvasPrefab;
    private GameObject skillCanvasInstance;

    private void Start()
    {
        // 1회만 생성
        if (skillCanvasInstance == null)
        {
            skillCanvasInstance = Instantiate(skillCanvasPrefab);
            DontDestroyOnLoad(skillCanvasInstance); // ✅ 유지되도록 함
            skillCanvasInstance.SetActive(false);
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
            Debug.LogError("SkillCanvasInstance가 존재하지 않습니다.");
            return;
        }

        bool nowActive = !skillCanvasInstance.activeSelf;
        skillCanvasInstance.SetActive(nowActive);

        if (nowActive)
        {
            AutoAssignSlots();
        }
    }

    private void AutoAssignSlots()
    {
        if (skillCanvasInstance == null) return;

        var slots = skillCanvasInstance.GetComponentsInChildren<SkillEquipSlot>(true);

        foreach (var slot in slots)
        {
            string name = slot.gameObject.name;

            if (name.Contains("A")) SkillManager.Instance.SetSlotA(slot);
            else if (name.Contains("S")) SkillManager.Instance.SetSlotS(slot);
            else if (name.Contains("D")) SkillManager.Instance.SetSlotD(slot);
        }

        SkillManager.Instance.RestoreEquippedSkills();
    }
}

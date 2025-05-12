using UnityEngine;

public class SkillCanvasController : MonoBehaviour
{
    [Header("스킬 캔버스 프리팹")]
    [SerializeField] private GameObject skillCanvasPrefab;

    private GameObject skillCanvasInstance;

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
    }
}

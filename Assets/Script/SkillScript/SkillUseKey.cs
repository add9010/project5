using UnityEngine;

public class SkillUseKey : MonoBehaviour
{
    [SerializeField] private SkillEquipSlot slot;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            UseEquippedSkill();
        }
    }

    private void UseEquippedSkill()
    {
        if (slot.EquippedSkill == null)
        {
            Debug.Log("스킬이 장착되지 않았습니다.");
            return;
        }

        GameObject prefab = slot.EquippedSkill.skillLogicPrefab;
        if (prefab == null)
        {
            Debug.LogWarning("Skill Logic Prefab이 비어 있습니다.");
            return;
        }

        GameObject instance = Instantiate(prefab);
        var logic = instance.GetComponent<Skill1>(); // 예: Skill1 전용 처리
        logic?.Activate();
        Destroy(instance); // 로그 확인 후 파괴
    }
}

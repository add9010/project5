using UnityEngine;
using System.Collections.Generic;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    [SerializeField] private List<SkillData> skillList;
    [SerializeField] private SkillEquipSlot activeSkillSlot; // 현재 슬롯 하나만 사용
    private Dictionary<string, SkillData> skillDict;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환해도 유지
        }
        else
        {
            Destroy(gameObject); // 중복 방지
            return;
        }

        skillDict = new Dictionary<string, SkillData>();

        if (skillList == null || skillList.Count == 0)
        {
            Debug.LogWarning("⚠️ SkillManager: skillList가 비어 있습니다.");
            return;
        }

        foreach (var skill in skillList)
        {
            if (skill == null || string.IsNullOrEmpty(skill.skillName)) continue;

            if (skillDict.ContainsKey(skill.skillName))
            {
                Debug.LogWarning($"⚠️ 중복된 스킬 이름: {skill.skillName}");
            }

            skillDict[skill.skillName] = skill;
        }

        Debug.Log("✅ SkillManager 초기화 완료");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            UseSkillFromSlot(activeSkillSlot);
        }
    }

    private void UseSkillFromSlot(SkillEquipSlot slot)
    {
        if (slot == null || slot.EquippedSkill == null)
        {
            Debug.Log("❌ 장착된 스킬이 없습니다.");
            return;
        }

        var prefab = slot.EquippedSkill.skillLogicPrefab;
        if (prefab == null)
        {
            Debug.LogWarning("❌ Skill Logic Prefab이 비어 있습니다.");
            return;
        }

        GameObject instance = Instantiate(prefab);
        var logic = instance.GetComponent<Skill1>(); // 추후 인터페이스로 확장 가능
        if (logic == null)
        {
            Debug.LogWarning("❌ Skill1 컴포넌트를 찾지 못했습니다!");
        }
        else
        {
            logic.Activate();
        }

        Destroy(instance);
    }

    public SkillData GetSkillByName(string name)
    {
        skillDict.TryGetValue(name, out var skill);
        return skill;
    }
}

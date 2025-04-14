using UnityEngine;
using System.Collections.Generic;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    [SerializeField] private List<SkillData> skillList;
    private Dictionary<string, SkillData> skillDict;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        skillDict = new Dictionary<string, SkillData>();

        if (skillList == null || skillList.Count == 0)
        {
            Debug.LogWarning("SkillManager: skillList가 비어 있습니다.");
            return;
        }

        foreach (var skill in skillList)
        {
            if (skill == null || string.IsNullOrEmpty(skill.skillName)) continue;

            if (skillDict.ContainsKey(skill.skillName))
            {
                Debug.LogWarning($"중복된 스킬 이름: {skill.skillName}");
            }

            skillDict[skill.skillName] = skill;
        }
    }

    public SkillData GetSkillByName(string name)
    {
        skillDict.TryGetValue(name, out var skill);
        return skill;
    }
}

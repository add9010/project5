using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skill/SkillData")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public Sprite icon;
    public GameObject skillLogicPrefab; // 여기서 Skill1 프리팹 연결

    [Header("스킬 쿨타임")]
    public float cooldown = 1.0f; // 스킬마다 개별 설정 가능
    public int manaCost = 1; // 추가


}

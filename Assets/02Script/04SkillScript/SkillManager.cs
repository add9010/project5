using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    [Header("QWE 슬롯 연결")]
    [SerializeField] private SkillEquipSlot slotA;
    [SerializeField] private SkillEquipSlot slotS;
    [SerializeField] private SkillEquipSlot slotD;

    // 저장용
    private SkillData savedSlotA;
    private SkillData savedSlotS;
    private SkillData savedSlotD;

    [SerializeField] private GameObject skillCanvasPrefab;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        if (Object.FindFirstObjectByType<SkillCanvasController>() == null)
        {
            Instantiate(skillCanvasPrefab, transform);
        }
    }
    public void SetSlotA(SkillEquipSlot slot)
    {
        slotA = slot;
        TryAutoRestore();
    }

    public void SetSlotS(SkillEquipSlot slot)
    {
        slotS = slot;
        TryAutoRestore();
    }

    public void SetSlotD(SkillEquipSlot slot)
    {
        slotD = slot;
        TryAutoRestore();
    }

    public SkillEquipSlot GetSlotA() => slotA;
    public SkillEquipSlot GetSlotS() => slotS;
    public SkillEquipSlot GetSlotD() => slotD;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) UseSkillFromSlot(slotA);
        if (Input.GetKeyDown(KeyCode.S)) UseSkillFromSlot(slotS);
        if (Input.GetKeyDown(KeyCode.D)) UseSkillFromSlot(slotD);
    }

    private void UseSkillFromSlot(SkillEquipSlot slot)
    {
        if (slot == null || slot.EquippedSkill == null)
        {
            //Debug.LogWarning("장착된 스킬이 없습니다.");
            return;
        }

        var pm = GameObject.FindWithTag("Player")?.GetComponent<PlayerManager>();
        if (pm == null || pm.IsDead) return;

        if (!slot.IsReady())
        {
            return;
        }

        if (!pm.TryUseMana(slot.EquippedSkill.manaCost)) // 마나 
        {
            return;
        }

        var prefab = slot.EquippedSkill.skillLogicPrefab;
        if (prefab == null)
        {
            Debug.LogWarning("프리팹 추가해주세요.");
            return;
        }

        GameObject instance = Instantiate(prefab);

        if (instance.TryGetComponent(out Skill1 skill1))
        {
            skill1.Initialize(pm);
            skill1.Activate();
        }
        else if (instance.TryGetComponent(out Skill2 skill2))
        {
            skill2.Initialize(pm);
            skill2.Activate();
        }

        Destroy(instance);
        slot.MarkUsed(); // 쿨타임 시작
    }

    // 슬롯 상태 수동 저장 (씬 전환 전 호출)
    public void SaveEquippedSkills()
    {
        savedSlotA = slotA?.EquippedSkill;
        savedSlotS = slotS?.EquippedSkill;
        savedSlotD = slotD?.EquippedSkill;
    }

    // 슬롯 상태 복원
    public void RestoreEquippedSkills()
    {
        if (slotA != null && savedSlotA != null) slotA.Equip(savedSlotA);
        if (slotS != null && savedSlotS != null) slotS.Equip(savedSlotS);
        if (slotD != null && savedSlotD != null) slotD.Equip(savedSlotD);
    }

   
    private void TryAutoRestore()
    {
        if (slotA != null && slotS != null && slotD != null)
        {
            RestoreEquippedSkills();
        }
    }

    // 슬롯 키 기반 저장
    public void SaveSlotData(char key, SkillData skill)
    {
        switch (key)
        {
            case 'A': savedSlotA = skill; break;
            case 'S': savedSlotS = skill; break;
            case 'D': savedSlotD = skill; break;
        }
    }
}

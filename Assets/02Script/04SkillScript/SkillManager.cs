using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    [Header("QWE 슬롯 연결")]
    [SerializeField] private SkillEquipSlot slotA;
    [SerializeField] private SkillEquipSlot slotS;
    [SerializeField] private SkillEquipSlot slotD;
    public void SetSlotA(SkillEquipSlot slot) => slotA = slot;
    public void SetSlotS(SkillEquipSlot slot) => slotS = slot;
    public void SetSlotD(SkillEquipSlot slot) => slotD = slot;
    public SkillEquipSlot GetSlotA() => slotA;
    public SkillEquipSlot GetSlotS() => slotS;
    public SkillEquipSlot GetSlotD() => slotD;
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
            Debug.LogWarning("장착된 스킬이 없습니다.");
            return;
        }

        var pm = GameObject.FindWithTag("Player")?.GetComponent<PlayerManager>();
        if (pm == null || pm.IsDead) return;

        if (!slot.IsReady())
        {
            Debug.Log($"⏳ 스킬 '{slot.EquippedSkill.skillName}' 쿨타임 진행 중");
            return;
        }

        var prefab = slot.EquippedSkill.skillLogicPrefab;
        if (prefab == null)
        {
            Debug.LogWarning("Skill Logic 프리팹이 없습니다.");
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
        else
        {
            Debug.LogWarning("알 수 없는 스킬 프리팹입니다.");
        }

        Destroy(instance);

        // ✅ 사용 후 쿨타임 갱신
        slot.MarkUsed();
    }
}

using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    [Header("QWE 슬롯 연결")]
    [SerializeField] private SkillEquipSlot slotQ;
    [SerializeField] private SkillEquipSlot slotW;
    [SerializeField] private SkillEquipSlot slotE;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) UseSkillFromSlot(slotQ);
        if (Input.GetKeyDown(KeyCode.W)) UseSkillFromSlot(slotW);
        if (Input.GetKeyDown(KeyCode.E)) UseSkillFromSlot(slotE);
    }

    private void UseSkillFromSlot(SkillEquipSlot slot)
    {
        if (slot == null || slot.EquippedSkill == null)
        {
            Debug.LogWarning(" 장착된 스킬이 없습니다.");
            return;
        }

        var prefab = slot.EquippedSkill.skillLogicPrefab;
        if (prefab == null)
        {
            Debug.LogWarning("🟡 Skill Logic 프리팹이 없습니다.");
            return;
        }

        GameObject instance = Instantiate(prefab);
        var pm = GameObject.FindWithTag("Player")?.GetComponent<PlayerManager>();

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
            Debug.LogWarning("⚠️ 알 수 없는 스킬 프리팹입니다.");
        }

        Destroy(instance);
    }
}

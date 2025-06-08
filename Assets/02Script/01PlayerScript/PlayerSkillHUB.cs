using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillHUD : MonoBehaviour
{
    [System.Serializable]
    public class SkillHUDSlot
    {
        public Image iconImage;
        public Image cooldownOverlay;
        public SkillEquipSlot trackedSlot;
    }

    [Header("액티브 스킬 슬롯 (A/S/D)")]
    public SkillHUDSlot slot1;
    public SkillHUDSlot slot2;
    public SkillHUDSlot slot3;

    [Header("대시 쿨타임 HUD")]
    public Image dashCooldownOverlay;

    [Header("패링 쿨타임 HUD")]
    public Image parryCooldownOverlay;

    [Header("마나 HUD")]
    public Image[] manaIcons; 

    private PlayerDash dash;
    private PlayerParry parry;
    private bool dashInit = false;
    private bool parryInit = false;
    private void Start()
    {
        // 스킬 슬롯 자동 연결
        if (SkillManager.Instance != null)
        {
            slot1.trackedSlot = SkillManager.Instance.GetSlotA();
            slot2.trackedSlot = SkillManager.Instance.GetSlotS();
            slot3.trackedSlot = SkillManager.Instance.GetSlotD();
        }

        // Dash / Parry 참조
        var pm = PlayerManager.Instance; 
        if (pm != null)
        {
            dash = pm.playerDash;
            parry = pm.playerParry;
            //Debug.Log("[HUD] Dash & Parry 연결 성공");
        }
        else
        {
           // Debug.LogWarning("[HUD] PlayerManager.Instance == null");
        }
    }

    private void Update()
    {
        // 스킬 슬롯 추적 유지
        if (slot1.trackedSlot == null && SkillManager.Instance != null)
            slot1.trackedSlot = SkillManager.Instance.GetSlotA();
        if (slot2.trackedSlot == null && SkillManager.Instance != null)
            slot2.trackedSlot = SkillManager.Instance.GetSlotS();
        if (slot3.trackedSlot == null && SkillManager.Instance != null)
            slot3.trackedSlot = SkillManager.Instance.GetSlotD();

        UpdateSkillSlot(slot1);
        UpdateSkillSlot(slot2);
        UpdateSkillSlot(slot3);
        UpdateManaUI();
        // ✅ Dash & Parry 안전 연결
        if (!dashInit || !parryInit)
        {
            var pm = PlayerManager.Instance;
            if (pm != null)
            {
                if (!dashInit && pm.playerDash != null)
                {
                    dash = pm.playerDash;
                    dashInit = true;
                    //Debug.Log("✅ Dash 연결 성공");
                }
                if (!parryInit && pm.playerParry != null)
                {
                    parry = pm.playerParry;
                    parryInit = true;
                   // Debug.Log("✅ Parry 연결 성공");
                }
            }
        }

        // Dash 쿨타임
        if (dashCooldownOverlay != null && dash != null)
        {
            float duration = dash.GetCooldownDuration();
            float remain = Mathf.Clamp(dash.GetLastUsedTime() + duration - Time.time, 0f, duration);
            dashCooldownOverlay.fillAmount = remain / duration;
        }

        // Parry 쿨타임
        if (parryCooldownOverlay != null && parry != null)
        {
            float duration = parry.GetCooldownDuration();
            float remain = Mathf.Clamp(parry.GetLastUsedTime() + duration - Time.time, 0f, duration);
            parryCooldownOverlay.fillAmount = remain / duration;
        }
    }

    private void UpdateSkillSlot(SkillHUDSlot slot)
    {
        if (slot.trackedSlot == null || slot.trackedSlot.EquippedSkill == null)
            return;

        // 아이콘
        if (slot.iconImage != null)
        {
            slot.iconImage.sprite = slot.trackedSlot.EquippedSkill.icon;
            slot.iconImage.enabled = true;
        }

        // 쿨타임
        float cooldown = slot.trackedSlot.EquippedSkill.cooldown;
        float usedTime = slot.trackedSlot.GetLastUsedTime();
        float remain = Mathf.Clamp(usedTime + cooldown - Time.time, 0f, cooldown);
        float percent = remain / cooldown;

        if (slot.cooldownOverlay != null)
            slot.cooldownOverlay.fillAmount = percent;
    }
    private void UpdateManaUI()
    {
        var pm = PlayerManager.Instance;
        if (pm == null || manaIcons == null || manaIcons.Length == 0) return;

        int current = pm.currentMana;
        int max = pm.data.maxMana;
        float progress = pm.GetManaRechargeProgress(); // float 0~1

        for (int i = 0; i < manaIcons.Length; i++)
        {
            if (i < current)
            {
                manaIcons[i].fillAmount = 1f; // 충전 완료
            }
            else if (i == current && current < max)
            {
                manaIcons[i].fillAmount = progress; // 현재 충전중인 마나
            }
            else
            {
                manaIcons[i].fillAmount = 0f; // 아직 비어 있음
            }
        }
    }

}

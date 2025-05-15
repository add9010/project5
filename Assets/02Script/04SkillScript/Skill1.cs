using UnityEngine;

public class Skill1 : MonoBehaviour
{
    private PlayerManager pm;

    [Header("점프 힘")]
    public float jumpForce = 7.5f;

    [Header("소환할 오브젝트 프리팹")]
    public GameObject summonPrefab;

    [Header("플레이어 위치 기준 소환 오프셋 (XYZ)")]
    public Vector3 spawnOffset = Vector3.zero;

    [Header("공격 박스 크기")]
    public Vector2 attackBoxSize = new Vector2(2f, 1.5f);

    [Header("피격 이펙트")]
    public GameObject hitEffectPrefab;

    public void Initialize(PlayerManager playerManager)
    {
        this.pm = playerManager;
    }

    public void Activate()
    {
        if (pm == null)
        {
            Debug.LogWarning("PlayerManager가 연결되지 않았습니다.");
            return;
        }

        // 상태 및 점프
        pm.playerStateController.ForceSetSkill("Skill1",AnimType.Skill1);
        pm.playerStateController.LockSkillState(0.5f);

        pm.rb.linearVelocity = new Vector2(pm.rb.linearVelocity.x, jumpForce);
        Debug.Log("Skill1 스킬 점프 실행됨!");

        // 데미지 적용
        Vector2 center = pm.transform.position;
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, attackBoxSize, 0, LayerMask.GetMask("Enemy"));

        foreach (var col in hits)
        {
            if (NetworkClient.Instance != null && NetworkClient.Instance.isConnected)
            {
                NetworkCombatManager.SendMonsterDamage((int)10f);
            }
            else
            {
                CombatManager.ApplyDamage(col.gameObject, pm.data.attackPower, 10f, pm.transform.position);
            }

            // 이펙트
            if (hitEffectPrefab != null)
            {
                Vector3 spawnPos = col.transform.position;
                spawnPos.z = 0f;
                GameObject fx = GameObject.Instantiate(hitEffectPrefab, spawnPos, Quaternion.identity);
                GameObject.Destroy(fx, 0.5f);
            }
        }

        // 오브젝트 소환
        if (summonPrefab != null)
        {
            Vector3 spawnPos = pm.transform.position + spawnOffset;
            GameObject clone = Instantiate(summonPrefab, spawnPos, Quaternion.identity);
            Debug.Log("오브젝트 소환 완료: " + clone.name);

            Destroy(clone, 0.5f);
        }
    }

    // Gizmo로 범위 확인
    private void OnDrawGizmosSelected()
    {
        if (pm == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, attackBoxSize);
    }
}

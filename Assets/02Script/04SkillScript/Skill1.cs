using System.Collections;
using System.Collections.Generic;
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
        pm.playerStateController.ForceSetSkill("Skill1", AnimType.Skill1);
        pm.playerStateController.LockSkillState(0.5f);
        pm.rb.linearVelocity = new Vector2(pm.rb.linearVelocity.x, jumpForce);
        Debug.Log("Skill1 스킬 점프 실행됨!");

        //사운드
        if (pm.skill1SFX != null)
        {
            SoundManager.Instance.PlaySFX(pm.skill1SFX);
        }

        // 데미지 적용
        Vector2 center = pm.transform.position;
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, attackBoxSize, 0, LayerMask.GetMask("Enemy"));

        List<Transform> delayedHitTargets = new(); // 추가

        foreach (var col in hits)
        {
            if (NetworkClient.Instance != null && NetworkClient.Instance.isConnected)
            {
                NetworkCombatManager.SendMonsterDamage((int)10f);
            }
            else
            {
                CombatManager.ApplyDamage(col.gameObject, pm.data.attackPower, 10f, pm.transform.position);

                Rigidbody2D enemyRb = col.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    enemyRb.linearVelocity = new Vector2(enemyRb.linearVelocity.x, 10f);
                }
            }

            delayedHitTargets.Add(col.transform); // 이펙트용 저장
        }

        // ⏱️ 딜레이 이펙트 실행
        if (hitEffectPrefab != null && delayedHitTargets.Count > 0)
        {
            pm.StartCoroutine(DelayedHitEffects(delayedHitTargets));
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
    private IEnumerator DelayedHitEffects(List<Transform> targets)
    {
        yield return new WaitForSeconds(0.3f); // 딜레이 후 현재 위치 기준

        foreach (Transform t in targets)
        {
            if (t == null) continue;

            Vector3 pos = t.position; // ✅ 현재 위치 기준
            pos.y += 0.2f; // 약간 위로만
            pos.z = 0f;

            GameObject fx = Instantiate(hitEffectPrefab, pos, Quaternion.identity);
            Destroy(fx, 0.5f);
        }
    }
    // Gizmo로 범위 확인
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, attackBoxSize);
    }
}

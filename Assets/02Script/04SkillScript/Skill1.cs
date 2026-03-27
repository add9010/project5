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
            return;
        }

        pm.playerStateController.ForceSetSkill("Skill1", AnimType.Skill1);
        pm.playerStateController.LockSkillState(0.5f);
        pm.rb.linearVelocity = new Vector2(pm.rb.linearVelocity.x, jumpForce);

        //사운드
        if (pm.skill1SFX != null)
        {
            SoundManager.Instance.PlaySFX(pm.skill1SFX);
        }

        Vector2 center = pm.transform.position;
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, attackBoxSize, 0, LayerMask.GetMask("Enemy"));

        List<Transform> delayedHitTargets = new(); // 추가

        foreach (var col in hits)
        {
            GameObject target = col.gameObject;
            delayedHitTargets.Add(col.transform); // 이펙트용 저장

            if (NetworkClient.Instance != null && NetworkClient.Instance.isConnected)
            {
                TrapVisual tv = target.GetComponent<TrapVisual>();
                if (tv != null && !string.IsNullOrEmpty(tv.trapId))
                {
                    NetworkCombatManager.SendTrapDamage(tv.trapId, (int)10f);
                    // Debug.Log($"트랩 데미지 전송: 10 to trap {tv.trapId}");
                }
                else
                {
                    NetworkCombatManager.SendMonsterDamage((int)10f);
                    // Debug.Log("몬스터 데미지 전송: 10");
                }
            }
            else
            {
                CombatManager.ApplyDamage(target, pm.data.attackPower, 10f, pm.transform.position);

                Rigidbody2D enemyRb = col.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    enemyRb.linearVelocity = new Vector2(enemyRb.linearVelocity.x, 10f);
                }
            }
        }

        if (hitEffectPrefab != null && delayedHitTargets.Count > 0)
        {
            pm.StartCoroutine(DelayedHitEffects(delayedHitTargets));
        }
    

        if (summonPrefab != null)
        {
            Vector3 spawnPos = pm.transform.position + spawnOffset;
            GameObject clone = Instantiate(summonPrefab, spawnPos, Quaternion.identity);

            Destroy(clone, 0.5f);
        }
    }
    private IEnumerator DelayedHitEffects(List<Transform> targets)
    {
        yield return new WaitForSeconds(0.3f); 

        foreach (Transform t in targets)
        {
            if (t == null) continue;

            Vector3 pos = t.position;
            pos.y += 0.2f;
            pos.z = 0f;

            GameObject fx = Instantiate(hitEffectPrefab, pos, Quaternion.identity);
            Destroy(fx, 0.5f);
        }
    }
    // Gizmo 범위
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, attackBoxSize);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill2 : MonoBehaviour
{
    private PlayerManager pm;

    public float chargeTime = 0.3f;
    public float dashSpeed = 45f;
    public float dashDuration = 0.2f;
    public Vector2 hitBoxSize = new Vector2(1.5f, 1f);
    private HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>();

    [Header("피격 이펙트 프리팹")]
    public GameObject hitEffectPrefab;

    [Header("돌진 중 불 이펙트")]
    public GameObject fireEffectPrefab;
    private GameObject fireEffectInstance;

    public void Initialize(PlayerManager playerManager)
    {
        pm = playerManager;
    }

    public void Activate()
    {
        if (pm == null || pm.IsDead) return;

        if (pm.skill2SFX != null)
            SoundManager.Instance.PlaySFX(pm.skill2SFX);

        pm.playerStateController.ForceSetSkill("Skill2", AnimType.Skill2);
        pm.playerStateController.LockSkillState(0.5f);

        // 충돌 무시
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);

        // 카메라 줌인
        pm.cameraController.ZoomIn(6f, 2f);

        pm.StartAttackCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        pm.isAction = true;
        pm.rb.linearVelocity = Vector2.zero;
        hitEnemies.Clear();

        yield return new WaitForSeconds(chargeTime); // 차징

        Vector2 dir = pm.spriteRenderer.flipX ? Vector2.left : Vector2.right;
        float elapsed = 0f;
        float currentSpeed = dashSpeed;
        Vector3 fireOffset = new Vector3(pm.spriteRenderer.flipX ? -2f : 2f, -0.5f, 0f);
        if (fireEffectPrefab != null)
        {
            // ▶ X축으로만 offset 적용 (방향 따라 1.5f 앞으로)
            

            fireEffectInstance = Instantiate(fireEffectPrefab, pm.transform.position + fireOffset, Quaternion.identity);

            // ▶ 항상 반전된 상태로 시작
            Vector3 scale = fireEffectInstance.transform.localScale;
            scale.x *= -1;
            if (pm.spriteRenderer.flipX)
                scale.x *= -1;
            fireEffectInstance.transform.localScale = scale;

            Destroy(fireEffectInstance, 0.7f);
        }
        while (elapsed < dashDuration)
        {
            pm.rb.linearVelocity = dir * currentSpeed;

            // 불 이펙트 따라가게
            if (fireEffectInstance != null)
            {
                fireEffectInstance.transform.position = pm.transform.position + fireOffset;
            }

            Vector2 boxCenter = (Vector2)pm.transform.position + dir * 0.8f;
            Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, hitBoxSize, 0, LayerMask.GetMask("Enemy"));
            foreach (var col in hits)
            {
                if (hitEnemies.Contains(col)) continue;

                GameObject target = col.gameObject;

                if (pm.skillHitSFX != null)
                    SoundManager.Instance.PlaySFX(pm.skillHitSFX, 0.5f);

                float finalDamage = pm.data.attackPower * 1.2f;

                if (NetworkClient.Instance != null && NetworkClient.Instance.isConnected)
                {
                    TrapVisual tv = target.GetComponent<TrapVisual>();
                    if (tv != null && !string.IsNullOrEmpty(tv.trapId))
                    {
                        NetworkCombatManager.SendTrapDamage(tv.trapId, (int)finalDamage);
                        // Debug.Log($"트랩 데미지 전송: {finalDamage} → {tv.trapId}");
                    }
                    else
                    {
                        NetworkCombatManager.SendMonsterDamage((int)finalDamage);
                        // Debug.Log($"몬스터 데미지 전송: {finalDamage}");
                    }
                }
                else
                {
                    CombatManager.ApplyDamage(target, finalDamage, 10f, pm.transform.position);
                }

                pm.cameraController.Shake(0.1f, 0.2f);
                hitEnemies.Add(col);

                // 이펙트
                if (hitEffectPrefab != null)
                {
                    Vector3 spawnPos = col.transform.position;
                    spawnPos.z = 0f;
                    Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(-45f, 45f));
                    GameObject fx = Instantiate(hitEffectPrefab, spawnPos, randomRotation);
                    Destroy(fx, 0.5f);
                }
            }


            currentSpeed -= 30f * Time.deltaTime;
            currentSpeed = Mathf.Max(0f, currentSpeed);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 종료 처리
        pm.rb.linearVelocity = Vector2.zero;
        pm.isAction = false;

        pm.cameraController.ResetZoom(0.3f);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
    }

    private void OnDrawGizmosSelected()
    {
        if (pm == null) return;
        Vector2 dir = pm.spriteRenderer.flipX ? Vector2.left : Vector2.right;
        Vector2 boxCenter = (Vector2)transform.position + dir * 0.8f;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCenter, hitBoxSize);
    }
}

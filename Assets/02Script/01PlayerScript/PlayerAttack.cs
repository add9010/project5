using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAttack
{
    private PlayerManager pm;
    private float timeSinceAttack;
    private float attackInputTimer = 0f;
    private const float attackResetDelay = 0.5f;
    private int attackCount;
    private bool isAttacking;
    public int CurrentCombo => attackCount;
    private HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>();

    public bool IsAttacking => isAttacking;
    float staggerDamage = 10f;
    public PlayerAttack(PlayerManager manager)
    {
        this.pm = manager;
    }

    public void Update()
    {
        timeSinceAttack += Time.deltaTime;

        // 입력 없을 경우 카운트 초기화 타이머
        if (!Input.GetKey(KeyCode.Z))
        {
            attackInputTimer += Time.deltaTime;
            if (attackInputTimer >= attackResetDelay)
            {
                attackCount = 0;
                attackInputTimer = 0f;
            }
        }
        else
        {
            attackInputTimer = 0f;
        }
    }

    public bool TryAttack()
    {
        bool zKeyPressed = Input.GetKeyDown(KeyCode.Z);
        bool readyToAttack = timeSinceAttack >= pm.data.attackDuration;
        bool notCurrentlyAttacking = !isAttacking;
        bool notDashing = !pm.isDashing; 
        return zKeyPressed && readyToAttack && notCurrentlyAttacking;
    }

    public void DoAttack()
    {
        pm.StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        Debug.Log("AttackCoroutine 시작!");
        isAttacking = true;
        hitEnemies.Clear();
        timeSinceAttack = 0f;

        bool isAir = !pm.isGrounded;
        string animationTrigger = isAir ? "AttackJP" : "Attack" + (attackCount + 1);
        pm.GetAnimator().SetTrigger(animationTrigger);

        // 💥 사운드 재생 (공격 즉시)
        if (!isAir)
        {
            AudioClip clipToPlay = (attackCount == 2) ? pm.attackSFX3 : pm.attackSFX1;
            if (clipToPlay != null)
                SoundManager.Instance.PlaySFX(clipToPlay);
        }

        if (!isAir)
        {
            attackCount++;
            if (attackCount >= 3)
                attackCount = 0;

            // 지상에서만 전진 이동
            Vector2 direction = pm.spriteRenderer.flipX ? Vector2.left : Vector2.right;
            float elapsed = 0f;
            float moveDuration = pm.data.attackForwardDuration;

            while (elapsed < moveDuration)
            {
                Vector2 velocity = pm.rb.linearVelocity;
                velocity.x = direction.x * pm.data.attackForwardSpeed;
                pm.rb.linearVelocity = velocity;

                elapsed += Time.deltaTime;
                yield return null;
            }

            pm.rb.linearVelocity = new Vector2(0f, pm.rb.linearVelocity.y); // x축 멈춤
        }

        PerformAttack();

        float remain = Mathf.Max(0f, pm.data.attackDuration - pm.data.attackForwardDuration);
        if (remain > 0f)
            yield return new WaitForSeconds(remain);

        isAttacking = false;
    }

    private void PerformAttack()
    {
        float knockback = (attackCount == 3) ? pm.data.attackKnockbackThird : pm.data.attackKnockback;
        float damage = (attackCount == 3) ? pm.data.attackPower * 1.5f : pm.data.attackPower;

        Vector3 pos = pm.attackPos.position;
        int enemyLayerMask = LayerMask.GetMask("Enemy");
        Collider2D[] colliders = Physics2D.OverlapBoxAll(pos, pm.data.attackBoxSize, 0, enemyLayerMask);

        bool hitSomething = false;

        foreach (Collider2D col in colliders)
        {
            if (hitEnemies.Contains(col)) continue;

            GameObject target = col.gameObject;
            hitSomething = true;

            if (NetworkClient.Instance != null && NetworkClient.Instance.isConnected)
            {
                if (NetworkClient.Instance != null && NetworkClient.Instance.isConnected)
                {
                    TrapVisual tv = target.GetComponent<TrapVisual>();
                    if (tv != null && !string.IsNullOrEmpty(tv.trapId))  // trapId가 있다고 가정
                    {
                        NetworkCombatManager.SendTrapDamage(tv.trapId, (int)damage);
                       // Debug.Log($"트랩 데미지 전송: {damage} to trap {tv.trapId}");
                    }
                    else
                    {
                        NetworkCombatManager.SendMonsterDamage((int)damage);
                       // Debug.Log($"몬스터 데미지 전송: {damage}");
                    }
                }

            }
            else
            {
                CombatManager.ApplyDamage(target, damage, knockback, pm.transform.position, staggerDamage);
            }

            if (pm.hitEffectPrefab != null)
            {
                Vector3 hitPos = col.bounds.center; // 적 중심 위치

                // ▶ x 방향: 플레이어 방향 기준 0.5 ~ 1.5f 앞쪽
                float xOffset = 1f;
                if (pm.spriteRenderer.flipX)
                    xOffset *= -1f;

                // ▶ y 방향: 살짝 위아래 -0.2 ~ 0.2f
                float yOffset = UnityEngine.Random.Range(-0.5f, 0f);

                hitPos.x += xOffset;
                hitPos.y += yOffset;

                GameObject effect = GameObject.Instantiate(pm.hitEffectPrefab, hitPos, Quaternion.identity);
                GameObject.Destroy(effect, 0.5f);
            }

            hitEnemies.Add(col);
        }

        // ▶ 적을 맞췄을 때만 카메라 흔들림
        if (hitSomething)
        {
            if (attackCount == 3)
                pm.cameraController.Shake(0.2f, 0.1f);
            else
                pm.cameraController.Shake(0.1f, 0.2f);
        }
    }


    public void UpdateAttackPosition()
    {
        Vector3 offset = pm.data.attackBoxOffset;
        if (!pm.spriteRenderer.flipX)
            offset.x *= -1;

        pm.attackPos.localPosition = offset;
    }

    public void DrawGizmos()
    {
        if (pm == null || pm.attackPos == null) return;

        Gizmos.color = Color.cyan;
        Vector3 pos = pm.attackPos.position;
        Gizmos.DrawWireCube(pos, pm.data.attackBoxSize);
    }
}

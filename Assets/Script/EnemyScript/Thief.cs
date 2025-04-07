using System.Collections;
using UnityEngine;

public class Thief : Enemy
{
    protected bool isAttacking = false;

    protected override void Start()
    {
        base.Start();
        SetEnemyStatus("도적", 120, 10, 4);
    }

    protected override void DetectAndChasePlayer()
    {
        if (player == null || isInDamageState || isEnemyDead) return;

        PlayerManager playerManager = player.GetComponent<PlayerManager>();
        if (playerManager != null && playerManager.IsDead)
        {
            isChasing = false;
            anim.SetBool("isWalk", false);
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            LookAtPlayer();

            if (distanceToPlayer <= 1.2f) // 근거리 공격 범위
            {
                rigid.linearVelocity = Vector2.zero;
                anim.SetBool("isWalk", false);

                if (!isAttacking)
                    StartCoroutine(MeleeAttack());
            }
            else if (!isAttacking)
            {
                anim.SetBool("isWalk", true);
                Vector3 direction = (player.position - transform.position).normalized;
                transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            }

            if (!isChasing) SpawnMark();
            isChasing = true;
        }
        else
        {
            Patrol();
            isChasing = false;
        }
    }

    protected IEnumerator MeleeAttack()
    {
        isAttacking = true;
        anim.SetTrigger("attack");

        yield return new WaitForSeconds(0.3f); // 애니메이션 타이밍 고려 (타격 타이밍)

        PlayerManager playerManager = player.GetComponent<PlayerManager>();
        if (playerManager != null && !playerManager.IsDead)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer <= 1.2f)
            {
                playerManager.playerHealth.TakeDamage(atkDmg);
                Debug.Log($"{enemyName} 근거리 공격: {atkDmg} 데미지");
            }
        }

        yield return new WaitForSeconds(0.5f); // 쿨타임
        isAttacking = false;
    }

    protected override void Patrol()
    {
        if (Vector2.Distance(transform.position, patrolTarget) < 0.2f || patrolTimer >= maxPatrolTime)
        {
            patrolTimer = 0f;
            SetPatrolTarget();
        }
        else
        {
            patrolTimer += Time.deltaTime;
        }

        if (!IsOnPlatform())
        {
            Vector3 reverseDirection = (transform.position - patrolTarget).normalized;
            patrolTarget = transform.position + reverseDirection * patrolRange;
            rigid.linearVelocity = Vector2.zero;
        }

        float adjustedMoveSpeed = moveSpeed * 0.7f;
        transform.position = Vector2.MoveTowards(transform.position, patrolTarget, adjustedMoveSpeed * Time.deltaTime);
        anim.SetBool("isWalk", true);
        LookAtPatrolTarget();
    }

    public override void TakeDamage(ParameterPlayerAttack argument)
    {
        if (isTakingDamage || anim.GetBool("isDead")) return;

        isTakingDamage = true;
        nowHp -= argument.damage;

        anim.SetTrigger("hit");

        if (nowHp <= 0)
        {
            HandleWhenDead();
            return;
        }

        isInDamageState = true;
        Vector2 knockbackDirection = (transform.position - player.position).normalized;
        rigid.linearVelocity = Vector2.zero;
        rigid.AddForce(knockbackDirection * argument.knockback, ForceMode2D.Impulse);

        Invoke("ResumeChase", 0.5f);
        StartCoroutine(EndDamage());
    }
}

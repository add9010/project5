using System.Collections;
using UnityEngine;

public class Wizard : Enemy
{
    public GameObject projectilePrefab;     // 마법 투사체 프리팹
    public Transform firePoint;             // 발사 위치
    public float attackCooldown = 2f;       // 공격 간격
    public float attackRange = 4f;          // 마법 사거리
    protected bool isAttacking = false;

    protected override void Start()
    {
        SetEnemyStatus("마법사", 100, 15, 3); // 마법사 전용 스탯 설정

        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

       // hpBar = Instantiate(prfHpBar, canvas.transform).GetComponent<RectTransform>();
        // nowHpbar = hpBar.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null) Debug.LogError("Player object not found!");
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

            if (distanceToPlayer <= attackRange)
            {
                rigid.linearVelocity = Vector2.zero;
                anim.SetBool("isWalk", false);

                if (!isAttacking)
                    StartCoroutine(RangedAttack());
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

    protected IEnumerator RangedAttack()
    {
        isAttacking = true;
        anim.SetTrigger("attack");

        yield return new WaitForSeconds(0.5f);

        if (projectilePrefab != null && firePoint != null && !isEnemyDead)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Vector2 dir = (player.position - firePoint.position).normalized;
            projectile.GetComponent<Rigidbody2D>().linearVelocity = dir * 5f;
            if (dir.x < 0)
            {
                Vector3 scale = projectile.transform.localScale;
                scale.x = -Mathf.Abs(scale.x); // 왼쪽으로 쏠 때 반전
                projectile.transform.localScale = scale;
            }
            else
            {
                Vector3 scale = projectile.transform.localScale;
                scale.x = Mathf.Abs(scale.x); // 오른쪽이면 원래 방향
                projectile.transform.localScale = scale;
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }


    protected override void HandleWhenDead()
    {
        base.HandleWhenDead();
        // 마법사 전용 사망 연출 또는 드랍 처리 가능
    }
}

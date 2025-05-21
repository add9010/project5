using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class Enemy : MonoBehaviour, IDamageable, IKnockbackable
{
    [Header("Enemy Stats")]
    public string enemyName = "Enemy";
    public float maxHp = 100f;
    public float atkDmg = 10f;
    public float moveSpeed = 3f;
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 0f;
    public float patrolRange = 2f;
    public bool enablePatrol = true;
    [Header("References")]
    public GameObject markPrefab;
    public float markYOffset = 2.0f;

    public float attackHitDelay = 0.5f; // 공격 히트 딜레이 (애니메이션과 맞춰야 함)

    [HideInInspector] public Animator anim;
    [HideInInspector] public Rigidbody2D rigid;
    [HideInInspector] public Transform player;

    protected Vector2 patrolTarget;
    protected float patrolTimer;
    public float maxPatrolTime = 3f;

    protected IEnemyState currentState;

    private float lastAttackTime = -999f;
    public bool CanAttack() => Time.time - lastAttackTime >= attackCooldown;
    public void RecordAttackTime() => lastAttackTime = Time.time;
    private bool isEnemyDead = false;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    protected virtual void Start()
    {
        SetEnemyStatus(enemyName, maxHp, atkDmg, moveSpeed);
        patrolTimer = 0f;

        SwitchState(enablePatrol ? new PatrolState() : new IdleState());
        IgnoreEnemyCollisions();
    }

    protected virtual void Update()
    {
        if (!(currentState is AttackState)
       && !(currentState is HurtState)   // ← 이 줄 추가
       && IsPlayerInAttackRange()
       && CanAttack())
        {
            SwitchState(new AttackState());
            return;
        }

        currentState?.Update(this);
    }

    public void SwitchState(IEnemyState newState)
    {
        currentState?.Exit(this);
        currentState = newState;
        currentState.Enter(this);
    }

    public void SetEnemyStatus(string name, float hp, float atk, float speed)
    {
        enemyName = name;
        maxHp = hp;
        nowHp = hp;
        atkDmg = atk;
        moveSpeed = speed;
    }

    // === 상태 검사 ===
    public bool IsPlayerDetected() => player && Vector2.Distance(transform.position, player.position) <= detectionRange;
    public bool IsPlayerInAttackRange() => player && Vector2.Distance(transform.position, player.position) <= attackRange;

    // === 이동 관련 ===
    public void Patrol()
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

        transform.position = Vector2.MoveTowards(transform.position, patrolTarget, moveSpeed * 0.7f * Time.deltaTime);
        LookAt(patrolTarget);
    }

    public void SetPatrolTarget()
    {
        float randX = Random.Range(-patrolRange, patrolRange);
        patrolTarget = new Vector2(transform.position.x + randX, transform.position.y);
    }

    public void MoveToPlayer()
    {
        if (player == null) return;

        // 현재 Y 유지, X만 플레이어 쪽으로
        Vector2 current = rigid.position;
        Vector2 target = new Vector2(player.position.x, current.y);
        Vector2 nextPos = Vector2.MoveTowards(
            current,
            target,
            moveSpeed * Time.fixedDeltaTime);

        rigid.MovePosition(nextPos);
        LookAt(new Vector2(player.position.x, current.y));
    }

    public void LookAt(Vector2 target)
    {
        if (target.x > transform.position.x)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else
            transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    public void StopMovement()
    {
        rigid.linearVelocity = Vector2.zero;
    }

    // === 상태 처리 ===
    protected float nowHp;

    public void TakeDamage(float damage)
    {
        nowHp -= damage;
        if (nowHp <= 0)
        {
            SwitchState(new DeadState());
        }
        else
        {
            SwitchState(new HurtState());
        }
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        rigid.linearVelocity = Vector2.zero;
        rigid.AddForce(direction * force, ForceMode2D.Impulse);
    }

    public void ReturnToDefaultState()
    {
        SwitchState(enablePatrol ? new PatrolState() : new IdleState());
    }

    public void HandleWhenDead()
    {
        if (isEnemyDead) return;
        isEnemyDead = true;

        anim.SetBool("isDead", true);

        StartCoroutine(HandleDeathCoroutine(0.8f));
    }
    private IEnumerator HandleDeathCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        Die();
    }
    public void Die()
    {
        gameObject.SetActive(false);
        // 사망 시 부모 맵에 알림-0521추가 맵안에 있는 적이 다 죽어야 출구가 열림
        MapEnemyController map = GetComponentInParent<MapEnemyController>();
        if (map != null)
        {
            map.OnEnemyDied();
        }
    }

    public virtual void PerformAttack()
    {
        // 개별 적에서 오버라이드 (Thief, Wizard 등)
        Debug.Log($"{enemyName} 공격 실행");
    }

    public void SpawnMark()
    {
        if (markPrefab != null)
        {
            Vector3 pos = transform.position + new Vector3(0, markYOffset, 0);
            GameObject marker = Instantiate(markPrefab, pos, Quaternion.identity);
            Mark markScript = marker.GetComponent<Mark>();
            if (markScript != null)
            {
                markScript.enemy = transform;
            }
        }
    }

    private void IgnoreEnemyCollisions()
    {
        Collider2D myCol = GetComponent<Collider2D>();
        foreach (Enemy other in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            if (other != this)
            {
                Collider2D col = other.GetComponent<Collider2D>();
                if (col) Physics2D.IgnoreCollision(myCol, col);
            }
        }
    }

    // Enemy.cs 내부에 추가
    public bool IsParryWindow { get; private set; } = false;

    public void SetParryWindow(bool isOpen)
    {
        IsParryWindow = isOpen;
#if UNITY_EDITOR
        if (isOpen) Debug.Log($"{enemyName}: 패링 타이밍 진입");
#endif
    }

    // 상태 확인 메서드도 추가
    public bool IsParryable()
    {
        return currentState is AttackState;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }


}

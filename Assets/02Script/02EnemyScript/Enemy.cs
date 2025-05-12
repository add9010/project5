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
    public float attackCooldown = 1f;
    public float patrolRange = 2f;
    public bool enablePatrol = true;

    [Header("References")]
    public GameObject markPrefab;
    public float markYOffset = 2.0f;

    [HideInInspector] public Animator anim;
    [HideInInspector] public Rigidbody2D rigid;
    [HideInInspector] public Transform player;

    protected Vector2 patrolTarget;
    protected float patrolTimer;
    public float maxPatrolTime = 3f;

    protected IEnemyState currentState;

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
        if (!enablePatrol
       && !(currentState is AttackState)
       && IsPlayerInAttackRange())
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
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        LookAt(player.position);
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

    public void Die()
    {
        gameObject.SetActive(false);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}

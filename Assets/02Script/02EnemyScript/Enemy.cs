using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class Enemy : MonoBehaviour, IDamageable, IKnockbackable
{
    #region ▒ Inspector에 노출되는 필드 ▒
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

    [Header("Stagger")]
    public float maxStagger = 50f;
    public float staggerRecoverTime = 1.5f;
    public float stunDuration = 3f;

    public float attackHitDelay = 0.5f; // 공격 히트 딜레이 (애니메이션과 맞춰야 함)
    #endregion

    #region ▒ Inspector에는 숨기고 코드에서만 쓰는 필드 ▒
    [HideInInspector] public Animator anim;
    [HideInInspector] public Rigidbody2D rigid;
    [HideInInspector] public Transform player;
    #endregion

    #region ▒ 런타임 상태 변수 ▒
    public float currentStagger;
    private float stunTimer;
    //private bool isStunned = false;

    protected bool isChasing = false;
    protected bool isInDamageState = false;

    private float nowHp;
    private bool isEnemyDead = false;

    protected Vector2 patrolTarget;
    protected float patrolTimer;
    public float maxPatrolTime = 5f;

    private float lastAttackTime = -999f;

    protected IEnemyState currentState;
    #endregion

    #region ▒ Unity 콜백 ▒
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
        currentStagger = maxStagger;

        SwitchState(enablePatrol ? new PatrolState() : new IdleState());
        IgnoreEnemyCollisions();
    }

    protected virtual void Update()
    {
        // 스턴 처리
        if (currentState is StaggerState)
        {
            currentState.Update(this);
            return;
        }

        // 공격 상태 전환
        if (!(currentState is AttackState)
            && !(currentState is HurtState)
            && IsPlayerInAttackRange()
            && CanAttack())
        {
            SwitchState(new AttackState());
            return;
        }

        // 감지 & 추적
        DetectAndChasePlayer();

        // 상태 업데이트
        currentState?.Update(this);
    }
    #endregion

    #region ▒ 상태 머신 관리 ▒
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
    #endregion

    #region ▒ 감지 & 추적 ▒
    protected void LookAtPlayer()
    {
        Vector3 dir = player.position - transform.position;
        transform.rotation = dir.x > 0
            ? Quaternion.Euler(0, 0, 0)
            : Quaternion.Euler(0, 180, 0);
    }

    protected virtual void DetectAndChasePlayer()
    {
        if (player == null || isInDamageState) return;
        float dist = Vector2.Distance(transform.position, player.position);

        if (!enablePatrol && dist <= detectionRange)
            enablePatrol = true;

        if (dist <= detectionRange)
        {
            if (!isChasing) SpawnMark();
            isChasing = true;

            if (!IsOnPlatform())
            {
                StopMovement();
                Debug.Log("플랫폼 이탈 감지");
            }
            else
            {
                HandleChaseBehavior(dist);
            }

            LookAtPlayer();
        }
        else
        {
            if (enablePatrol)
            {
                Patrol();
            }
            else
            {
                StopMovement();
                anim.SetBool("isWalk", false);
            }
            isChasing = false;
        }
    }

    protected virtual void HandleChaseBehavior(float distanceToPlayer)
    {
        anim.SetBool("isWalk", true);
        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime);
        Debug.Log("플레이어 추적 중");
    }

    public bool IsPlayerDetected() => player && Vector2.Distance(transform.position, player.position) <= detectionRange;
    public bool IsPlayerInAttackRange() => player && Vector2.Distance(transform.position, player.position) <= attackRange;
    protected bool IsOnPlatform() => true;  // TODO: 실제 로직
    #endregion

    #region ▒ 이동 ▒
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

        transform.position = Vector2.MoveTowards(
            transform.position,
            patrolTarget,
            moveSpeed * 0.7f * Time.deltaTime);
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

        Vector2 current = rigid.position;
        Vector2 target = new Vector2(player.position.x, current.y);
        Vector2 next = Vector2.MoveTowards(
            current, target,
            moveSpeed * Time.fixedDeltaTime);

        rigid.MovePosition(next);
        LookAt(new Vector2(player.position.x, current.y));
    }

    public void LookAt(Vector2 target)
    {
        transform.rotation = (target.x > transform.position.x)
            ? Quaternion.Euler(0, 0, 0)
            : Quaternion.Euler(0, 180, 0);
    }

    public void StopMovement()
    {
        rigid.linearVelocity = Vector2.zero;
    }

    public void SpawnMark()
    {
        if (markPrefab == null) return;
        Vector3 pos = transform.position + Vector3.up * markYOffset;
        var m = Instantiate(markPrefab, pos, Quaternion.identity);
        if (m.TryGetComponent<Mark>(out var mark)) mark.enemy = transform;
    }
    #endregion

    #region ▒ 공격 ▒
    public virtual void PerformAttack()
    {
        Debug.Log($"{enemyName} 공격 실행");
    }

    public bool CanAttack() => Time.time - lastAttackTime >= attackCooldown;
    public void RecordAttackTime() => lastAttackTime = Time.time;
    #endregion

    #region ▒ 피해 & 경직 ▒
    public void TakeDamage(float damage)
    {
        nowHp -= damage;
        if (nowHp <= 0)
            SwitchState(new DeadState());
        else
            SwitchState(new HurtState());
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        rigid.linearVelocity = Vector2.zero;
        rigid.AddForce(direction * force, ForceMode2D.Impulse);
    }

    public void ReduceStagger(float amount)
    {
        if (currentState is StaggerState) return;

        currentStagger -= amount;
        if (currentStagger <= 0f)
        {
            // StaggerState 진입
            SwitchState(new StaggerState());
        }
    }

    private void EnterStun()
    {
        //isStunned = true;
        stunTimer = stunDuration;
        StopMovement();
    }

    public void ReturnToDefaultState()
    {
        SwitchState(enablePatrol ? new PatrolState() : new IdleState());
    }
    #endregion

    #region ▒ 사망 ▒
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

        var map = GetComponentInParent<MapEnemyController>();
        map?.OnEnemyDied();
    }
    #endregion

    #region ▒ 기타 ▒
    private void IgnoreEnemyCollisions()
    {
        Collider2D myCol = GetComponent<Collider2D>();
        foreach (Enemy other in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            if (other != this && other.TryGetComponent<Collider2D>(out var col))
                Physics2D.IgnoreCollision(myCol, col);
        }
    }

    public bool IsParryWindow { get; private set; } = false;
    public void SetParryWindow(bool isOpen) => IsParryWindow = isOpen;
    public bool IsParryable() => currentState is AttackState;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
    #endregion
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    protected Rigidbody2D rigid;
    protected SpriteRenderer spriteRenderer;
    protected Transform player;
    protected Animator anim;

    protected RectTransform hpBar;
    protected Image nowHpbar;
    public GameObject prfHpBar;
    public GameObject canvas;

    public GameObject markPrefab;
    public float markYOffset = 1f;
    public float height = 1.7f;

    [Header("Enemy Stats")]
    public string enemyName = "Enemy";
    public float maxHp = 100f;
    public float nowHp = 100f;
    public float atkDmg = 10;
    public float moveSpeed = 3f;
    public float detectionRange = 5f;

    protected Vector3 patrolTarget;    // ��ȸ
    public float patrolRange = 2f;
    protected float patrolTimer = 0f;  // 
    public float maxPatrolTime = 3f;   // ��ǥ ������ �������� ���� ���¿��� �ð��� �󸶳� ��ٸ��� ���� (�� ����)



    protected bool isInDamageState = false;
    protected bool isChasing = false;
    protected bool isEnemyDead = false;
    protected bool isTakingDamage = false;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void SetEnemyStatus
        (string _enemyName, float _maxHp, float _atkDmg, float _moveSpeed )
    {
        enemyName = _enemyName;
        maxHp = _maxHp;
        nowHp = _maxHp;
        atkDmg = _atkDmg;
        moveSpeed = _moveSpeed;
    }

    protected virtual void Start()
    {
        // ü�� �� �ʱ�ȭ
        hpBar = Instantiate(prfHpBar, canvas.transform).GetComponent<RectTransform>();
        nowHpbar = hpBar.transform.GetChild(0).GetComponent<Image>();

        SetEnemyStatus("enemyName", maxHp, atkDmg, moveSpeed) ; // �� �ʱ�ȭ

        // �÷��̾� ã��
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null) Debug.LogError("Player object not found!");
    }

    protected virtual void Update()
    {
        // �ǰ� ���°� �ƴϰ�, ���������, �÷��̾ �����Ѵٸ� �߰�
        if (!isInDamageState && nowHp > 0 && player != null)
            DetectAndChasePlayer();


        // ü�� �� ��ġ,���� ����
        if (hpBar != null)
        {
            Vector3 _hpBarPos = Camera.main.WorldToScreenPoint
                (new Vector3(transform.position.x, transform.position.y + height, 0));
            hpBar.position = _hpBarPos;
            nowHpbar.fillAmount = nowHp / maxHp; 
        }
    }

    public virtual void TakeDamage(ParameterPlayerAttack argument)
    {
        if (isTakingDamage || anim.GetBool("isDead")) return;

        isTakingDamage = true;
        nowHp -= argument.damage;

        if (nowHp <= 0)
        {
            HandleWhenDead();
            return;
        }

        // �ǰ� �� �߰� ����
        isInDamageState = true;  // �ǰ� ���·� ��ȯ
        anim.SetBool("isHunt", true);
        Vector2 knockbackDirection = (transform.position - player.position).normalized;
        rigid.linearVelocity = Vector2.zero;  // �˹� ȿ���� ����� ����ǵ��� �ʱ�ȭ
        rigid.AddForce(knockbackDirection * argument.knockback, ForceMode2D.Impulse);  // �˹�

        // 0.5�� �� �߰��� �簳
        Invoke("ResumeChase", 0.5f);

        StartCoroutine(EndDamage());
    }

    private void ResumeChase()
    {
        isInDamageState = false;  // �ǰ� ���� ����
    }

    protected virtual IEnumerator EndDamage()
    {
        yield return new WaitForSeconds(0.5f);
        isTakingDamage = false;
        anim.SetBool("isHunt", false);
    }

    protected virtual void HandleWhenDead()
    {
        isEnemyDead = true;
        nowHp = 0;
        anim.SetBool("isDead", true);

        if (hpBar != null) Destroy(hpBar.gameObject); // ü�� �� UI ����

        // ���� �� �ִϸ��̼� ���̸� Ȯ���� �� ��Ȱ��ȭ ó��
        // float deathAnimationLength = GetAnimationClipLengthByTag("Dead"); // �±װ� Dead�� ������ �ִϸ��̼� ���� ���ϱ�
        StartCoroutine(HandleDeath(0.8f)); // �̺κ� Ȯ�� ��Ź�帲

        Debug.Log($"[{GetType().Name}] {enemyName} is dead.");
    }

    protected virtual IEnumerator HandleDeath(float delay)
    {
        yield return new WaitForSeconds(delay); // �ִϸ��̼� ��� �ð���ŭ ���
        gameObject.SetActive(false); // ������Ʈ ��Ȱ��ȭ
        Debug.Log($"[{GetType().Name}] {enemyName} has died.");
    }

    protected bool IsOnPlatform()
    {
        int platformLayer = LayerMask.GetMask("Platform"); 
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position + new Vector3(0.8f, -1f, 0), Vector2.down, 10f, platformLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position + new Vector3(-0.8f, -1f, 0), Vector2.down, 10f, platformLayer);
        Debug.DrawRay(transform.position + new Vector3(0.8f, -1f, 0), Vector2.down * 10f, Color.red);
        Debug.DrawRay(transform.position + new Vector3(-0.8f, -1f, 0), Vector2.down * 10f, Color.red);

        if (hitRight.collider == null || !hitRight.collider.CompareTag("Platform") ||
            hitLeft.collider == null || !hitLeft.collider.CompareTag("Platform"))
        {
            return false;  // ������ �Ǵ� ���ʿ� �÷����� ������ false
        }
        // �� �� ��� �÷����� ���� ��� true ��ȯ
        return true;
    }

    protected virtual void Patrol()
    {
        // ��ǥ ������ �����ߴ��� Ȯ���ϰ�, �������� ���ϸ� Ÿ�̸� ����
        if (Vector2.Distance(transform.position, patrolTarget) < 0.2f)
        {
            patrolTimer = 0f;  // ��ǥ ������ �����ϸ� Ÿ�̸Ӹ� �ʱ�ȭ
            SetPatrolTarget();  // ���ο� ��ǥ ����
        }
        else
        {
            patrolTimer += Time.deltaTime;  // ��ǥ ������ �������� ���ϸ� Ÿ�̸� ����
        }

        // ��ǥ ������ ���� �ð� ���� �������� ���ϸ� ���ο� ��ǥ ���� ����
        if (patrolTimer >= maxPatrolTime)
        {
            patrolTimer = 0f;  // Ÿ�̸� �ʱ�ȭ
            SetPatrolTarget();  // ���ο� ��ǥ ����
        }

        if (!IsOnPlatform())  // �÷����� ������ �ݴ� �������� �̵�{
        {
            Vector3 reverseDirection = (transform.position - patrolTarget).normalized;
            patrolTarget = transform.position + reverseDirection * patrolRange;
            rigid.linearVelocity = Vector2.zero;  // �̵� ����
        }
        // ��ǥ �������� �̵� (��ȸ �� �̵� �ӵ��� 0.7��� ����)
        float adjustedMoveSpeed = moveSpeed * 0.7f;
        transform.position = Vector2.MoveTowards(transform.position, patrolTarget, adjustedMoveSpeed * Time.deltaTime);
        anim.SetBool("isWalk", true);
        LookAtPatrolTarget();  // �̵� ���⿡ ���� ȸ��
    }

    // ��ȸ�� ��ǥ ��ġ�� �����ϰ� ����
    protected virtual void SetPatrolTarget()
    {
        float randomX = Random.Range(-patrolRange, patrolRange); //x�����θ� ������
        patrolTarget = new Vector2(transform.position.x + randomX, transform.position.y);
    }

    // �̵� ������ �������� �ֳʹ� ȸ�� ����
    protected virtual void LookAtPatrolTarget()
    {
        Vector3 direction = patrolTarget - transform.position;

        if (direction.x > 0)
            transform.rotation = Quaternion.Euler(0, 0, 0); // ������ ����
        else if (direction.x < 0)
            transform.rotation = Quaternion.Euler(0, 180, 0); // ���� ����
    }

    protected virtual void DetectAndChasePlayer()
    {
        if (player == null || isInDamageState) return;  // �ǰ� ������ ��� �߰����� ����

        PlayerManager playerManager = player.GetComponent<PlayerManager>();
        if (playerManager != null && playerManager.IsDead) // �÷��̾� ����� �߰�����
        {
            isChasing = false;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange)
        {
            if (!isChasing) SpawnMark();
            isChasing = true;

            if (!IsOnPlatform())  // ������ ������ �� �̻� �߰����� ����
            {
                rigid.linearVelocity = Vector2.zero;  // �̵� ����
                Debug.Log("���� ��ȸ");
            }
            else  // ������ ������ �߰� ���
            {
                anim.SetBool("isWalk", true);
                Vector3 direction = (player.position - transform.position).normalized;  // �÷��̾ �߰�
                transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
                
                Debug.Log("�÷��̾� ��������");
            }

            LookAtPlayer();
        }
        else
        {
            Patrol();  // �÷��̾ ���� �� ��ȸ
            isChasing = false;
        }
    }

    protected virtual void SpawnMark()
    {
        if (markPrefab != null)
        {
            // ��Ŀ�� ������ ��ġ�� ���� ��ġ���� markYOffset��ŭ Y������ �ø�
            Vector3 spawnPosition = transform.position + new Vector3(0, markYOffset, 0); 
            GameObject markInstance = Instantiate(markPrefab, spawnPosition, Quaternion.identity);

            // ��Ŀ�� Mark ��ũ��Ʈ���� ���� ������ ���
            Mark markScript = markInstance.GetComponent<Mark>();
            if (markScript != null)
            {
                markScript.enemy = transform;  // ���� ���� Transform�� ��Ŀ�� �Ҵ�
            }
        }
    }

    protected virtual void LookAtPlayer()
    {
        Vector3 direction = player.position - transform.position;

        if (direction.x > 0) transform.rotation = Quaternion.Euler(0, 0, 0); // ������ ����
        else transform.rotation = Quaternion.Euler(0, 180, 0); // ���� ���� (Y�� ȸ��)
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    //protected float GetAnimationClipLengthByTag(string tag)
    //{
    //    // ���� Animator ���� ��������
    //    AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0); // Layer 0 ����
    //    if (anim != null && stateInfo.IsTag(tag))
    //    {
    //        // ���� ���°� �±׿� ��ġ�ϸ� �ش� ������ ���̸� ��ȯ
    //        return stateInfo.length;
    //    }

    //    Debug.LogWarning($"Animation with tag '{tag}' not found!");
    //    return 0f; // �±׸� ã�� ������ �� �⺻�� ��ȯ
    //}

}

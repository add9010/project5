using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;
    private RectTransform hpBar;
    private Image nowHpbar;
    private Transform player;
    private Animator anim;

    public GameObject prfHpBar;
    public GameObject canvas;
    public GameObject markPrefab;
    public string enemyName;
    public float maxHp;
    public float nowHp;
    public int atkDmg = 10;
    public float moveSpeed = 3f;
    public float height = 1.7f;
    public float detectionRange = 5f;
    public float markYOffset = 1f;  
    protected Vector3 initialPosition;

    protected bool isChasing = false;
    protected bool isEnemyDead = false;
    protected bool isTakingDamage = false;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        // ü�� �� �ʱ�ȭ
        hpBar = Instantiate(prfHpBar, canvas.transform).GetComponent<RectTransform>();
        nowHpbar = hpBar.transform.GetChild(0).GetComponent<Image>();

        SetEnemyStatus("���� ������", 100, 10); // �� �ʱ�ȭ

        // �÷��̾� ã��
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Player object not found!");
        }

        initialPosition = transform.position;
    }

    protected virtual void Update()
    {
        // ü�� �� ��ġ ����
        Vector3 _hpBarPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + height, 0));
        hpBar.position = _hpBarPos;

        // ü�� �� ���� ����
        if (nowHpbar != null)
        {
            nowHpbar.fillAmount = nowHp / maxHp;
        }

        // �÷��̾� Ž�� �� ����
        if (nowHp > 0)
            DetectAndChasePlayer();
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (isEnemyDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            HeroKnightUsing playerScript = collision.gameObject.GetComponent<HeroKnightUsing>();
            if (playerScript != null && !playerScript.isDead)
            {
                playerScript.TakeDamage(atkDmg);
                Debug.Log($"{enemyName} ����: {atkDmg} ������");
            }
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

        anim.SetBool("isHunt", true);
        Vector2 knockbackDirection = (transform.position - player.position).normalized;
        rigid.velocity = Vector2.zero;
        rigid.AddForce(knockbackDirection * argument.knockback, ForceMode2D.Impulse);
        StartCoroutine(EndDamage());
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
    
        if (hpBar != null)Destroy(hpBar.gameObject); // ü�� �� UI ����

        StartCoroutine(HandleDeath());

        Debug.Log($"[{GetType().Name}] {enemyName} is dead."); // Ŭ���� �̸��� �� �̸� ���
    }

    protected virtual IEnumerator HandleDeath()
    {
        // ���� �׾��� �� ó���� ����
        Debug.Log($"[{GetType().Name}] {enemyName} has died.");

        // ����: �� ������Ʈ ��Ȱ��ȭ
        gameObject.SetActive(false);

        yield return null; // �ʿ�� ���
    }

    protected virtual void SetEnemyStatus(string _enemyName, int _maxHp, int _atkDmg)
    {
        enemyName = _enemyName;
        maxHp = _maxHp;
        nowHp = _maxHp;
        atkDmg = _atkDmg;
    }

    protected virtual void DetectAndChasePlayer()
    {
        if (player == null) return;

        HeroKnightUsing playerScript = player.GetComponent<HeroKnightUsing>();
        if (playerScript != null && playerScript.isDead)
        {
            isChasing = false;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange)
        {
            if (!isChasing) // �÷��̾ ó�� Ž������ ���� ��ũ�� ��ȯ
            {
                SpawnMark();
            }

            isChasing = true;
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            LookAtPlayer();
        }
        else
        {
            isChasing = false;
        }
    }
    protected virtual void SpawnMark()
    {
        //Debug.Log("������ũ ȣ��");
        if (markPrefab != null)
        {
            // ��Ŀ�� ������ ��ġ�� ���� ��ġ���� markYOffset��ŭ Y������ �ø�
            Vector3 spawnPosition = transform.position + new Vector3(0, markYOffset, 0); // markYOffset ����ŭ Y������ �̵�
            GameObject markInstance = Instantiate(markPrefab, spawnPosition, Quaternion.identity);

            // ������ ��Ŀ�� ���ʹ̸� �����ϵ��� ����
            Mark markScript = markInstance.GetComponent<Mark>();
            if (markScript != null)
            {
                markScript.enemy = transform; // ��Ŀ�� ���ʹ̸� �����ϵ��� ����
            }
        }
    }

   


    protected virtual void LookAtPlayer()
    {
        Vector3 direction = player.position - transform.position;

        // ���⸸ ȸ���ϵ��� ����
        if (direction.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0); // ������ ����
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0); // ���� ���� (Y�� ȸ��)
        }
    }


    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}

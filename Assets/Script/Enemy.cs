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
        // 체력 바 초기화
        hpBar = Instantiate(prfHpBar, canvas.transform).GetComponent<RectTransform>();
        nowHpbar = hpBar.transform.GetChild(0).GetComponent<Image>();

        SetEnemyStatus("레드 슬라임", 100, 10); // 적 초기화

        // 플레이어 찾기
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Player object not found!");
        }

        initialPosition = transform.position;
    }

    protected virtual void Update()
    {
        // 체력 바 위치 갱신
        Vector3 _hpBarPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + height, 0));
        hpBar.position = _hpBarPos;

        // 체력 바 상태 갱신
        if (nowHpbar != null)
        {
            nowHpbar.fillAmount = nowHp / maxHp;
        }

        // 플레이어 탐지 및 추적
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
                Debug.Log($"{enemyName} 공격: {atkDmg} 데미지");
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
    
        if (hpBar != null)Destroy(hpBar.gameObject); // 체력 바 UI 삭제

        StartCoroutine(HandleDeath());

        Debug.Log($"[{GetType().Name}] {enemyName} is dead."); // 클래스 이름과 적 이름 출력
    }

    protected virtual IEnumerator HandleDeath()
    {
        // 적이 죽었을 때 처리할 로직
        Debug.Log($"[{GetType().Name}] {enemyName} has died.");

        // 예시: 적 오브젝트 비활성화
        gameObject.SetActive(false);

        yield return null; // 필요시 대기
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
            if (!isChasing) // 플레이어를 처음 탐지했을 때만 마크를 소환
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
        //Debug.Log("스폰마크 호출");
        if (markPrefab != null)
        {
            // 마커를 생성할 위치를 적의 위치에서 markYOffset만큼 Y축으로 올림
            Vector3 spawnPosition = transform.position + new Vector3(0, markYOffset, 0); // markYOffset 값만큼 Y축으로 이동
            GameObject markInstance = Instantiate(markPrefab, spawnPosition, Quaternion.identity);

            // 생성된 마커가 에너미를 추적하도록 설정
            Mark markScript = markInstance.GetComponent<Mark>();
            if (markScript != null)
            {
                markScript.enemy = transform; // 마커가 에너미를 추적하도록 설정
            }
        }
    }

   


    protected virtual void LookAtPlayer()
    {
        Vector3 direction = player.position - transform.position;

        // 방향만 회전하도록 설정
        if (direction.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0); // 오른쪽 방향
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0); // 왼쪽 방향 (Y축 회전)
        }
    }


    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}

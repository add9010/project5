using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 씬을 다시 로드하기 위해 추가


public class HeroKnightUsing : MonoBehaviour
{
    public static HeroKnightUsing singleton;
    public bool isDead = false;
    private bool m_canDoubleJump = false; // 더블 점프 가능 여부
    private bool m_canPerformDoubleJump = false;

    [Header("속성")]
    [SerializeField] float m_attackPower = 10.0f;    // 공격력
    [SerializeField] float m_speed = 4.0f;           // 이동 속도
    [SerializeField] float m_jumpForce = 7.5f;       // 점프 힘
    [SerializeField] float m_rollForce = 6.0f;       // 구르기 힘
    [SerializeField] float m_attackKnockback = 8.0f;       // 플레이어가 몬스터 공격 시 넉백
    [SerializeField] float m_attackKnockbackThird = 800.0f;       // 플레이어가 몬스터 공격 시 넉백(3타 공격)
    [SerializeField] GameObject m_slideDust;         // 슬라이딩 먼지 이펙트

    private Animator m_animator;                      // 애니메이터
    private Rigidbody2D m_body2d;                     // 물리 엔진
    private Sensor_HeroKnight m_groundSensor;         // 바닥 센서
    private Sensor_HeroKnight[] m_wallSensors = new Sensor_HeroKnight[4]; // 벽 센서 배열

    private bool m_isWallSliding = false;             // 벽에 붙어 있는지 여부
    private bool m_grounded = false;                  // 바닥에 닿아 있는지 여부
    private bool m_rolling = false;                   // 구르고 있는지 여부
    private int m_facingDirection = 1;                // 캐릭터가 보는 방향
    private int m_currentAttack = 1;                  // 현재 공격 상태 (콤보)
    private int m_attackCount = 0;
    private float m_timeSinceAttack = 0.0f;           // 공격 후 시간
    private float m_delayToIdle = 0.0f;               // 아이들로 돌아가는 딜레이
    private float m_rollDuration = 8.0f / 14.0f;      // 구르기 지속 시간
    private float m_rollCurrentTime = 0.0f;           // 구르기 현재 시간


    private bool m_isAttacking = false;               // 공격 중인지 여부
    private HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>(); // 타격된 적 추적


    public Transform pos;                             // 공격 위치
    public Vector2 boxSize;                           // 공격 범위 크기
    public float attackDuration = 0.2f;               // 공격 속도

    RectTransform hpBar;
    Image nowHpbar;
    public GameObject prfHpBar;
    public GameObject canvas;
    public float height = 1.7f;

    [Header("체력 및 피해")]
    [SerializeField] private float maxHealth = 100;      // 최대 체력
    private float currentHealth;                         // 현재 체력

    //[SerializeField] private float invincibilityDuration = 1.0f; // 무적 시간
    //private bool isInvincible = false;                 // 무적 상태 여부
    //private float invincibilityTimer = 0.0f;           // 무적 시간 타이머

    [SerializeField] private CameraShake cameraShake;



    void Start()
    {
        singleton = this;

        currentHealth = maxHealth;
        // 필요한 컴포넌트 초기화
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();


        // 벽 센서 초기화
        for (int i = 0; i < 4; i++)
        {
            m_wallSensors[i] = transform.Find("WallSensor_" + (i < 2 ? "R" : "L") + (i % 2 + 1)).GetComponent<Sensor_HeroKnight>();
        }

        // 체력 바 초기화 (각 적에 대해 독립적으로 생성)
        hpBar = Instantiate(prfHpBar, canvas.transform).GetComponent<RectTransform>();

        // 체력 바의 이미지 초기화
        nowHpbar = hpBar.transform.GetChild(0).GetComponent<Image>();

        // 초기화된 값 확인 (디버깅용)
        //Debug.Log($"Enemy Name: {enemyName}, Max HP: {maxHp}, Current HP: {nowHp}");

    }

    void Update()
    {

        if (isDead) { return; } // 플레이어가 죽으면 아래 코드 실행하지 않음

        // 체력 바 위치 갱신
        Vector3 _hpBarPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + height, 0));
        hpBar.position = _hpBarPos;


        if (nowHpbar != null) // nowHpbar가 null이 아닌 경우에만 갱신
        {
            nowHpbar.fillAmount = (float)currentHealth / (float)maxHealth;
        }

       
       

        // 타이머 업데이트
        m_timeSinceAttack += Time.deltaTime; // 공격 후 경과 시간 갱신
        if (m_rolling)
        {
            m_rollCurrentTime += Time.deltaTime;
            if (m_rollCurrentTime > m_rollDuration)
                m_rolling = false;
        }
        // 바닥에 닿았는지 체크
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        HandleAnimations(); // 애니메이션 처리
    }
    private void FixedUpdate()
    {
        HandleMovement(); // 이동 처리

        // 구르기 상태가 끝났을 때 충돌을 다시 활성화
        if (m_rolling)
        {
            m_rollCurrentTime += Time.deltaTime;
            if (m_rollCurrentTime >= m_rollDuration)
            {
                m_rolling = false; // 구르기 종료
                IgnoreEnemyCollisions(false); // 구르기 끝난 후 충돌 다시 활성화
            }
        }
    }


    // 속성 값 변경하는 메소드
    public void SetCharacterAttribute(string attribute)
    {
        if (attribute == "speed")
        {
            m_speed *= 1.3f; // 이동 속도 1.3배 증가
            Debug.Log("이동 속도 1.3배 증가");
        }
        else if (attribute == "attack")
        {
            m_attackPower *= 1.5f; // 공격력 1.3배 증가
            Debug.Log("공격력 1.5배 증가");
        }
        else if (attribute == "health")
        {
            maxHealth *= 1.3f; // 최대 체력 1.3배 증가
            currentHealth = maxHealth; // 현재 체력도 최대 체력에 맞춰 조정
            Debug.Log("체력 1.3배 증가");
        }
        else if(attribute =="random")
        {
            m_canDoubleJump = true;
            Debug.Log("축하합니다! 더블점프 해금");
        }
    }


    // 데미지를 받는 함수
    public void TakeDamage(float damage)
    {
        if (isDead) { return; }
        // 디버그 메시지 출력
        Debug.Log("아파!");
       // if (isInvincible || currentHealth <= 0) return; // 무적 상태에서는 데미지를 받지 않음

        currentHealth -= damage; // 체력 감소
        m_animator.SetTrigger("Hurt"); // 데미지 애니메이션 트리거

        // 체력이 0 이하일 경우
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;  // 플레이어가 죽었음을 상태로 설정
            Die(); // 사망 처리
        }

        // 디버그 메시지 출력
        Debug.Log("체력이 " + damage + "만큼 감소, 현재 체력: " + currentHealth);
    }

    // 사망 처리 함수
    public void Die()
    {
       
        if (nowHpbar != null) // nowHpbar가 null이 아닌 경우에만 갱신
        {
            nowHpbar.fillAmount = (float)currentHealth / (float)maxHealth;
        }
        m_animator.SetTrigger("Death"); // 사망 애니메이션 트리거
        Debug.Log("캐릭터가 사망했습니다.");

        // 사망 애니메이션이 끝날 때까지 기다린 후 씬을 다시 로드
        StartCoroutine(WaitForDeathAnimation());
    }

    // 사망 애니메이션 후 기다리는 코루틴
    private IEnumerator WaitForDeathAnimation()
    {
        // 사망 애니메이션의 길이를 가져오기
        float deathAnimationDuration = 3.0f; // 예시로 3초 설정

        // 애니메이션이 끝날 때까지 기다림
        yield return new WaitForSeconds(deathAnimationDuration);

        // 게임 오버 후 씬을 처음부터 다시 시작
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 현재 씬을 다시 로드
        Debug.Log("다시 모험을 떠나요!!");
    }



    private void IgnoreEnemyCollisions(bool ignore) 
    {
        // 모든 적과의 충돌 무시 / 활성화 처리
        Collider2D[] enemies = Physics2D.OverlapBoxAll(transform.position, boxSize, 0);
        foreach (Collider2D enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                if (ignore)
                {
                    Physics2D.IgnoreCollision(GetComponent<Collider2D>(), enemy, true);
                }
                else
                {
                    Physics2D.IgnoreCollision(GetComponent<Collider2D>(), enemy, false);
                }
            }
        }
    }

    private void HandleMovement() // 구르기
    {
        if (isDead) { return; } // 플레이어가 죽으면 아래 코드 실행하지 않음
        float inputX = Input.GetAxis("Horizontal");

        // 구르기 중에 y축 속도를 0으로 고정
        if (m_rolling)
        {
            m_body2d.linearVelocity = new Vector2(m_facingDirection * m_rollForce * 3, 0f); // y축 속도 0으로 설정

            // 구르기 중에 적 위로 올라가는지 확인
            Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, boxSize, 0);
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Enemy"))
                {
                    // 적과 충돌한 경우 구르기를 종료
                    m_rolling = false;
                    m_rollCurrentTime = m_rollDuration; // 구르기를 즉시 끝내기 위해 타이머 설정
                    break;
                }
            }
        }
        else
        {
            m_body2d.linearVelocity = new Vector2(inputX * m_speed, m_body2d.linearVelocity.y); // 일반 이동 처리
        }

        // 공중에 있을 때의 애니메이션 처리
        m_animator.SetFloat("AirSpeedY", m_body2d.linearVelocity.y);
    }

    // 애니메이션 처리 (방향 고정)
    private void HandleAnimations()
    {

        // 벽에 붙어 있는지 체크
        m_isWallSliding = (m_wallSensors[0].State() && m_wallSensors[1].State()) || (m_wallSensors[2].State() && m_wallSensors[3].State());
        m_animator.SetBool("WallSlide", m_isWallSliding);

        // 공격, 구르기, 점프 등 애니메이션 트리거 처리
        if (Input.GetKeyDown("e") && !m_rolling) m_animator.SetTrigger("Death");
        else if (Input.GetKeyDown("q") && !m_rolling) m_animator.SetTrigger("Hurt");
        else if (Input.GetMouseButtonDown(0) && m_timeSinceAttack > attackDuration && !m_rolling && !m_isAttacking) // 공격 쿨타임 추가
        {
            m_attackCount++;
            StartCoroutine(AttackCoroutine()); // 공격 코루틴 호출
        }
        else if (Input.GetMouseButtonDown(1) && !m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }
        else if (Input.GetMouseButtonUp(1)) m_animator.SetBool("IdleBlock", false);
        // 구르기 입력 시
        else if (Input.GetKeyDown("left shift") && !m_rolling && !m_isWallSliding && m_grounded)
        {
            // 땅에 닿아 있을 때만 구르기 가능
            m_rolling = true;
            m_animator.SetTrigger("Roll");

            // 구르기 이동 속도 및 거리 조정 (기존 m_rollForce 값을 크게 설정)
            float enhancedRollSpeed = m_facingDirection * m_rollForce * 3;  // 기존보다 1.5배 더 빨리 이동
            m_body2d.linearVelocity = new Vector2(enhancedRollSpeed, m_body2d.linearVelocity.y);

            // 구르기 지속 시간 늘리기 (기존 시간을 조금 늘려서 구를 때 더 멀리 이동)
            m_rollDuration = 0.25f; // 기존보다 약간 더 길게 (예시로 0.25초로 설정)
            m_rollCurrentTime = 0.0f;
        }
        else if (Input.GetKeyDown("space") && (m_grounded || m_canPerformDoubleJump) && !m_rolling)
        {
            m_animator.SetTrigger("Jump");

            if (m_grounded)
            {
                // 첫 번째 점프
                m_grounded = false;  // 바닥에 있지 않다고 설정
                m_animator.SetBool("Grounded", m_grounded);
                m_canPerformDoubleJump = m_canDoubleJump;  // 더블 점프 가능 여부를 갱신
            }
            else if (m_canPerformDoubleJump)  // 더블 점프 실행
            {
                m_canPerformDoubleJump = false;  // 더블 점프는 한 번만 가능
            }

            // 점프 동작
            m_body2d.linearVelocity = new Vector2(m_body2d.linearVelocity.x, m_jumpForce);

            // 바닥 센서를 약간 비활성화 (0.2초 동안)
            m_groundSensor.Disable(0.2f);
        }
        else if (Mathf.Abs(Input.GetAxis("Horizontal")) > Mathf.Epsilon)
        {
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }
        else
        {
            m_delayToIdle -= Time.deltaTime;
            if (m_delayToIdle < 0) m_animator.SetInteger("AnimState", 0);
        }

        // 공격 중에는 방향을 고정
        if (m_isAttacking)
        {
            // 공격 중에 방향을 고정하고, 반대 방향으로 전환되지 않도록 함
            if (m_facingDirection == 1)
            {
                GetComponent<SpriteRenderer>().flipX = false; // 오른쪽 방향 유지
            }
            else
            {
                GetComponent<SpriteRenderer>().flipX = true; // 왼쪽 방향 유지
            }
        }
        else
        {
            // 공격이 끝나면 방향 전환을 허용
            float inputX = Input.GetAxis("Horizontal");

            // 방향 전환 처리
            if (inputX > 0)
            {
                GetComponent<SpriteRenderer>().flipX = false;
                m_facingDirection = 1;
            }
            else if (inputX < 0)
            {
                GetComponent<SpriteRenderer>().flipX = true;
                m_facingDirection = -1;
            }

            // 구르기 중이 아닐 때만 이동
            if (!m_rolling)
                m_body2d.linearVelocity = new Vector2(inputX * m_speed, m_body2d.linearVelocity.y);
        }

        // 구르기 상태가 끝났을 때 충돌을 다시 활성화
        if (m_rolling && m_rollCurrentTime > m_rollDuration)
        {
            m_rolling = false;
            IgnoreEnemyCollisions(false); // 충돌을 다시 활성화
        }
    }

    private IEnumerator AttackCoroutine()
    {
        m_isAttacking = true;
        hitEnemies.Clear(); // 타격된 적 리스트 초기화
        m_animator.SetTrigger("Attack" + m_currentAttack); // 공격 애니메이션 시작

        // 공격 지속 시간 동안 공격 판정 처리
        float timeElapsed = 0f;

        // 공격 중에 타격 체크
        while (timeElapsed < attackDuration)
        {
            Attack(); // 타겟을 때리기
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        m_isAttacking = false;
        m_currentAttack = (m_currentAttack % 3) + 1; // 공격 콤보 순서 변경

        // 공격 후 쿨타임 추가
        m_timeSinceAttack = 0.0f; // 공격 후 쿨타임 리셋
    }

    private void Attack()
    {
        // 공격 횟수 증가
        ParameterPlayerAttack attackArgument = new ParameterPlayerAttack();
        attackArgument.damage = ((m_attackCount == 3) ? m_attackPower * 1.5f : m_attackPower);
        attackArgument.knockback = ((m_attackCount == 3) ? m_attackKnockbackThird : m_attackKnockback);

        // 3번째 공격마다 카메라 흔들림 발생
        if (m_attackCount == 3)
        {
            Debug.Log($"Attack() : 카메라 셰이크 - {m_attackCount}");

            cameraShake.ShakeCamera(); // 카메라 흔들림 실행
            m_attackCount = 0; // 공격 횟수 초기화
        }

        // 공격 박스 위치 조정
        Vector3 attackBoxPosition = pos.position;
        if (m_facingDirection == -1)
            attackBoxPosition = new Vector3(pos.position.x - boxSize.x, pos.position.y, pos.position.z);

        // 적과 충돌 체크
        Collider2D[] colliders = Physics2D.OverlapBoxAll(attackBoxPosition, boxSize, 0);
        foreach (Collider2D collider in colliders)
        {
            // Enemy 클래스를 상속받은 모든 적을 대상으로 피해 적용
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null) // Enemy를 상속받은 객체만 처리
            {
                enemy.TakeDamage(attackArgument); // 피해 적용
                hitEnemies.Add(collider);        // 타격된 적으로 등록
            }
        }
    }



    private void OnDrawGizmos()
    {
        if (pos != null)
        {
            Gizmos.color = Color.blue;
            Vector3 attackBoxPosition = m_facingDirection == -1 ? new Vector3(pos.position.x - boxSize.x, pos.position.y, pos.position.z) : pos.position;
            Gizmos.DrawWireCube(attackBoxPosition, boxSize); // 공격 범위 시각화
        }
    }
}



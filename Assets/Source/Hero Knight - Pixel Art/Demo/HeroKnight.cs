using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class HeroKnight1 : MonoBehaviour
{
    // [Header] 속성을 이용해 인스펙터에서 보기 좋게 카테고리화
    [Header("속성")]
    [SerializeField] float m_speed = 4.0f;           // 이동 속도
    [SerializeField] float m_jumpForce = 7.5f;       // 점프 힘
    //[SerializeField] float m_rollForce = 6.0f;       // 구르기 힘
    //[SerializeField] bool m_noBlood = false;         // 피 이펙트 활성화 여부
    [SerializeField] GameObject m_slideDust;         // 슬라이딩 시 먼지 이펙트

    // 캐릭터 컴포넌트 및 상태 관련 변수들
    private Animator m_animator;                      // 애니메이터 컴포넌트
    private Rigidbody2D m_body2d;                     // Rigidbody2D 컴포넌트
    private Sensor_HeroKnight m_groundSensor;         // 바닥 감지 센서
    private Sensor_HeroKnight[] m_wallSensors = new Sensor_HeroKnight[4]; // 벽 감지 센서 배열

    // 캐릭터의 현재 상태 플래그 및 상태값
    private bool m_isWallSliding = false;             // 벽 슬라이딩 중인지 여부
    private bool m_grounded = false;                  // 캐릭터가 바닥에 닿아 있는지 여부
    private bool m_rolling = false;                   // 구르기 중인지 여부
    private int m_facingDirection = 1;                // 캐릭터가 보는 방향 (1: 오른쪽, -1: 왼쪽)
    private int m_currentAttack = 1;                  // 공격 콤보 단계
    private float m_timeSinceAttack = 0.0f;           // 마지막 공격 후 경과 시간
    //private float m_delayToIdle = 0.0f;               // Idle 상태로 돌아가는 딜레이
    private float m_rollDuration = 8.0f / 14.0f;      // 구르기 지속 시간
    private float m_rollCurrentTime = 0.0f;           // 현재 구르기 경과 시간

    // 공격 관련 변수
    public Transform pos;                             // 공격 위치
    public Vector2 boxSize;                           // 공격 범위 크기
    private bool m_isAttacking = false;               // 공격 중인지 여부
    private HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>(); // 이미 공격한 적을 추적하여 중복 타격 방지

    void Start()
    {
        // 필수 컴포넌트 초기화
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();

        // 벽 감지 센서를 배열로 초기화
        for (int i = 0; i < 4; i++)
        {
            m_wallSensors[i] = transform.Find("WallSensor_" + (i < 2 ? "R" : "L") + (i % 2 + 1)).GetComponent<Sensor_HeroKnight>();
        }
    }

    void Update()
    {
        // 상태 및 타이머 업데이트
        m_timeSinceAttack += Time.deltaTime;
        if (m_rolling)
        {
            m_rollCurrentTime += Time.deltaTime;
            if (m_rollCurrentTime > m_rollDuration)
                m_rolling = false; // 구르기가 끝남
        }

        // 바닥에 닿는 상태 감지
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        HandleAnimations(); // 애니메이션 상태 처리
    }

    void FixedUpdate()
    {
        HandleMovement(); // 이동 처리
    }

    private void HandleMovement()
    {
        float inputX = Input.GetAxis("Horizontal"); // 키보드 좌우 입력 값

        // 캐릭터 방향 전환 처리
        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false; // 오른쪽 바라봄
            m_facingDirection = 1;
        }
        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true; // 왼쪽 바라봄
            m_facingDirection = -1;
        }

        // 구르기 상태에서는 이동 불가
        if (!m_rolling)
            m_body2d.linearVelocity = new Vector2(inputX * m_speed, m_body2d.linearVelocity.y); // 좌우 이동 처리

        // 공중에서의 속도 업데이트 (애니메이션용)
        m_animator.SetFloat("AirSpeedY", m_body2d.linearVelocity.y);
    }

    private void HandleAnimations()
    {
        // 벽 슬라이딩 상태 확인
        m_isWallSliding = (m_wallSensors[0].State() && m_wallSensors[1].State()) || (m_wallSensors[2].State() && m_wallSensors[3].State());
        m_animator.SetBool("WallSlide", m_isWallSliding);

        // 다양한 상태에 따른 애니메이션 트리거 처리
        if (Input.GetKeyDown("e") && !m_rolling) m_animator.SetTrigger("Death"); // 사망 애니메이션
        else if (Input.GetKeyDown("q") && !m_rolling) m_animator.SetTrigger("Hurt"); // 피격 애니메이션
        else if (Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !m_rolling && !m_isAttacking)
        {
            StartCoroutine(AttackCoroutine()); // 공격 코루틴 실행
        }
        else if (Input.GetMouseButtonDown(1) && !m_rolling) // 방어 상태
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }
        else if (Input.GetMouseButtonUp(1)) m_animator.SetBool("IdleBlock", false); // 방어 해제

        // 점프 애니메이션 처리
        if (Input.GetKeyDown("space") && m_grounded && !m_rolling)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.linearVelocity = new Vector2(m_body2d.linearVelocity.x, m_jumpForce); // 점프 힘 적용
            m_groundSensor.Disable(0.2f); // 잠시 바닥 감지 비활성화
        }
    }

    private IEnumerator AttackCoroutine()
    {
        m_isAttacking = true;
        hitEnemies.Clear(); // 이미 공격한 적 초기화
        m_animator.SetTrigger("Attack" + m_currentAttack); // 현재 공격 단계 애니메이션 실행

        float attackDuration = 0.2f; // 공격 지속 시간
        float timeElapsed = 0f;

        // 공격 지속 시간 동안 적을 공격
        while (timeElapsed < attackDuration)
        {
            Attack(); // 적에게 데미지 입힘
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        m_isAttacking = false;
        m_currentAttack = (m_currentAttack % 3) + 1; // 공격 콤보 단계 갱신
    }

    private void Attack()
    {
        // 공격 범위 계산
        Vector3 attackBoxPosition = pos.position;
        if (m_facingDirection == -1)
            attackBoxPosition = new Vector3(pos.position.x - boxSize.x, pos.position.y, pos.position.z);

        // 공격 범위 내의 적 체크
        Collider2D[] colliders = Physics2D.OverlapBoxAll(attackBoxPosition, boxSize, 0);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy") && !hitEnemies.Contains(collider) )
            {
                //collider.GetComponent<Enemy>().TakeDamage(20); // 적에게 데미지 전달
                collider.GetComponent<Enemy>().TakeDamage(new ParameterPlayerAttack() { damage = 20 }); // 적에게 데미지 전달
                hitEnemies.Add(collider); // 중복 타격 방지
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (pos != null)
        {
            Gizmos.color = Color.blue; // 시각화 색상
            Vector3 attackBoxPosition = m_facingDirection == -1 ? new Vector3(pos.position.x - boxSize.x, pos.position.y, pos.position.z) : pos.position;
            Gizmos.DrawWireCube(attackBoxPosition, boxSize); // 공격 범위 시각화
        }
    }
}

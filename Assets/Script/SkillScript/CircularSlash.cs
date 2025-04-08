using UnityEngine;
using System.Collections;

public class CircularSlash : MonoBehaviour
{
    public GameObject skillPrefab; // 스킬 프리팹
    public float skillDamage = 20f; // 스킬 데미지
    public Vector3 effectSize = new Vector3(7, 3, 1); // 이펙트 크기 (크기를 조정할 벡터)

    private Transform pos; // 스킬 소환 위치
    public float skillRadius = 6f; // 원형 범위 반경 (기즈모와 관련된 값)

    private bool isSkillActive = false; // 스킬 활성화 여부
    private float damageTimer = 0f; // 대미지 처리 시간 간격
    private Coroutine skillCoroutine; // 코루틴을 저장할 변수

    void Start()
    {
        // pos를 자동으로 할당 (플레이어의 위치)
        pos = transform; // 스킬을 발동하는 객체가 이 스크립트가 붙어 있는 게임 오브젝트라고 가정
    }

    void Update()
    {
        // G 키를 눌렀을 때 스킬 발동
        if (Input.GetKeyDown(KeyCode.G))StartSkill();
        
        // G 키를 떼었을 때 스킬 중지
        if (Input.GetKeyUp(KeyCode.G))StopSkill();
        

        // 스킬이 활성화된 상태에서 계속 대미지 입히기
        if (isSkillActive)
        {
            damageTimer += Time.deltaTime;

            if (damageTimer >= 0.2f)  // 0.2초마다 대미지 처리
            {
                damageTimer = 0f;  // 타이머 초기화
                ApplyDamage();
            }
        }
    }

    private void StartSkill()
    {
        // 스킬을 활성화하고 이펙트를 소환
        if (!isSkillActive)
        {
            isSkillActive = true;  // 스킬 활성화
            skillCoroutine = StartCoroutine(SpawnSkillPrefab());  // 이펙트 생성 시작
        }
    }

    private void StopSkill()
    {
        isSkillActive = false;  // 스킬 비활성화
        damageTimer = 0f;  // 타이머 초기화

        // 이펙트 생성 코루틴을 중지
        if (skillCoroutine != null)
        {
            StopCoroutine(skillCoroutine);
            skillCoroutine = null;
        }
    }

    private void ApplyDamage()
    {
        // 적에게 대미지 처리
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(pos.position, skillRadius);
        foreach (Collider2D collider in hitEnemies)
        {
            if (collider.CompareTag("Enemy")) // "Enemy" 태그를 가진 객체들만 처리
            {
                // Enemy 스크립트가 있다면 대미지 입히기
                Enemy enemyScript = collider.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    // ParameterPlayerAttack 객체 생성
                    ParameterPlayerAttack attackParams = new ParameterPlayerAttack
                    {
                        damage = skillDamage,
                    };

                    // TakeDamage 함수 호출
                   // enemyScript.TakeDamage(attackParams);
                }
            }
        }
    }

    private IEnumerator SpawnSkillPrefab()
    {
        // 스킬 이펙트가 활성화된 동안 계속 생성
        while (isSkillActive)
        {
            // 스킬 프리팹 소환
            Vector3 spawnPosition = pos.position + new Vector3(0, 0, -1);
            GameObject spawnedSkill = Instantiate(skillPrefab, spawnPosition, Quaternion.identity);

            // 이펙트 크기 조정
            spawnedSkill.transform.localScale = effectSize;

            // 잠시 대기 (소환 간격 대기)
            yield return new WaitForSeconds(0.18f);  // 0.2초마다 이펙트 생성
        }
    }

    // Gizmos를 사용해 원형 범위를 시각적으로 확인
    void OnDrawGizmosSelected()
    {
        // 기즈모 색상 설정
        Gizmos.color = Color.red;

        // 원형 범위 시각화
        Gizmos.DrawWireSphere(transform.position, skillRadius);
    }
}

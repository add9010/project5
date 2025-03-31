using UnityEngine;
using System.Collections;

public class Skill : MonoBehaviour
{
    public GameObject[] skillPrefabs;  // 서로 다른 3개의 프리팹
    public Transform pos;              // 기준 소환 위치
    public float spawnInterval = 0.4f; // 프리팹 소환 간격
    public float skillDamage = 10f;    // 스킬 데미지
    public float knockback = 5f;       // 스킬 넉백 값
    public Vector2[] boxSizes;         // 각 프리팹의 박스 사이즈
    public Animator animator;          // 애니메이터 컴포넌트 (스킬 애니메이션을 제어)
    public float moveSpeed = 30f;      // 최대 속도 (초기값 80)
    private float minSpeed = 0f;       // 최소 속도 (0으로 설정)
    public float speedDecay = 0.5f;    // 속도 감소 값
    private bool isCastingSkill;       // 스킬 시전 여부 체크
    private int m_facingDirection = 1; // 1: 오른쪽, -1: 왼쪽

    void Update()
    {
        // 방향 전환 처리 (입력 값에 따라 왼쪽 또는 오른쪽)
        float inputX = Input.GetAxis("Horizontal");
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

        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(SpawnSkillPrefabs());
        }

        // 스킬 시전 중이라면 플레이어 이동
        if (isCastingSkill)
        {
            MovePlayer(); // 이동 처리 (Update에서 호출)
        }
    }

    private IEnumerator SpawnSkillPrefabs()
    {
        isCastingSkill = true;  // 스킬 시전 시작

        string[] attackAnimations = { "Attack1", "Attack2", "Attack3" };

        for (int i = 0; i < skillPrefabs.Length; i++)
        {
            Vector3 spawnPosition = pos.position + new Vector3(0, 0, -1);
            if (m_facingDirection == -1)
            {
                spawnPosition.x -= 2f;
            }
            else
            {
                spawnPosition.x += 2f;
            }

            GameObject spawnedSkill = Instantiate(skillPrefabs[i], spawnPosition, Quaternion.identity);

            DamageDealer damageDealer = spawnedSkill.AddComponent<DamageDealer>();
            Vector2 boxSize = i < boxSizes.Length ? boxSizes[i] : new Vector2(1.5f, 1.5f);
            damageDealer.Initialize(skillDamage, knockback, 0.2f, 3, pos, boxSize);

            SpriteRenderer skillSprite = spawnedSkill.GetComponent<SpriteRenderer>();
            if (skillSprite != null)
            {
                skillSprite.flipX = m_facingDirection == -1;
            }

            // 애니메이션 트리거
            if (animator != null)
            {
                animator.SetTrigger("Attack" + (i + 1));
            }

            // 이동을 시작하고 기다리지 않음 (Update에서 이동 처리)
            yield return new WaitForSeconds(spawnInterval);
        }

        isCastingSkill = false;  // 스킬 시전 완료
    }

    // 플레이어를 이동시키는 코루틴
    private void MovePlayer()
    {
        // 이동 속도 감소 (매 프레임마다 -0.5씩 감소)
        moveSpeed = Mathf.Max(minSpeed, moveSpeed - speedDecay * Time.deltaTime);  // 최소 속도 이하로 감소하지 않도록 처리

        // 이동할 위치 계산
        transform.position += new Vector3(m_facingDirection * moveSpeed * Time.deltaTime, 0, 0);
    }
}
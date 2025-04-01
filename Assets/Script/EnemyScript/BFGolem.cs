using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BFGolem : Golem
{
    public Text attackMessageText;                       // UI 텍스트를 참조할 변수

    public delegate void BossDeathHandler();
    public event BossDeathHandler OnBossDeath;           // 보스가 죽었을 때 호출될 이벤트

    private bool canUseSpecialAttack = false;             // 광역공격 사용 여부 변수
    public float damageOutsideRange = 10f;                // 공격 범위 밖에 나갔을 때 입을 대미지
    public float skillRange = 10f;                        // 골램 주변의 큰 원 범위 (반지름)
    public GameObject attackEffectPrefab;                 // 이펙트 Prefab
    public Vector3 effectScale = new Vector3(1f, 1f, 1f); // 이펙트의 크기 조정

    private Coroutine damageCoroutine;                    // 대미지를 주는 코루틴을 관리할 변수
    private GameObject currentAttackEffect;               // 현재 활성화된 공격 이펙트
    private Coroutine effectCoroutine;                     // 이펙트 반복 호출을 위한 코루틴

    protected override void Start()
    {
        base.Start(); // 부모 클래스의 Start 호출
        Debug.Log("BF골렘이 몸을 움직이기 시작합니다 ");
    }

    protected override void Update()
    {
        base.Update();  // 부모 클래스의 Update 호출

        // 범위 밖에 나가면 대미지 입히기
        if (canUseSpecialAttack && !isEnemyDead)
            ApplyDamageOutsideRange();
    }

    protected override void DetectAndChasePlayer()
    {
        if (player == null) return;

        HeroKnightUsing playerScript = player.GetComponent<HeroKnightUsing>();
        if (playerScript != null && playerScript.isDead) // 플레이어 사망시 추격해제
        {
            isChasing = false;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange)
        {
            if (!isChasing) // 보스패턴시작
            {
                SpawnMark();
                canUseSpecialAttack = true;
                Debug.Log("보스 패턴 시작");
                ShowAttackMessage("에이션트 골램이 영역을 선포합니다!!");
            }
            isChasing = true;

            anim.SetBool("isWalk", true);
            Vector3 direction = (player.position - transform.position).normalized;  // 플레이어를 추격
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            LookAtPlayer();
        }
        else
        {
            isChasing = false;
        }
    }

    // 골램 범위 밖에 나가면 대미지를 입히는 함수
    private void ApplyDamageOutsideRange()
    {
        ShowAttackEffect();  // 스킬 이펙트 표시
        // 범위 밖에 있는 "Player" 태그를 가진 플레이어 찾기
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, skillRange);

        // 범위 밖에 있는 플레이어에 대미지 적용
        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Player") && !IsWithinSkillRange(collider))
            {
                HeroKnightUsing playerScript = collider.GetComponent<HeroKnightUsing>();
                if (playerScript != null)
                {
                    // 범위 밖에 있으면 1초마다 대미지를 입힘
                    if (damageCoroutine == null)
                    {
                        damageCoroutine = StartCoroutine(DealDamageOverTime(playerScript));
                    }
                }
            }
        }
    }

    // 범위 내에 있는지 체크
    private bool IsWithinSkillRange(Collider2D collider)
    {
        return Vector2.Distance(transform.position, collider.transform.position) <= skillRange;
    }

    // 범위 밖에 있을 때 1초마다 대미지를 입히는 코루틴
    private IEnumerator DealDamageOverTime(HeroKnightUsing playerScript)
    {
        while (playerScript != null && !IsWithinSkillRange(playerScript.GetComponent<Collider2D>()))
        {
            playerScript.TakeDamage(damageOutsideRange); // 대미지 입히기
            Debug.Log("범위 밖에서 1초마다 대미지 입음!");
            yield return new WaitForSeconds(1f); // 1초마다 대미지
        }

        damageCoroutine = null; // 코루틴 종료 후 null로 설정
    }

    // 스킬 이펙트 표시 (0.5초마다 반복 호출)
    private void ShowAttackEffect()
    {
        if (attackEffectPrefab != null)
        {
            // 0.5초마다 이펙트를 새로 생성
            if (effectCoroutine == null)
            {
                effectCoroutine = StartCoroutine(SpawnEffect());
            }
        }
    }

    // 0.5초마다 이펙트를 새로 생성하는 코루틴
    private IEnumerator SpawnEffect()
    {
        while (canUseSpecialAttack && !isEnemyDead)
        {
            // 새로운 이펙트를 생성
            GameObject newEffect = Instantiate(attackEffectPrefab, transform.position, Quaternion.identity);
            newEffect.transform.localScale = effectScale; // 이펙트 크기 조정

            // 이펙트가 생성된 후 0.5초마다 반복
            yield return new WaitForSeconds(0.3f);
        }

        // 스킬이 종료되면 이펙트를 모두 제거
        EndSpecialAttack();
    }

    // 스킬이 끝나면 이펙트를 비활성화하는 함수
    private void EndSpecialAttack()
    {
        // 스킬 종료 시 반복 코루틴을 멈추고, 생성된 이펙트를 모두 삭제
        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine); // 반복 코루틴 종료
            effectCoroutine = null;
        }

        // 추가된 이펙트들이 너무 많아지지 않도록 삭제
        foreach (Transform child in transform)
        {
            if (child.CompareTag("AttackEffect"))
            {
                Destroy(child.gameObject);
            }
        }
    }

    // 적이 죽었을 때 호출되는 함수
    protected override void HandleWhenDead()
    {
        ShowAttackMessage("에이션트 골램을 처치하였습니다!!");
        base.HandleWhenDead();  // 기본 Enemy의 죽음 처리

        OnBossDeath?.Invoke();
        Debug.Log("보스 죽음 이벤트 호출됨");

        DropSpecialLoot();
        EndSpecialAttack();  // 보스 죽을 때 스킬 종료 처리
    }

    // 보스 전용 특별 아이템 드랍 함수
    private void DropSpecialLoot()
    {
        // 실제 아이템 오브젝트 생성 (예: Instantiation을 통한 아이템 드랍)
        Debug.Log("Special loot is dropped!");
    }

    // 공격 범위 시각화 (디버깅용)
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // 골램 주변의 큰 원 범위 그리기 (대미지를 입히는 범위)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, skillRange);  // skillRange 사용 (골램 주변 큰 원)
    }

    private void ShowAttackMessage(string message)
    {
        // UI 텍스트에 메시지를 설정
        if (attackMessageText != null)
            attackMessageText.text = message;
    }
}

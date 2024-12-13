using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class Boss : Enemy
{
    public delegate void BossDeathHandler();
    public event BossDeathHandler OnBossDeath;  // 보스가 죽었을 때 호출될 이벤트

    public float specialAttackCooldown = 2f;                // 특수 공격(폭팔공격) 쿨다운
    public float PerformAreaAttackRange; // 범위공격반경
    public GameObject attackEffectPrefab;                   // 폭팔 공격 이펙트 Prefab을 참조할 변수
    private bool canUseSpecialAttack = true;
  

    // Start 메서드를 override로 선언
    protected override void Start()
    {
        base.Start(); // 부모 클래스의 Start 호출

        // RedBoss 고유의 상태 설정
        SetEnemyStatus("레드보스 킹", 1000, 20); // 보스 초기화
        Debug.Log("RedBoss Initialized");

        PerformAreaAttackRange = detectionRange / 2; // 범위공격반경
    }

    void Update()
    {
        // 기본 Enemy의 Update 기능 유지
        base.Update();

        // 보스 특수 행동 추가
        if (canUseSpecialAttack && !isEnemyDead)
        {
            StartCoroutine(UseSpecialAttack());
        }
    }

    protected override void SpawnMark()
    {
        Debug.Log("레드 슬라임킹이 당신을 발견했습니다!!");
        if (markPrefab != null)
        {
            // 마커를 생성할 위치를 적의 위치에서 markYOffset만큼 Y축으로 올림
            Vector3 spawnPosition = transform.position + new Vector3(0, 3f, 0); // markYOffset 값만큼 Y축으로 이동
            GameObject markInstance = Instantiate(markPrefab, spawnPosition, Quaternion.identity);

            // 생성된 마커가 에너미를 추적하도록 설정
            Mark markScript = markInstance.GetComponent<Mark>();
            if (markScript != null)
            {
                markScript.enemy = transform; // 마커가 에너미를 추적하도록 설정
            }
        }
    }


    private IEnumerator UseSpecialAttack()
    {
        canUseSpecialAttack = false;

        // 특수 공격 로직 (예: 범위 공격)
        Debug.Log("레드 슬라임킹이 강한 공격을 준비합니다.");
        PerformAreaAttack();

        // 쿨다운 대기
        yield return new WaitForSeconds(specialAttackCooldown);
        canUseSpecialAttack = true;
    }


    private void PerformAreaAttack()
    {
        // 범위 내에 있는 모든 Collider2D를 탐지
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, PerformAreaAttackRange);


        // 공격 범위에 이펙트 생성 (범위 전체에 표시)
        if (attackEffectPrefab != null)
        {
            GameObject effectInstance = Instantiate(attackEffectPrefab, transform.position, Quaternion.identity);
            effectInstance.transform.localScale = new Vector3(detectionRange, PerformAreaAttackRange, 1f); // X, Y 크기를 detectionRange로 설정
        }

        foreach (Collider2D hitObject in hitObjects)
        {
            // 플레이어 태그를 가진 오브젝트인지 확인
            if (hitObject.CompareTag("Player"))
            {
                // 플레이어의 스크립트 가져오기
                HeroKnightUsing playerScript = hitObject.GetComponent<HeroKnightUsing>();

                if (playerScript != null && !playerScript.isDead)
                {   
                    playerScript.TakeDamage(atkDmg * 2);
                    Debug.Log($"레드보스가 {hitObject.name}에게 {atkDmg * 2}의 특수 공격으로 데미지를 입혔습니다!");

                   
                }
            }
        }
    }



    // Gizmo: 특수 공격 범위를 시각적으로 표시
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();  // 기본 Enemy의 시각적 범위 표시
        Gizmos.color = Color.yellow;  // 특수 공격 범위 색깔
        Gizmos.DrawWireSphere(transform.position, detectionRange);  // 특수 공격 범위
    }

    // 적이 죽었을 때 호출되는 함수
    protected override void HandleWhenDead()
    {
        base.HandleWhenDead();  // 기본 Enemy의 죽음 처리

       
        // 보스 죽음 이벤트 발생
        OnBossDeath?.Invoke();

        DropSpecialLoot();
    }

    // 보스 전용 특별 아이템 드랍 함수
    private void DropSpecialLoot()
    {
        // 실제 아이템 오브젝트 생성 (예: Instantiation을 통한 아이템 드랍)
        Debug.Log("레드슬라임 킹을 처치하였습니다!");
        // 예시: Instantiate(lootPrefab, transform.position, Quaternion.identity);
    }
}

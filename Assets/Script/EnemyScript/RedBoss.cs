//using UnityEngine;
//using UnityEngine.UI;
//using System.Collections;

//public class RedSlimeKing : Slime
//{

//    public delegate void BossDeathHandler();
//    public event BossDeathHandler OnBossDeath;  // 보스가 죽었을 때 호출될 이벤트

//    public Text attackMessageText;                          // UI 텍스트를 참조할 변수 (디버그 메시지를 표시할 텍스트)
//    public float specialAttackCooldown = 8f;                // 특수 공격(폭팔공격) 쿨다운
//    public float PerformAreaAttackRange;                    // 범위공격반경
//    public GameObject attackEffectPrefab;                   // 폭팔 공격 이펙트 Prefab을 참조할 변수
//    private bool canUseSpecialAttack = false;               // 광역공격 사용 여부 변수
//    private SpawnEnemy spawnEnemyScript;                    // SpawnEnemy 스크립트를 참조할 변수

//    public Vector3 effectScale = new Vector3(1f, 1f, 1f);  // 이펙트의 크기 조정 (인스펙터에서 조정 가능)


//    // Start 메서드를 override로 선언
//    protected override void Start()
//    {
//        base.Start(); // 부모 클래스의 Start 호출

//        Debug.Log("RedBoss Initialized");
//        PerformAreaAttackRange = detectionRange * 0.9f; // 범위공격반경
//        spawnEnemyScript = GetComponent<SpawnEnemy>();
//    }

//    protected override void Update()
//    {
//        // 기본 Enemy의 Update 기능 유지
//        base.Update();

//        // 보스 특수 행동 추가
//        if (canUseSpecialAttack && !isEnemyDead)
//            StartCoroutine(UseSpecialAttack());
//    }

//    protected override void DetectAndChasePlayer()
//    {
//        if (player == null) return;

//        HeroKnightUsing playerScript = player.GetComponent<HeroKnightUsing>();
//        if (playerScript != null && playerScript.isDead)
//        {
//            isChasing = false;
//            return;
//        }

//        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
//        if (distanceToPlayer <= detectionRange)
//        {
//            if (!isChasing) // 플레이어를 처음 탐지했을 때만 마크를 소환
//            {
//                SpawnMark();
//                canUseSpecialAttack = true;

//                // 보스가 플레이어를 탐지했을 때 적 스폰을 시작
//                if (spawnEnemyScript != null) spawnEnemyScript.StartSpawning();
//                SpawnMark();
//                canUseSpecialAttack = true;

//                isChasing = true;
//                Vector3 direction = (player.position - transform.position).normalized;
//                transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
//                LookAtPlayer();
//            }

         
//        }
//        else
//        {
//            isChasing = false;
//        }
//    }
//    protected override void SpawnMark()
//    {
//        Debug.Log("레드 슬라임킹이 당신을 주시합니다!!");
//        if (markPrefab != null)
//        {
//            // 마커를 생성할 위치를 적의 위치에서 markYOffset만큼 Y축으로 올림
//            Vector3 spawnPosition = transform.position + new Vector3(0, 3f, 0); // markYOffset 값만큼 Y축으로 이동
//            GameObject markInstance = Instantiate(markPrefab, spawnPosition, Quaternion.identity);
//            StartCoroutine(SpeedBoostRoutine());  // 이동 속도 증가 코루틴 시작

//            // 생성된 마커가 에너미를 추적하도록 설정
//            Mark markScript = markInstance.GetComponent<Mark>();
//            if (markScript != null)
//            {
//                markScript.enemy = transform; // 마커가 에너미를 추적하도록 설정
//            }
//        }
//    }

//    private IEnumerator UseSpecialAttack()
//    {
//        canUseSpecialAttack = false;

//        // 특수 공격 준비 메시지를 화면에 표시
//        ShowAttackMessage("레드 슬라임킹이 강한 공격을 준비합니다.");
//        Debug.Log("레드 슬라임킹이 강한 공격을 준비합니다.");
//        yield return new WaitForSeconds(2f);  // 2초 동안 텍스트 유지
//        ShowAttackMessage("");
//        yield return new WaitForSeconds(5f);  // 3초 뒤 광역공격 
//        PerformAreaAttack();


//        // 쿨다운 대기
//        yield return new WaitForSeconds(specialAttackCooldown);


//        canUseSpecialAttack = true;

//    }

//    private void ShowAttackMessage(string message)
//    {
//        // UI 텍스트에 메시지를 설정
//        if (attackMessageText != null)
//        {
//            attackMessageText.text = message;
//        }
//    }

//    private void PerformAreaAttack()
//    {
//        // 범위 내에 있는 모든 Collider2D를 탐지
//        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, PerformAreaAttackRange);

//        // 공격 범위에 이펙트 생성 (범위 전체에 표시)
//        if (attackEffectPrefab != null)
//        {
//            GameObject effectInstance = Instantiate(attackEffectPrefab, transform.position, Quaternion.identity);
//            // 인스펙터에서 지정된 effectScale로 크기 설정
//            effectInstance.transform.localScale = effectScale;
//        }

//        foreach (Collider2D hitObject in hitObjects)
//        {
//            // 플레이어 태그를 가진 오브젝트인지 확인
//            if (hitObject.CompareTag("Player"))
//            {
//                // 플레이어의 스크립트 가져오기
//                HeroKnightUsing playerScript = hitObject.GetComponent<HeroKnightUsing>();

//                if (playerScript != null && !playerScript.isDead)
//                {
//                    playerScript.TakeDamage(atkDmg * 2);
//                    Debug.Log($"레드보스가 {hitObject.name}에게 {atkDmg * 3}의 특수 공격으로 데미지를 입혔습니다!");
//                }
//            }
//        }
//    }

//    // 적이 죽었을 때 호출되는 함수
//    protected override void HandleWhenDead()
//    {
//        base.HandleWhenDead();  // 기본 Enemy의 죽음 처리

//        // RedBoss 고유의 죽음 처리
//        Debug.Log("RedBoss defeated! Special loot dropped.");

//        OnBossDeath?.Invoke();
//        Debug.Log("보스 죽음 이벤트 호출됨");


//        DropSpecialLoot();
//    }

//    // 보스 전용 특별 아이템 드랍 함수
//    private void DropSpecialLoot()
//    {
//        // 실제 아이템 오브젝트 생성 (예: Instantiation을 통한 아이템 드랍)
//        Debug.Log("Special loot is dropped!");
//    }
//    protected override void OnDrawGizmosSelected()
//    {
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireSphere(transform.position, detectionRange);

//        Gizmos.color = Color.blue;
//        Gizmos.DrawWireSphere(transform.position, PerformAreaAttackRange);
//    }
//}

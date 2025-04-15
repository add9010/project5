//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Golem : Enemy
//{
//    [Header("Golem Stats")]
//    public float attackAreaRadius = 3f;  // 골램 앞부분의 공격 범위 (반지름)
//    public float downAttackRange = 1.5f;

//    protected override void Update()
//    {
//        // 피격 상태가 아니고, 살아있으며, 플레이어가 존재한다면 추격
//        if (!isInDamageState && nowHp > 0 && player != null&& !anim.GetBool("isAttack"))
//            DetectAndChasePlayer();


//        // 체력 바 위치,상태 갱신
//        if (hpBar != null)
//        {
//            Vector3 _hpBarPos = Camera.main.WorldToScreenPoint
//                (new Vector3(transform.position.x, transform.position.y + height, 0));
//            hpBar.position = _hpBarPos;
//            nowHpbar.fillAmount = nowHp / maxHp;
//        }

//        // 공격 애니메이션 상태가 아니고, 플레이어와의 거리가 공격 범위 내이면 공격
//        if (player != null && !isEnemyDead && !isInDamageState && !anim.GetBool("isAttack"))
//        {
//            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
//            if (distanceToPlayer <= attackAreaRadius)
//            {
//                Attack();  // 공격 호출
//            }
//        }
//    }

//    protected virtual void Attack() // 골램의 팔휘두르기 공격
//    {
//        Debug.Log("공격호출");

//        // 공격 애니메이션 설정
//        anim.SetBool("isAttack", true);

//        // 공격 애니메이션이 끝날 때까지 대기
//        StartCoroutine(WaitForAttackAnimation(1.4f));  // 애니메이션 길이(2초)에 맞춰서 조절
//    }

//    private IEnumerator WaitForAttackAnimation(float animationLength)
//    {
//        yield return new WaitForSeconds(animationLength);  // 애니메이션이 끝날 때까지 대기

//        // 공격 범위 내 플레이어에게 피해를 주기
//        Vector2 attackPosition = new Vector2(transform.position.x, transform.position.y - downAttackRange); 
//        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackPosition, attackAreaRadius);

//        foreach (var collider in hitColliders)
//        {
//            if (collider != null && collider.CompareTag("Player"))
//            {
//                HeroKnightUsing playerScript = collider.GetComponent<HeroKnightUsing>();
//                if (playerScript != null)
//                {
//                    playerScript.TakeDamage(atkDmg);  // 플레이어에게 공격력만큼 피해 입히기
//                }
//            }
//        }


//        // 애니메이션 끝나면 공격 상태 종료
//        anim.SetBool("isAttack", false);
//        yield return new WaitForSeconds(1.7f);
//    }

//    protected override void OnDrawGizmosSelected()
//    {
//        base.OnDrawGizmosSelected();

//        Gizmos.color = Color.blue;
//        // 공격 범위 시각화 (Y축으로 2만큼 내린 위치에 원을 그림)
//        Vector2 gizmoPosition = new Vector2(transform.position.x, transform.position.y - downAttackRange);
//        Gizmos.DrawWireSphere(gizmoPosition, attackAreaRadius); 
//    }
//}

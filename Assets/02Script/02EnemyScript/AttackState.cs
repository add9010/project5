using UnityEngine;

public class AttackState : IEnemyState
{
    private bool hasAttacked;
    private float timer;

    public void Enter(Enemy enemy)
    {
        enemy.anim.SetBool("attack",true);
        enemy.StopMovement();
        hasAttacked = false;
        timer = 0f;
    }

    public void Update(Enemy enemy)
    {
        timer += Time.deltaTime;

        // ⏱️ 패링 윈도우 설정
        enemy.SetParryWindow(timer >= 0.2f && timer <= 0.5f);

        if (!hasAttacked && timer >= enemy.attackHitDelay)
        {
            hasAttacked = true;
            enemy.PerformAttack();
        }

        if (timer >= enemy.attackCooldown)
        {
            enemy.SetParryWindow(false); // 공격 끝나면 패링 종료

            // ◀ 여기서 자기 자신(AttackState)으로 전환하지 않음
            if (enemy.enablePatrol && enemy.IsPlayerDetected())
            {
                enemy.SwitchState(new ChaseState());
            }
            else
            {
                enemy.ReturnToDefaultState();  // 기본 상태(Idle)로 복귀
            }
        }
        // 타격 시점
        AnimatorStateInfo stateInfo = enemy.anim.GetCurrentAnimatorStateInfo(0);

        if (!hasAttacked && stateInfo.normalizedTime >= enemy.attackHitDelay)
        {
            hasAttacked = true;
            enemy.PerformAttack();
            enemy.RecordAttackTime();
        }

        // 애니메이션 끝났을 때 상태 전환
        if (stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 1.0f)
        {
            enemy.anim.SetBool("attack", false);

            if (enemy.IsPlayerDetected())
                enemy.SwitchState(new ChaseState());
            else
                enemy.ReturnToDefaultState();
        }
    }

    public void Exit(Enemy enemy)
    {
        // 애니 Bool 리셋
        enemy.anim.SetBool("attack", false);
        enemy.SetParryWindow(false);
    }
}

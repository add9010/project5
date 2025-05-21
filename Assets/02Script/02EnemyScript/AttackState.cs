using UnityEngine;

public class AttackState : IEnemyState
{
    private bool hasAttacked;
    private float timer;

    public void Enter(Enemy enemy)
    {
        enemy.RecordAttackTime();
        enemy.anim.SetTrigger("attack");
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
            enemy.RecordAttackTime();
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
    }

    public void Exit(Enemy enemy)
    {
        enemy.SetParryWindow(false);
    }
}

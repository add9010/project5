using UnityEngine;

public class AttackState : IEnemyState
{
    private bool hasAttacked;
    private float timer;

    public void Enter(Enemy enemy)
    {
        enemy.anim.SetTrigger("attack");
        enemy.StopMovement();
        hasAttacked = false;
        timer = 0f;
    }

    public void Update(Enemy enemy)
    {
        timer += Time.deltaTime;

        // ⏱️ 0.2초 ~ 0.5초 사이만 패링 가능
        enemy.SetParryWindow(timer >= 0.2f && timer <= 0.5f);

        if (!hasAttacked && timer >= 0.3f)
        {
            hasAttacked = true;
            enemy.PerformAttack();
        }

        if (timer >= enemy.attackCooldown)
        {
            enemy.SetParryWindow(false); // 공격 끝나면 패링 종료
            if (enemy.IsPlayerInAttackRange())
                enemy.SwitchState(new AttackState());
            else if (enemy.enablePatrol && enemy.IsPlayerDetected())
                enemy.SwitchState(new ChaseState());
            else
                enemy.ReturnToDefaultState();
        }
    }

    public void Exit(Enemy enemy) { }
}

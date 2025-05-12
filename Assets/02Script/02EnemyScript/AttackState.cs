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

        if (!hasAttacked && timer >= 0.3f)
        {
            hasAttacked = true;
            enemy.PerformAttack(); // 개별 클래스(Thief/Wizard)에서 오버라이드된 공격 실행
        }

        if (timer >= enemy.attackCooldown)
        {
            if (enemy.IsPlayerInAttackRange())
                enemy.SwitchState(new AttackState());       // 쿨다운 후 다시 AttackState → 연속 공격
            else if (enemy.enablePatrol && enemy.IsPlayerDetected())
                enemy.SwitchState(new ChaseState());        // 사정거리 밖이면서 순찰 활성 → 추격
            else
                enemy.ReturnToDefaultState();
        }
    }

    public void Exit(Enemy enemy) { }
}

using System.Collections;
using UnityEngine;

public class AttackState : IEnemyState
{
    private bool hasAttacked = false;
    private float attackCooldown = 1.0f;
    private float timer = 0f;

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
            enemy.PerformAttack(); // 실제 공격 (Thief/Wizard 오버라이드)
        }

        if (timer >= attackCooldown)
        {
            if (enemy.IsPlayerInAttackRange())
                enemy.SwitchState(new AttackState()); // 연속 공격 가능
            else if (enemy.enablePatrol && enemy.IsPlayerDetected())
                enemy.SwitchState(new ChaseState());
            else
                enemy.ReturnToDefaultState(); // Idle or Patrol
        }
    }

    public void Exit(Enemy enemy) { }
}

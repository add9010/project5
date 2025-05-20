using UnityEngine;

public class IdleState : IEnemyState
{
    public void Enter(Enemy enemy)
    {
        enemy.StopMovement();
        enemy.anim.SetBool("isWalk", false);
    }

    public void Update(Enemy enemy)
    {
        if (enemy.IsPlayerInAttackRange() && enemy.CanAttack())
        {
            enemy.SwitchState(new AttackState());
            return;
        }

        if (enemy.enablePatrol && enemy.IsPlayerDetected())
        {
            enemy.SwitchState(new ChaseState());
        }
    }

    public void Exit(Enemy enemy) { }
}

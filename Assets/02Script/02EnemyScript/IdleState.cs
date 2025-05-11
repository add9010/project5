using UnityEngine;

public class IdleState : IEnemyState
{
    public void Enter(Enemy enemy)
    {
        enemy.anim.SetBool("isWalk", false);
    }

    public void Update(Enemy enemy)
    {
        if (enemy.IsPlayerInAttackRange())
        {
            enemy.SwitchState(new AttackState());
        }
        else if (enemy.enablePatrol && enemy.IsPlayerDetected())
        {
            enemy.SwitchState(new ChaseState());
        }
    }

    public void Exit(Enemy enemy) { }
}

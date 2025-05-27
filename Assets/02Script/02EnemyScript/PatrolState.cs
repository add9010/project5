using UnityEngine;

public class PatrolState : IEnemyState
{
    public void Enter(Enemy enemy)
    {
        enemy.anim.SetBool("isWalk", true);
        enemy.SetPatrolTarget();
    }

    public void Update(Enemy enemy)
    {
        if (enemy.IsPlayerDetected())
        {
            enemy.SwitchState(new ChaseState());
            return;
        }
        enemy.anim.SetBool("isWalk", true);
        enemy.Patrol();
    }

    public void Exit(Enemy enemy)
    {
        enemy.anim.SetBool("isWalk", false);
    }
}

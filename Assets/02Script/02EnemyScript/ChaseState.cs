using UnityEngine;

public class ChaseState : IEnemyState
{
    private bool spawnedMark = false;

    public void Enter(Enemy enemy)
    {
     
        if (!enemy.enablePatrol)
        {
            enemy.SwitchState(new IdleState());
            return;
        }

        enemy.anim.SetBool("isWalk", true);

        if (!spawnedMark)
        {
            enemy.SpawnMark();
            spawnedMark = true;
        }
    }

    public void Update(Enemy enemy)
    {
        if (!enemy.IsPlayerDetected())
        {
            enemy.SwitchState(new PatrolState());
            return;
        }

        if (enemy.IsPlayerInAttackRange() && enemy.CanAttack())
        {
            enemy.SwitchState(new AttackState());
            return;
        }

        enemy.MoveToPlayer();
    }

    public void Exit(Enemy enemy)
    {
        enemy.anim.SetBool("isWalk", false);
    }
}

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

        enemy.SetParryWindow(timer >= 0.2f && timer <= 0.5f);

        if (!hasAttacked && timer >= enemy.attackHitDelay)
        {
            hasAttacked = true;
            enemy.PerformAttack();
            enemy.RecordAttackTime();
        }

        if (timer >= enemy.attackCooldown)
        {
            enemy.SetParryWindow(false);
            
            if (enemy.enablePatrol && enemy.IsPlayerDetected())
            {
                enemy.SwitchState(new ChaseState());
            }
            else
            {
                enemy.ReturnToDefaultState(); 
            }
        }
    }

    public void Exit(Enemy enemy)
    {
        enemy.SetParryWindow(false);
    }
}

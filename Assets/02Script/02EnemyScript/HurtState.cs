using UnityEngine;

public class HurtState : IEnemyState
{
    private float stunTime = 0.2f;
    private float timer = 0f;

    public void Enter(Enemy enemy)
    {
        enemy.anim.SetTrigger("hit");
        enemy.StopMovement();
        timer = 0f;
    }

    public void Update(Enemy enemy)
    {
        timer += Time.deltaTime;
        if (timer >= stunTime)
        {
            enemy.SwitchState(enemy.enablePatrol ? new PatrolState() : new IdleState());
        }
    }

    public void Exit(Enemy enemy) { }
}

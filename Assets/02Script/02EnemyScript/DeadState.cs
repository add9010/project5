using UnityEngine;

public class DeadState : IEnemyState
{
    public void Enter(Enemy enemy)
    {
        enemy.HandleWhenDead();
    }

    public void Update(Enemy enemy) { }
    public void Exit(Enemy enemy) { }
}

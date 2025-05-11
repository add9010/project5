using UnityEngine;

public class DeadState : IEnemyState
{
    public void Enter(Enemy enemy)
    {
        enemy.anim.SetBool("isDead", true);
        enemy.Die(); // 체력 0 처리
    }

    public void Update(Enemy enemy) { }
    public void Exit(Enemy enemy) { }
}

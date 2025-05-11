using UnityEngine;

public interface IEnemyState
{
    void Enter(Enemy enemy);
    void Update(Enemy enemy);
    void Exit(Enemy enemy);
}

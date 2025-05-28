// StaggerState.cs
using UnityEngine;

public class StaggerState : IEnemyState
{
    private float timer;

    public void Enter(Enemy enemy)
    {
        // 애니메이션 전환
        enemy.anim.SetBool("isStagger", true);
        // 스턴 시간 초기화
        timer = enemy.stunDuration;
        // 멈춤 처리
        enemy.StopMovement();
    }

    public void Update(Enemy enemy)
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            // 애니메이션 해제
            enemy.anim.SetBool("isStagger", false);
            // stagger 회복
            enemy.currentStagger = enemy.maxStagger;
            // 기본 순찰 또는 대기 상태로 복귀
            enemy.SwitchState(enemy.enablePatrol
                ? (IEnemyState)new PatrolState()
                : new IdleState());
        }
    }

    public void Exit(Enemy enemy)
    {
        // 추가로 해줄 게 있으면…
    }
}

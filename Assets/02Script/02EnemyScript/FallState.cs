using UnityEngine;

public class FallState : IEnemyState
{
    private readonly float fallGravity;
    private readonly float normalGravity;

    public FallState(float fallGravity, float normalGravity)
    {
        this.fallGravity = fallGravity;
        this.normalGravity = normalGravity;
    }

    public void Enter(Enemy e)
    {
        // 1) Y축 고정 해제
        e.Rigid.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
        // 2) 중력 스케일 즉시 변경
        e.Rigid.gravityScale = fallGravity;
        // 3) 기존 Y속도 초기화 → 중력이 바로 먹히도록
        var v = e.Rigid.linearVelocity;
        e.Rigid.AddForce(Vector2.down * fallGravity * 10f, ForceMode2D.Impulse);
        // 4) 걷기 애니메이션 끄기
        e.Anim.SetBool("isWalk", false);
    }

    public void Update(Enemy e)
    {
        if (e.EdgeDetector.IsGroundAhead)
        {
            // 1) 땅에 닿은 지점 찾기
            Vector2 origin = e.Rigid.position + Vector2.up * 0.1f;
            var hit = Physics2D.Raycast(origin, Vector2.down, 2f, e.EdgeDetector.groundLayer);
            if (hit.collider != null)
            {
                // 2) 콜라이더 절반 높이 만큼 올려서 정확히 스냅
                float footH = e.GetComponent<Collider2D>().bounds.extents.y;
                float landY = hit.point.y + footH;

                // 3) 즉시 위치 보정
                e.Rigid.position = new Vector2(e.Rigid.position.x, landY);
                e.transform.position = new Vector3(e.transform.position.x, landY, e.transform.position.z);
            }

            // 4) 다시 y 고정하거나 회전만 고정
            e.Rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            // 또는: e.Rigid.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

            // 5) 원래 상태로 복귀
            var next = e.EnablePatrol ? (IEnemyState)new PatrolState() : new IdleState();
            e.SwitchState(next);
        }
    }

    public void Exit(Enemy e)
    {
        // 낙하 끝나면 중력·제약 복구
        e.Rigid.gravityScale = normalGravity;
        e.Rigid.constraints |= RigidbodyConstraints2D.FreezePositionY;
    }
}

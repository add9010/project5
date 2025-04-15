using UnityEngine;
using System.Collections;
public class PlayerDash
{
    private PlayerManager manager;
    private bool isDashing = false;

    public PlayerDash(PlayerManager manager)
    {
        this.manager = manager;
    }

    public void TryDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && IsGrounded() && !isDashing)
        {
            manager.StartAttackCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        isDashing = true;
        manager.isDashing = true; // 상태 컨트롤러용

        manager.playerStateController.ForceSetDash(); // Dash 상태 고정

        float direction = manager.spriteRenderer.flipX ? -1f : 1f;
        manager.rb.linearVelocity = new Vector2(direction * manager.data.dashForce, 0);

        yield return new WaitForSeconds(manager.data.dashDuration); // <- 해당하는 기간동안 애니메이션 멈춤 // 조용히 해
        

        isDashing = false; // <- 해당 값은 의심안해도 됨
        manager.isDashing = false; // <- 해당 값이 true일 때, 애니메이션을 막는것으로 의심
    }

    private bool IsGrounded()
    {
        return manager.groundSensor != null && manager.groundSensor.State();
    }
}

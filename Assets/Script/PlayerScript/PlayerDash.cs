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

        yield return new WaitForSeconds(manager.data.dashDuration);

        isDashing = false;
        manager.isDashing = false;
    }

    private bool IsGrounded()
    {
        return manager.groundSensor != null && manager.groundSensor.State();
    }
}

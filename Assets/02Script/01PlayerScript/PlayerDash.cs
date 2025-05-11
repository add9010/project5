using UnityEngine;
using System.Collections;

public class PlayerDash
{
    private PlayerManager manager;
    private bool isDashing = false;
    private bool dashCooldown = false;

    private float lastLeftTime = -1f;
    private float lastRightTime = -1f;
    private float doubleTapThreshold = 0.25f; // 더블탭 최대 허용 시간
    private float dashCooldownTime = 0.5f;     // 쿨타임 0.5초

    public PlayerDash(PlayerManager manager)
    {
        this.manager = manager;
    }

    public void TryDash()
    {
        if (isDashing || dashCooldown || !IsGrounded()) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (Time.time - lastLeftTime <= doubleTapThreshold)
                Dash(-1f); // 왼쪽 대시

            lastLeftTime = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (Time.time - lastRightTime <= doubleTapThreshold)
                Dash(1f); // 오른쪽 대시

            lastRightTime = Time.time;
        }
    }

    private void Dash(float direction)
    {
        manager.StartAttackCoroutine(DashCoroutine(direction));
    }

    private IEnumerator DashCoroutine(float direction)
    {
        isDashing = true;
        dashCooldown = true;
        manager.isDashing = true;

        manager.playerStateController.ForceSetDash();
        manager.rb.linearVelocity = new Vector2(direction * manager.data.dashForce, 0);

        yield return new WaitForSeconds(manager.data.dashDuration);

        isDashing = false;
        manager.isDashing = false;

        // 쿨타임 대기
        yield return new WaitForSeconds(dashCooldownTime);
        dashCooldown = false;
    }

    private bool IsGrounded()
    {
        return manager.groundSensor != null && manager.groundSensor.State();
    }
}

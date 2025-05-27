using UnityEngine;
using System.Collections;

public class PlayerDash
{
    private PlayerManager manager;
    private bool isDashing = false;
    private bool dashCooldown = false;

    private float lastLeftTime = -1f;
    private float lastRightTime = -1f;
    private float doubleTapThreshold = 0.25f;
    private float dashCooldownTime = 0.5f;

    private float dashLastUsedTime = -999f; // ✅ 추가

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
                Dash(-1f);

            lastLeftTime = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (Time.time - lastRightTime <= doubleTapThreshold)
                Dash(1f);

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

        dashLastUsedTime = Time.time; // ✅ 추가

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);

        manager.playerStateController.ForceSetDash();
        manager.rb.linearVelocity = new Vector2(direction * manager.data.dashForce, 0);

        yield return new WaitForSeconds(manager.data.dashDuration);

        isDashing = false;
        manager.isDashing = false;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);

        yield return new WaitForSeconds(dashCooldownTime);
        dashCooldown = false;
    }

    private bool IsGrounded()
    {
        return manager.groundSensor != null && manager.groundSensor.State();
    }

    // ✅ 쿨타임용 Getter
    public float GetLastUsedTime() => dashLastUsedTime;
    public float GetCooldownDuration() => dashCooldownTime;
    public bool IsCoolingDown() => Time.time < dashLastUsedTime + dashCooldownTime;
}

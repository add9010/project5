using UnityEngine;

public class PlayerMove
{
    private PlayerManager manager;

    public PlayerMove(PlayerManager manager)
    {
        this.manager = manager;
    }

    public float GetHorizontalInput()
    {
        return Input.GetAxisRaw("Horizontal");
    }

    public bool TryJump()
    {
        // 스페이스바 눌렀고 땅에 닿아 있을 때
        return Input.GetKeyDown(KeyCode.Space) && IsGrounded();
    }

    public void DoJump()
    {
        // 점프 힘만큼 수직 속도 설정
        manager.rb.linearVelocity = new Vector2(
            manager.rb.linearVelocity.x,
            manager.data.jumpForce
        );
    }
    public void Move(float inputX)
    {
        if (manager.isAction) return;

        manager.rb.linearVelocity = new Vector2(
            inputX * manager.data.speed,
            manager.rb.linearVelocity.y
        );
        if (inputX > 0)
            manager.spriteRenderer.flipX = false;
        else if (inputX < 0)
            manager.spriteRenderer.flipX = true;
    }

    private bool IsGrounded()
    {
        // 지상 센서가 있고 땅에 닿아 있는지 확인
        return manager.groundSensor != null && manager.groundSensor.State();
    }
}
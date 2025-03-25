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
        // �����̽��� ������ ���� ��� ���� ��
        return Input.GetKeyDown(KeyCode.Space) && IsGrounded();
    }

    public void DoJump()
    {
        // ���� ����ŭ ���� �ӵ� ����
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
        // ���� ������ �ְ� ���� ��� �ִ��� Ȯ��
        return manager.groundSensor != null && manager.groundSensor.State();
    }
}
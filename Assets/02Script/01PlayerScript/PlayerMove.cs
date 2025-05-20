using UnityEngine;

public class PlayerMove
{
    private PlayerManager manager;

    public PlayerMove(PlayerManager manager)
    {
        this.manager = manager;
    }

    public Vector2 GetInput()
    {
        float x = 0f;
        float y = 0f;

        if (Input.GetKey(KeyCode.LeftArrow)) x = -1f;
        else if (Input.GetKey(KeyCode.RightArrow)) x = 1f;

        if (Input.GetKey(KeyCode.UpArrow)) y = 1f;
        else if (Input.GetKey(KeyCode.DownArrow)) y = -1f;

        return new Vector2(x, y);
    }

    public bool TryJump()
    {
        bool isJumpKey = Input.GetKeyDown(KeyCode.UpArrow);
        return isJumpKey && IsGrounded();
    }

    public void DoJump()
    {
        manager.rb.linearVelocity = new Vector2(
            manager.rb.linearVelocity.x,
            manager.data.jumpForce
        );
    }

    public void Move(Vector2 input)
    {
        if (manager.isAction) return;

        float moveX = input.x;

        manager.rb.linearVelocity = new Vector2(
            moveX * manager.data.speed,
            manager.rb.linearVelocity.y
        );

        if (moveX > 0)
            manager.spriteRenderer.flipX = false;
        else if (moveX < 0)
            manager.spriteRenderer.flipX = true;

        // ↑ 방향 입력 시 상호작용을 위한 디버깅 (추후 사용 가능)
        if (input.y > 0)
        {
            Debug.Log("↑ 위 방향 입력 감지됨");
        }
    }

    private bool IsGrounded()
    {
        return manager.groundSensor != null && manager.groundSensor.State();
    }
}

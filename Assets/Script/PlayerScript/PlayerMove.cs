using UnityEngine;

public class PlayerMove
{
    private PlayerManager manager;

    private bool isRolling = false;
    private float rollTimer = 0f;

    private bool IsGrounded()
    {
        return manager.groundSensor != null && manager.groundSensor.State();
    }

    public PlayerMove(PlayerManager manager)
    {
        this.manager = manager;
    }

    public void HandleInput()
    {
        if (manager.isAction) return; // 대화 중엔 조작 X

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded() && !isRolling)
        {
            manager.animator.SetTrigger("Jump");
            manager.rb.linearVelocity = new Vector2(manager.rb.linearVelocity.x, manager.data.jumpForce);
        }
    }

    public void HandleMovement()
    {
        if (manager.isAction)
        {
            manager.rb.linearVelocity = Vector2.zero;
            return;
        }

        bool isGrounded = IsGrounded();
        manager.animator.SetBool("Grounded", isGrounded);

        if (isRolling)
        {
            rollTimer += Time.deltaTime;
            if (rollTimer >= manager.data.rollDuration)
            {
                isRolling = false;
                manager.GetComponent<PlayerCollision>()?.IgnoreEnemyCollisions(false);
            }
            return;
        }

        float inputX = Input.GetAxis("Horizontal");
        manager.rb.linearVelocity = new Vector2(inputX * manager.data.speed, manager.rb.linearVelocity.y);

        // 방향 반전
        if (inputX > 0)
        {
            manager.spriteRenderer.flipX = false;
        }
        else if (inputX < 0)
        {
            manager.spriteRenderer.flipX = true;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            // grounded = true; → grounded 사용 X
            // 대신 IsGrounded()를 애니메이션 갱신용으로만 사용
        }
    }
}
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
        //if (manager.isAction) return;// 대화 중엔 조작 X

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded() && !isRolling)
        {
            manager.animator.SetTrigger("Jump");
            manager.rb.linearVelocity = new Vector2(manager.rb.linearVelocity.x, manager.data.jumpForce);
            manager.groundSensor.Disable(0.2f);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && IsGrounded() && !isRolling)
        {
            isRolling = true;
            rollTimer = 0f;

            manager.animator.SetTrigger("Roll");

            float dir = manager.spriteRenderer.flipX ? -1 : 1;
            manager.rb.linearVelocity = new Vector2(dir * manager.data.rollForce, 0f);

            manager.GetComponent<PlayerCollision>()?.IgnoreEnemyCollisions(true);
        }
    }

    public void HandleMovement()
    {
        //if (manager.isaction)
        //{
        //    manager.rb.linearvelocity = vector2.zero; // rigidbody 이동 멈춤
        //    return;                        // 이후 이동 코드 실행 x
        //}

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
            return; // 구르는 중엔 조작 X
        }

        float inputX = Input.GetAxis("Horizontal");
        manager.rb.linearVelocity = new Vector2(inputX * manager.data.speed, manager.rb.linearVelocity.y);

        if (inputX > 0)
            manager.spriteRenderer.flipX = false;
        else if (inputX < 0)
            manager.spriteRenderer.flipX = true;
    }
}
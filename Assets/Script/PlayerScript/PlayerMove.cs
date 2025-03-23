using UnityEngine;

public class PlayerMove
{
    private PlayerManager manager;
    private bool grounded = true;

    public PlayerMove(PlayerManager manager)
    {
        this.manager = manager;
    }

    public void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            manager.animator.SetTrigger("Jump");
            manager.rb.linearVelocity = new Vector2(manager.rb.linearVelocity.x, manager.data.jumpForce);
            grounded = false;
        }
    }

    public void HandleMovement()
    {
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
            grounded = true;
        }
    }
}
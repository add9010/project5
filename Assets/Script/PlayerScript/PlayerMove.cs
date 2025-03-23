using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private PlayerManager manager;
    private bool grounded = true;

    private void Start()
    {
        manager = PlayerManager.Instance;
    }

    public void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            manager.animator.SetTrigger("Jump");
            manager.rb.linearVelocity = new Vector2(manager.rb.linearVelocity.x, manager.data.jumpForce); // 수정
            grounded = false;
        }
    }

    public void HandleMovement()
    {
        float inputX = Input.GetAxis("Horizontal");
        manager.rb.linearVelocity = new Vector2(inputX * manager.data.speed, manager.rb.linearVelocity.y); // 수정

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
}
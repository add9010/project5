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
            manager.rb.linearVelocity = new Vector2(manager.rb.linearVelocity.x, manager.data.jumpForce); // ����
            grounded = false;
        }
    }

    public void HandleMovement()
    {
        float inputX = Input.GetAxis("Horizontal");
        manager.rb.linearVelocity = new Vector2(inputX * manager.data.speed, manager.rb.linearVelocity.y); // ����

        // ���� ����
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
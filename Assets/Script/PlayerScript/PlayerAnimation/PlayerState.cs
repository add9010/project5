using UnityEngine;

public class PlayerStateController
{
    private PlayerManager pm;

    private enum PlayerState { Idle, Move, Jump, Roll, Hurt, Dead }
    private PlayerState currentState = PlayerState.Idle;

    private float delayToIdle;
    private bool grounded = true;

    public PlayerStateController(PlayerManager manager)
    {
        pm = manager;
    }

    public void Update()
    {
        if (currentState == PlayerState.Dead) return;

        HandleInput();
        UpdateStateLogic();
    }

    private void HandleInput()
    {
        float inputX = Input.GetAxisRaw("Horizontal");

        // ����
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            currentState = PlayerState.Jump;
            grounded = false;
            pm.animator.SetTrigger("Jump");
            pm.rb.linearVelocity = new Vector2(pm.rb.linearVelocity.x, pm.data.jumpForce);
            return;
        }

        // ������
        if (Input.GetKeyDown(KeyCode.LeftShift) && grounded)
        {
            currentState = PlayerState.Roll;
            pm.animator.SetTrigger("Roll");

            float dir = pm.spriteRenderer.flipX ? -1 : 1;
            pm.rb.linearVelocity = new Vector2(dir * pm.data.rollForce, 0f);

            // ������ �浹 ����
            pm.GetComponent<PlayerCollision>()?.IgnoreEnemyCollisions(true);
            pm.StartCoroutine(EndRollAfter(0.25f));
            return;
        }

        // �̵�
        if (Mathf.Abs(inputX) > 0.1f)
        {
            currentState = PlayerState.Move;
            return;
        }

        // �⺻ ����
        currentState = PlayerState.Idle;
    }

    private void UpdateStateLogic()
    {
        pm.animator.SetFloat("AirSpeedY", pm.rb.linearVelocity.y);

        switch (currentState)
        {
            case PlayerState.Idle:
                delayToIdle -= Time.deltaTime;
                if (delayToIdle < 0)
                    pm.animator.SetInteger("AnimState", 0);
                break;

            case PlayerState.Move:
                delayToIdle = 0.05f;
                pm.animator.SetInteger("AnimState", 1);
                break;
        }
    }

    private System.Collections.IEnumerator EndRollAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        pm.GetComponent<PlayerCollision>()?.IgnoreEnemyCollisions(false);
        currentState = PlayerState.Idle;
    }

    public void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground") || col.gameObject.CompareTag("Platform"))
            grounded = true;
    }
}
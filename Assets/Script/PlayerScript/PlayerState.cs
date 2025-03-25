using UnityEngine;

public class PlayerStateController
{
    private PlayerManager pm;

    private enum PlayerState { Idle, Move, Jump, Attack, Roll, Hurt, Dead, Dialog }
    private PlayerState currentState = PlayerState.Idle;

    public PlayerStateController(PlayerManager manager)
    {
        this.pm = manager;
    }

    public void UpdateState(float horizontalInput, bool isGrounded, bool isAttacking)
    {
        if (pm.isAction)
        {
            SetState(PlayerState.Dialog);
            return;
        }

        if (!isGrounded)
        {
            SetState(PlayerState.Jump);
        }
        else if (isAttacking)
        {
            SetState(PlayerState.Attack);
        }
        else if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            SetState(PlayerState.Move);
        }
        else
        {
            SetState(PlayerState.Idle);
        }

        UpdateAnimator(horizontalInput, isGrounded, pm.rb.linearVelocity.y);
    }

    private void SetState(PlayerState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
    }
    public void SetGrounded(bool isGrounded)
    {
        // 이 grounded는 상태 판단에 사용됨 (점프 → 착지 등)
        pm.animator.SetBool("Grounded", isGrounded);
    }

    private void UpdateAnimator(float horizontal, bool grounded, float verticalVelocity)
    {
        pm.animator.SetFloat("AirSpeedY", verticalVelocity);
        pm.animator.SetBool("Grounded", grounded);

        switch (currentState)
        {
            case PlayerState.Idle:
                pm.animator.SetInteger("AnimState", 0);
                break;
            case PlayerState.Move:
                pm.animator.SetInteger("AnimState", 1);
                break;
            case PlayerState.Jump:
                pm.animator.SetTrigger("Jump");
                break;
            case PlayerState.Attack:
                // 공격 애니메이션은 Trigger 기반으로 PlayerAttack에서 처리
                break;
            case PlayerState.Roll:
                pm.animator.SetTrigger("Roll");
                break;
            case PlayerState.Dialog:
                pm.animator.SetInteger("AnimState", 0);
                break;
        }
    }
}
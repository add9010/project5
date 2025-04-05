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
        }
        else if (!isGrounded)
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

        // 항상 호출!
        UpdateAnimator(horizontalInput, isGrounded, pm.rb.linearVelocity.y);
    }

    private void SetState(PlayerState newState)
    {
        if (currentState == newState) return;

        // 상태 전환 로그도 있으면 디버그에 좋아
        Debug.Log($"상태 전환: {currentState} → {newState}");

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
            case PlayerState.Dialog: // 둘 다 0번 상태로 고정
                pm.animator.SetInteger("AnimState", 0);
                break;
            case PlayerState.Move:
                pm.animator.SetInteger("AnimState", 1);
                break;
            case PlayerState.Jump:
                pm.animator.SetTrigger("Jump");
                break;
            case PlayerState.Attack:
                break;
            case PlayerState.Roll:
                pm.animator.SetTrigger("Roll");
                break;
        }
    }

    public AnimType GetAnimType()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
            case PlayerState.Dialog:
                return AnimType.Idle;
            case PlayerState.Move:
                return AnimType.Run;
            case PlayerState.Jump:
                return AnimType.Jump;
            case PlayerState.Attack:
                return AnimType.Attack;
            case PlayerState.Roll:
                return AnimType.Roll;
            case PlayerState.Hurt:
                return AnimType.Hit;
            case PlayerState.Dead:
                return AnimType.Die;
            default:
                return AnimType.Idle;
        }
    }



}

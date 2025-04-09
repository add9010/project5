using System;
using UnityEngine;

public class PlayerStateController
{
    private PlayerManager pm;

    private enum PlayerState { Idle, Move, Jump, Fall, Attack, AirAttack, Dash, Hurt, Dead, Dialog }
    private PlayerState currentState = PlayerState.Idle;
    private float jumpTimer = 0f;
    private const float fallTransitionTime = 0.2f; // 점프 후 이 시간 지나면 Fall로 간주

    public PlayerStateController(PlayerManager manager)
    {
        this.pm = manager;
    }

    public void UpdateState(float horizontalInput, bool isGrounded, bool isAttacking)
    {
        // 공격이 최우선
        if (isAttacking)
        {
            SetState(PlayerState.Attack); // 공중 공격도 이걸로 처리
        }
        else if (pm.isDashing)
        {
            SetState(PlayerState.Dash);
        }
        else if (pm.isAction)
        {
            SetState(PlayerState.Dialog);
        }
        else if (!isGrounded)
        {
            jumpTimer += Time.deltaTime;
            if (jumpTimer >= fallTransitionTime)
                SetState(PlayerState.Fall);
            else
                SetState(PlayerState.Jump);
        }
        else
        {
            jumpTimer = 0f; // 착지하면 초기화

            if (Mathf.Abs(horizontalInput) > 0.1f)
                SetState(PlayerState.Move);
            else
                SetState(PlayerState.Idle);
        }

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
        pm.GetAnimator().SetBool("Grounded", isGrounded);
    }
    public void ForceSetDash()
    {
        SetState(PlayerState.Dash);
        pm.GetAnimator().SetTrigger("Dash"); 
    }
    public void SetHurt()
    {
        SetState(PlayerState.Hurt);
        pm.GetAnimator().SetTrigger("Hurt");
    }

    private void UpdateAnimator(float horizontal, bool grounded, float verticalVelocity)
    {
        pm.GetAnimator().SetFloat("AirSpeedY", verticalVelocity);
        pm.GetAnimator().SetBool("Grounded", grounded);

        switch (currentState)
        {
            case PlayerState.Idle:
            case PlayerState.Dialog: // 둘 다 0번 상태로 고정
                pm.GetAnimator().SetInteger("AnimState", 0);
                break;
            case PlayerState.Move:
                pm.GetAnimator().SetInteger("AnimState", 1);
                break;
            case PlayerState.Jump:
                pm.GetAnimator().SetTrigger("Jump");
                break;
            case PlayerState.Attack:
                break;
            case PlayerState.Dash:
#warning 대쉬 문제의 원인

              //  Debug.Log($">> 현재 애니메이션 상태:{pm.GetAnimator().GetCurrentAnimatorStateInfo(0).IsName("Dash")}");
                if (pm.GetAnimator().GetCurrentAnimatorStateInfo(0).IsName("Dash") == false)
                    pm.GetAnimator().SetTrigger("Dash");
                break;
            case PlayerState.Hurt:
                pm.GetAnimator().SetTrigger("Hurt");
                break;
            case PlayerState.Fall:
                pm.GetAnimator().SetInteger("AnimState", 4); // 예시: Fall 상태 애니메이션 인덱스
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
            case PlayerState.Dash:
                return AnimType.Dash;
            case PlayerState.Hurt:
                return AnimType.Hit;
            case PlayerState.Dead:
                return AnimType.Die;
            default:
                return AnimType.Idle;
        }
    }



}

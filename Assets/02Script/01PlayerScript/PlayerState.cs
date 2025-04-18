using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateController
{
    private PlayerManager pm;

    private enum PlayerState { Idle, Move, Jump, Fall, Attack, AirAttack, Dash, Hurt, Dead, Dialog, Skill }
    private PlayerState currentState = PlayerState.Idle;
    private float jumpTimer = 0f;
    private const float fallTransitionTime = 0.1f; // 점프 후 이 시간 지나면 Fall로 간주

    private Dictionary<PlayerState, Func<bool>> stateTransitions;

    public PlayerStateController(PlayerManager manager)
    {
        this.pm = manager;
        stateTransitions = new Dictionary<PlayerState, Func<bool>>
        {
            { PlayerState.Attack, () => pm.playerAttack.IsAttacking },
            { PlayerState.Dash, () => pm.isDashing },
            { PlayerState.Dialog, () => pm.isAction },
            { PlayerState.Fall, () => !pm.groundSensor.State() && jumpTimer >= fallTransitionTime },
            { PlayerState.Jump, () => !pm.groundSensor.State() && jumpTimer < fallTransitionTime },
            { PlayerState.Move, () => pm.groundSensor.State() && Mathf.Abs(pm.horizontalInput) > 0.1f },
            { PlayerState.Idle, () => pm.groundSensor.State() && Mathf.Abs(pm.horizontalInput) <= 0.1f }
        };
    }

    public void UpdateState(float horizontalInput, bool isGrounded, bool isAttacking)
    {
        var animState = pm.GetAnimator().GetCurrentAnimatorStateInfo(0);

        // 애니메이션 갱신은 항상 해야 함!
        UpdateAnimator(horizontalInput, isGrounded, pm.rb.linearVelocity.y);

        // 상태 전이 차단 조건
        if (currentState == PlayerState.Skill)
        {
            // Skill1 애니메이션이 끝났는지 확인
            if (animState.IsName("Skill1") && animState.normalizedTime >= 1.0f)
            {
                // 상태를 Idle로 복원
                SetState(PlayerState.Idle);
            }
            return;
        }

        // 상태 전이 로직
        pm.horizontalInput = horizontalInput;
        jumpTimer = !isGrounded ? jumpTimer + Time.deltaTime : 0f;

        foreach (var transition in stateTransitions)
        {
            if (transition.Value.Invoke())
            {
                SetState(transition.Key);
                break;
            }
        }
    }

    private void SetState(PlayerState newState)
    {
        if (currentState == newState) return;

        // 상태 전환 로그도 있으면 디버그에 좋아
#if UNITY_EDITOR
        Debug.Log($"상태 전환: {currentState} → {newState}");
#endif

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
    public void ForceSetSkill()
    {
        SetState(PlayerState.Skill);
        pm.GetAnimator().SetTrigger("Skill1");
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
                if (pm.GetAnimator().GetCurrentAnimatorStateInfo(0).IsName("Jump") == false)
                pm.GetAnimator().SetTrigger("Jump");
                break;
            case PlayerState.Attack:
                break;
            case PlayerState.Dash:
//#warning 대쉬 문제의 원인

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

using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateController
{
    private PlayerManager pm;

    private enum PlayerState { Idle, Move, Jump, Fall, Attack, AttackJP, Dash, Hurt, Dead, Dialog, Skill, Parry }
    private PlayerState currentState = PlayerState.Idle;
    private float jumpTimer = 0f;
    private const float fallTransitionTime = 0.1f; // 점프 후 이 시간 지나면 Fall로 간주
    private float skillLockTimer = 0f;
    private const float skillLockDuration = 0.5f; // Skill 애니메이션 길이에 맞춤
    private AnimType currentSkillAnimType = AnimType.Idle;

    private Dictionary<PlayerState, Func<bool>> stateTransitions;

    public PlayerStateController(PlayerManager manager)
    {
        this.pm = manager;
        stateTransitions = new Dictionary<PlayerState, Func<bool>>
        {
            { PlayerState.Attack, () => pm.playerAttack.IsAttacking && pm.isGrounded },
            { PlayerState.AttackJP, () => pm.playerAttack.IsAttacking && !pm.isGrounded },
            { PlayerState.Dash, () => pm.isDashing },
            { PlayerState.Dialog, () => pm.isAction },
            { PlayerState.Jump, () => !pm.groundSensor.State() && jumpTimer < fallTransitionTime },
            { PlayerState.Fall, () => !pm.groundSensor.State() && jumpTimer >= fallTransitionTime },
            { PlayerState.Move, () => pm.groundSensor.State() && Mathf.Abs(pm.horizontalInput) > 0.1f },
            { PlayerState.Idle, () => pm.groundSensor.State() && Mathf.Abs(pm.horizontalInput) <= 0.1f }
        };
    }

    public void UpdateState(float horizontalInput, bool isGrounded, bool isAttacking)
    {
        if (pm.IsDead)
        {
            UpdateAnimator(horizontalInput, isGrounded, pm.rb.linearVelocity.y);
            return;
        }
        if (skillLockTimer > 0f)
        {
            skillLockTimer -= Time.deltaTime;
            return;
        }
        var animState = pm.GetAnimator().GetCurrentAnimatorStateInfo(0);

        UpdateAnimator(horizontalInput, isGrounded, pm.rb.linearVelocity.y);
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
    public void LockSkillState(float duration)
    {
        skillLockTimer = duration;
    }
    private void SetState(PlayerState newState)
    {
        if (currentState == newState) return;

        // 상태 전환 로그도 있으면 디버그에 좋아
#if UNITY_EDITOR
        //Debug.Log($"상태 전환: {currentState} → {newState}");
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
    public void ForceSetSkill(string triggerName, AnimType animType)
    {
        SetState(PlayerState.Skill);
        pm.GetAnimator().SetTrigger(triggerName);
        pm.SetAnimType(animType); // 이 함수는 PlayerManager에 직접 추가 필요
        currentSkillAnimType = animType;
    }
    public void SetHurt()
    {
        if (currentState == PlayerState.Skill)
            return;
        SetState(PlayerState.Hurt);
        pm.GetAnimator().SetTrigger("Hurt");
    }
    public void ForceSetDead()
    {
        SetState(PlayerState.Dead);
        pm.GetAnimator().SetTrigger("Dead");
    }
    public void ForceSetParry()
    {
        SetState(PlayerState.Parry);
        pm.GetAnimator().SetTrigger("Parry");
    }
    public void ForceSetIdle()
    {
        SetState(PlayerState.Idle);
    }

    private void UpdateAnimator(float horizontal, bool grounded, float verticalVelocity)
    {
        pm.GetAnimator().SetFloat("AirSpeedY", verticalVelocity);
        pm.GetAnimator().SetBool("Grounded", grounded);

        if (currentState == PlayerState.Skill || currentState == PlayerState.Parry)
            return;

        switch (currentState)
        {
            case PlayerState.Idle:
            case PlayerState.Dialog:
                pm.GetAnimator().SetInteger("AnimState", 0);
                break;
            case PlayerState.Move:
                pm.GetAnimator().SetInteger("AnimState", 1);
                break;
            case PlayerState.Jump:
                if (!pm.GetAnimator().GetCurrentAnimatorStateInfo(0).IsName("Jump"))
                    pm.GetAnimator().SetTrigger("Jump");
                break;
            case PlayerState.Dash:
                if (!pm.GetAnimator().GetCurrentAnimatorStateInfo(0).IsName("Dash"))
                    pm.GetAnimator().SetTrigger("Dash");
                break;
            case PlayerState.Hurt:
                break;
            case PlayerState.Attack:
                break;
            case PlayerState.AttackJP:
                break;

            case PlayerState.Fall:
                pm.GetAnimator().SetInteger("AnimState", 4);
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
                return AnimType.Dead;
            case PlayerState.Skill:
                return currentSkillAnimType;
            //case PlayerState.Parry:
            //return AnimType.Parry;
            //case PlayerState.Attack:
               // return pm.isGrounded ? AnimType.Attack : AnimType.AttackJP;
            default:
                return AnimType.Idle;
        }

    }

    }

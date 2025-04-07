using UnityEngine;

public class RemoteAnimation : MonoBehaviour
{
    private Animator animator;
    private AnimType currentAnimType = AnimType.Idle;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void SetGroundedState(AnimType stateName)
    {
        bool isGrounded = stateName == AnimType.Idle || stateName == AnimType.Run;
        animator.SetBool("Grounded", isGrounded);
    }
    public void SetGrounded(bool grounded)
    {
        animator.SetBool("Grounded", grounded);
    }
    public void PlayAnimation(AnimType stateName)
    {
        if (animator == null)
        {
            Debug.LogWarning("[RemoteAnimation] Animator가 null입니다.");
            return;
        }

        if (currentAnimType == stateName) return; // 같은 상태 반복 방지

        Debug.Log($"[RemoteAnimation] 애니 재생 시도: {stateName}");

        SetGroundedState(stateName);

        switch (stateName)
        {
            case AnimType.Idle:
                animator.SetInteger("AnimState", 0);
                break;
            case AnimType.Run:
                animator.SetInteger("AnimState", 1);
                break;
            case AnimType.Jump:
                animator.SetTrigger("Jump");
                break;
            case AnimType.Attack:
                animator.SetTrigger("Attack1");
                break;
            case AnimType.Roll:
                animator.SetTrigger("Roll");
                break;
            case AnimType.Hit:
                animator.SetTrigger("Hit");
                break;
            case AnimType.Die:
                animator.SetTrigger("Die");
                break;
            default:
                animator.SetInteger("AnimState", 0);
                break;
        }

        currentAnimType = stateName;
    }
}

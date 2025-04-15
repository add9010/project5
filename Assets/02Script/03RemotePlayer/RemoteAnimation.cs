using UnityEngine;

public class RemoteAnimation : MonoBehaviour
{
    private Animator animator;
    private AnimType currentAnimType = AnimType.Idle;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAnimation(AnimType stateName)
    {
        if (animator == null || currentAnimType == stateName)
            return;

        Debug.Log($"[RemoteAnimation] 애니 재생: {stateName}");

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
            case AnimType.Dash:
                animator.SetTrigger("Dash");
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

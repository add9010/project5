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
        if (animator == null) return;

        if (stateName != currentAnimType) // 중복 재생 방지
        {
            animator.Play(stateName.ToString()); // enum → string ("Run", "Idle" 등)
            currentAnimType = stateName;
        }
    }
}

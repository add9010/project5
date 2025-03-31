using UnityEngine;

public class RemoteAnimation : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAnimation(string stateName)
    {
        if (!string.IsNullOrEmpty(stateName))
        {
            animator.Play(stateName);
        }
    }
}
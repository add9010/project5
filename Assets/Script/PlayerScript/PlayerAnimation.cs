using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private PlayerManager manager;
    private float delayToIdle = 0.0f;

    private void Start()
    {
        manager = PlayerManager.Instance;
    }

    void Update()
    {
        HandleAnimationState();
    }

    private void HandleAnimationState()
    {
        float inputX = Input.GetAxis("Horizontal");

        if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            delayToIdle = 0.05f;
            manager.animator.SetInteger("AnimState", 1);
        }
        else
        {
            delayToIdle -= Time.deltaTime;
            if (delayToIdle < 0)
                manager.animator.SetInteger("AnimState", 0);
        }

        manager.animator.SetFloat("AirSpeedY", manager.rb.linearVelocity.y);
    }
}
using UnityEngine;

public enum BossState : byte
{
    IDLE = 0x00,  // 대기
    LFD = 0x01,   // 왼손 내려치기
    RFD = 0x02,   // 오른손 내려치기
    DEAD = 0x03   // 죽음
}

public class BossManager : MonoBehaviour
{
    public static BossManager Instance;

    private Animator _leftHandAnimator;
    private Animator _rightHandAnimator;
    private Animator _bodyAnimator;
    private Animator _deathAnimator;

    private void Awake()
    {
        Instance = this;
        _leftHandAnimator = transform.Find("LeftHand").GetComponent<Animator>();
        _rightHandAnimator = transform.Find("RightHand").GetComponent<Animator>();
    }

    public void ApplyBossState(BossState state)
    {
        switch (state)
        {
            case BossState.IDLE:
                PlayIdle();
                break;
            case BossState.LFD:
                MainThreadDispatcher.RunOnMainThread(() => PlayLeftFistDown());
                break;
            case BossState.RFD:
                MainThreadDispatcher.RunOnMainThread(() => PlayRightFistDown());
                break;
            case BossState.DEAD:
                MainThreadDispatcher.RunOnMainThread(() => PlayDeath());
                break;
            default:
                Debug.LogWarning($"Unknown boss state: {state}");
                break;
        }
    }

    private void PlayLeftFistDown()
    {
        Debug.Log("보스: 왼손 내려치기!");
        _leftHandAnimator.SetBool("LeftFistDown", true);

        Invoke("ResetLeftFistDown", 1f);

    }

    private void PlayRightFistDown()
    {
        Debug.Log("보스: 오른손 내려치기!");
        _rightHandAnimator.SetBool("RightFistDown", true);

        Invoke("ResetRightFistDown", 1f);
    }

    private void PlayIdle()
    {
        Debug.Log("보스: 정지");
       
    }
    private void PlayDeath()
    {
        Debug.Log("보스: 사망!");
        _deathAnimator.SetTrigger("Die");
    }

    // 1초 후 "LeftFistDown"을 false로 설정
    private void ResetLeftFistDown()
    {
        _leftHandAnimator.SetBool("LeftFistDown", false);
    }

    // 1초 후 "RightFistDown"을 false로 설정
    private void ResetRightFistDown()
    {
        _rightHandAnimator.SetBool("RightFistDown", false);
    }
}

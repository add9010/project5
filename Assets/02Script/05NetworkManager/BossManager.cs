using UnityEngine;
using UnityEngine.UI;

public enum BossState : byte
{
    IDLE = 0x00,  // 대기
    LeftFistDown = 0x01,   // 왼손 내려치기
    RightFistDown = 0x02,   // 오른손 내려치기
    AllFistDown = 0x03,   // 양손 내려치기
    DEAD = 0x04   // 죽음
}

public class BossManager : MonoBehaviour
{
    public static BossManager Instance;

    private Animator _leftHandAnimator;
    private Animator _rightHandAnimator;
    private Animator _bodyAnimator;


    [Header("Boss HP Bar")]
    public GameObject prfHpBar;
    public GameObject canvas;
    private RectTransform hpBar;
    private Image nowHpbar;
    public float height = 5f;

    [Header("Boss Stats")]
    public int maxHp = 3000;
    public int nowHp = 3000;

    private void Awake()
    {
        Instance = this;
        _leftHandAnimator = transform.Find("LeftHand").GetComponent<Animator>();
        _rightHandAnimator = transform.Find("RightHand").GetComponent<Animator>();
        _bodyAnimator = transform.Find("Head").GetComponent<Animator>();
    }


    private void Start()
    {
        if (prfHpBar != null && canvas != null)
        {
            hpBar = Instantiate(prfHpBar, canvas.transform).GetComponent<RectTransform>();
            nowHpbar = hpBar.transform.GetChild(0).GetComponent<Image>();
        }
    }

    private void Update()
    {
        if (hpBar != null)
        {
            Vector3 hpBarPos = Camera.main.WorldToScreenPoint
                (new Vector3(transform.position.x, transform.position.y + height, 0));
            hpBar.position = hpBarPos;
        }
    }

    public void UpdateBossHp(int newHp)
    {
        MainThreadDispatcher.RunOnMainThread(() =>
        {
            nowHp = newHp;

            if (nowHp <= 0)
            {
                nowHp = 0;
                ApplyBossState(BossState.DEAD);
            }

            if (hpBar != null)
            {
                nowHpbar.fillAmount = (float)nowHp / maxHp;
            }
        });
    }


    public void ApplyBossState(BossState state)
    {
        switch (state)
        {
            case BossState.IDLE:
                PlayIdle();
                break;
            case BossState.LeftFistDown:
                MainThreadDispatcher.RunOnMainThread(() => PlayLeftFistDown());
                break;
            case BossState.RightFistDown:
                MainThreadDispatcher.RunOnMainThread(() => PlayRightFistDown());
                break;
            case BossState.AllFistDown:
                MainThreadDispatcher.RunOnMainThread(() => PlayAllFistDown());
                break;
            case BossState.DEAD:
                MainThreadDispatcher.RunOnMainThread(() => PlayDeath());
                break;
            default:
                Debug.LogWarning($"Unknown boss state: {state}");
                break;
        }
    }

    private void PlayAllFistDown()
    {
        Debug.Log("보스: 양손 내려치기!");
        _leftHandAnimator.SetBool("LeftFistDown", true);
        _rightHandAnimator.SetBool("RightFistDown", true);

        Invoke("ResetLeftFistDown", 1f);
        Invoke("ResetRightFistDown", 1f);
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
       _bodyAnimator.SetBool("isDead", true);
        _leftHandAnimator.SetBool("LeftFistDown", false);
        _rightHandAnimator.SetBool("RightFistDown", false);

        if (hpBar != null) Destroy(hpBar.gameObject);
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

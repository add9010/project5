using UnityEngine;
using UnityEngine.UI;

public enum BossState : byte
{
    IDLE = 0x00,           // 대기
    LeftFistDown = 0x01,   // 왼손 내려치기
    RightFistDown = 0x02,  // 오른손 내려치기
    AllFistDown = 0x03,    // 양손 내려치기
    Shout = 0x04,          // 외침 (추후 확장 가능)
    LeftWield = 0x05,      // 왼손 휘두르기
    DEAD = 0x06,           // 죽음  
}

public class BossManager : MonoBehaviour
{
    public static BossManager Instance;

    private Animator _leftHandAnimator;
    private Animator _rightHandAnimator;
    private Animator _bodyAnimator;

    [Header("Meteor Settings")]
    public GameObject meteorPrefab;      // 운석 프리팹
    public Vector2 meteorSpawnStart = new Vector2(-25f, 10f);  // 시작 위치 (X축 기준)
    public float meteorXSpacing = 4f;    // 운석 간 X 간격
    public int meteorCount = 6;          // 운석 개수


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
            case BossState.LeftWield:
                MainThreadDispatcher.RunOnMainThread(() => PlayLeftWield());
                break;
            case BossState.RightFistDown:
                MainThreadDispatcher.RunOnMainThread(() => PlayRightFistDown());
                break;
            case BossState.AllFistDown:
                MainThreadDispatcher.RunOnMainThread(() => PlayAllFistDown());
                break;
            case BossState.Shout:
                MainThreadDispatcher.RunOnMainThread(() => PlayShout());
                break;
            case BossState.DEAD:
                MainThreadDispatcher.RunOnMainThread(() => PlayDeath());
                break;
            default:
                Debug.LogWarning($"Unknown boss state: {state}");
                break;
        }
    }

    private void PlayShout()
    {
        Debug.Log("보스: 외침!");

        _bodyAnimator.SetBool("Shout", true);
        SpawnMeteors();

        Invoke("ResetShout", 0.2f);  // 예: 2초 후에 외침을 종료하고 다른 상태로 변경
    }

    private void PlayAllFistDown()
    {
        Debug.Log("보스: 양손 내려치기!");
        _leftHandAnimator.SetBool("LeftFistDown", true);
        _rightHandAnimator.SetBool("RightFistDown", true);

        Invoke("ResetLeftFistDown", 0.2f);
        Invoke("ResetRightFistDown", 0.2f);
    }

    private void PlayLeftFistDown()
    {
        Debug.Log("보스: 왼손 내려치기!");
        _leftHandAnimator.SetBool("LeftFistDown", true);

        Invoke("ResetLeftFistDown", 0.2f);
    }

    private void PlayLeftWield()
    {
        Debug.Log("보스: 왼손 휘두르기!");

        _bodyAnimator.SetBool("LeftTwinkle", true);
        _leftHandAnimator.SetBool("LeftWield", true);

        Invoke("ResetLeftTwinkle", 0.2f);
        Invoke("ResetLeftWield", 0.2f);
    }

    private void PlayRightFistDown()
    {
        Debug.Log("보스: 오른손 내려치기!");
        _rightHandAnimator.SetBool("RightFistDown", true);

        Invoke("ResetRightFistDown", 0.2f);
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

    private void ResetShout()
    {
        // 외침이 끝난 후 할 작업 (예: 기본 상태로 복귀)
        _bodyAnimator.SetBool("Shout", false);
    }

    private void ResetLeftTwinkle()
    {
        // 외침이 끝난 후 할 작업 (예: 기본 상태로 복귀)
        _bodyAnimator.SetBool("LeftTwinkle", false);
    }

    // 1초 후 "LeftFistDown"을 false로 설정
    private void ResetLeftFistDown()
    {
        _leftHandAnimator.SetBool("LeftFistDown", false);
    }

    private void ResetLeftWield()
    {
        _leftHandAnimator.SetBool("LeftWield", false);
    }

    // 1초 후 "RightFistDown"을 false로 설정
    private void ResetRightFistDown()
    {
        _rightHandAnimator.SetBool("RightFistDown", false);
    }

    private void SpawnMeteors()
    {
        for (int i = 0; i < meteorCount; i++)
        {
            float spawnX = meteorSpawnStart.x + i * meteorXSpacing;
            Vector2 spawnPos = new Vector2(spawnX, meteorSpawnStart.y);

            Instantiate(meteorPrefab, spawnPos, Quaternion.identity);
        }
    }
}

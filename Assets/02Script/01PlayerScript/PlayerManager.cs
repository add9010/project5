using UnityEngine;
using System.Collections;
using TMPro;
using System.Runtime.CompilerServices;
using UnityEngine.SceneManagement;
using System.Linq;
public class PlayerManager : MonoBehaviour, IDamageable, IKnockbackable
{
    public static PlayerManager Instance;

    [Header("데이터")]
    public PlayerData data;

    [Header("컴포넌트")]
    [SerializeField] private Animator animator;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public CameraController cameraController;

    [Header("UI")]
    public UnityEngine.UI.Image hpbar;

    [Header("공격 위치")]
    public Transform attackPos;

    [Header("공격 사운드")]
    public AudioClip attackSFX1;  // 1~2타용
    public AudioClip attackSFX3;  // 3타용

    [Header("이동 사운드")]
    public AudioClip walkSFX;
    private float walkSFXTimer = 0f;
    private float walkStartBuffer = 0f;

    [Header("대시 사운드")]
    public AudioClip dashSFX;

    [Header("패링 사운드")]
    public AudioClip parrySuccessSFX;
    public AudioClip parryFailSFX; // (선택사항)

    [Header("센서")]
    public PlayerSensor groundSensor;

    [Header("마나 상태")]
    public int currentMana { get; private set; }
    private float manaTimer = 0f;

    public PlayerHealth playerHealth { get; private set; }
    public PlayerMove playerMove { get; private set; }
    public PlayerAttack playerAttack { get; private set; }
    public PlayerStateController playerStateController { get; private set; }
    public PlayerDash playerDash { get; private set; }
    public bool IsDead { get; private set; } = false;
    public bool CanDoubleJump { get; set; } = false;
    public float horizontalInput { get; set; }

    public PlayerParry playerParry { get; private set; }

    private AnimType currentAnimType = AnimType.Idle;

    [Header("대화")]
    //public DialogManager dialog;
    public PlayerDialog playerDialog { get; private set; }
    public bool isGrounded => groundSensor != null && groundSensor.State();
    public bool isAction = false;
    public bool isDashing = false;

    private float staggerTimer = 0f;
    private bool isStaggered = false;
    private static bool reconnected = false;
    [SerializeField] private string[] visibleInScenes = { "VillageStage", "RiverStage", "Boss1", "GolemStage" }; // 원하는 씬만 보여지게
    public bool canControl = true;
    public GameObject hitEffectPrefab; // 이펙트 프리팹을 인스펙터에서 할당
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 💡 씬 전환 시 파괴되지 않음
        }
        else
        {
            Destroy(gameObject);  // 중복 생성 방지
        }

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (attackPos == null)
        {
            Transform found = transform.Find("melee");
            if (found != null)
            {
                attackPos = found;
            }
            else
            {
                Debug.LogError("⚠️ PlayerManager: 'melee' 오브젝트를 찾을 수 없습니다!");
            }
        }
        SceneManager.sceneLoaded += OnSceneLoaded;

        gameObject.SetActive(false); // 시작 시 꺼둔다
        //if (DialogManager.Instance != null)
        //{
        //    dialog = DialogManager.Instance;
        //}
        //else
        //{
        //    // 혹시 예외 상황이라면 FindObjectOfType 로 후속 처리
        //    dialog = FindObjectOfType<DialogManager>();
        //    if (dialog == null)
        //        Debug.LogError("PlayerManager: DialogManager를 찾을 수 없습니다!");
        //}
    }

    private void Start()
    {
        playerStateController = new PlayerStateController(this);
        playerAttack = new PlayerAttack(this);
        playerHealth = new PlayerHealth(this);
        playerMove = new PlayerMove(this);
        playerDialog = new PlayerDialog(this);
        playerDash = new PlayerDash(this);
        playerParry = new PlayerParry(this);
        currentMana = data.maxMana;
        Camera.main.GetComponent<CameraController>().target = transform;

        // ✅ PYCanvas에서 HP바 자동 연결
        if (hpbar == null)
        {
            GameObject foundCanvas = GameObject.Find("PYCanvas");
            if (foundCanvas != null)
            {
                var foundHp = foundCanvas.transform.Find("hpbar") ?? foundCanvas.transform.Find("hpbar2/hpbar");
                if (foundHp != null)
                {
                    hpbar = foundHp.GetComponent<UnityEngine.UI.Image>();
                    Debug.Log("✅ PYCanvas에서 HP바 자동 연결 완료");
                }
                else
                {
                    Debug.LogWarning("⚠️ PYCanvas 안에서 hpbar 찾기 실패");
                }
            }
            else
            {
                Debug.LogWarning("⚠️ PYCanvas 자체를 찾을 수 없음");
            }
        }
        StartCoroutine(WaitForPYCanvas());
    }

    private void Update()
    {
        if (IsDead) return;


        if (!reconnected)
        {
            if (Instance == null)
            {
                Instance = this;
                Debug.Log("✅ PlayerManager 재연결됨");
            }
            reconnected = true;
        }

        if (isStaggered)
        {
            staggerTimer -= Time.deltaTime;
            if (staggerTimer <= 0f)
            {
                isStaggered = false;
            }
            return;
        }


        // 체력바 UI 갱신
        UpdateHpUI(playerHealth.currentHealth);
        Vector2 input = playerMove.GetInput();
        float inputX = input.x;

        horizontalInput = inputX;
        bool grounded = groundSensor != null && groundSensor.State();
        bool isAttacking = playerAttack.IsAttacking;
        playerAttack.UpdateAttackPosition();
        playerAttack.Update();
        playerHealth.Update();
        playerParry.Update();
        if (isAction)
            rb.linearVelocity = Vector2.zero; // ← 완전히 멈춤

        // 상태 업데이트 (애니메이션 및 상태 판단)
        playerStateController.UpdateState(inputX, grounded, playerAttack.IsAttacking);


        // 점프 처리
        // 점프 처리 (대시 중에는 금지)
        if (!isAction && !isDashing && playerMove.TryJump())
            playerMove.DoJump();

        // 이동 처리
        if (!isAction && !isDashing)
            playerMove.Move(input);
        //  걷기 사운드 처리
        bool isWalking = !isAction && !isDashing && isGrounded && Mathf.Abs(rb.linearVelocity.x) > 0.1f;

        if (isWalking)
        {
            // 걸음 시작 시 약간의 지연 시간 부여
            walkStartBuffer += Time.deltaTime;

            if (walkStartBuffer >= 0.13f) // 0.15초 이상 연속으로 걷고 있어야 발소리 발생
            {
                walkSFXTimer -= Time.deltaTime;

                if (walkSFXTimer <= 0f)
                {
                    if (walkSFX != null)
                        SoundManager.Instance.PlaySFX(walkSFX);
                    walkSFXTimer = 0.4f;
                }
            }
        }
        else
        {
            walkSFXTimer = 0f;
            walkStartBuffer = 0f;
        }
        // 공격 처리
        if (!isAction && playerAttack.TryAttack())
            playerAttack.DoAttack();

        // 대시 처리 (항상 가능해야 함)
        if (!isAction && playerDash != null)
            playerDash.TryDash();
        // 대화 시스템
        playerDialog.HandleInput();
        playerDialog.HandleScan();
        UpdateManaRecharge();




    }

    public void SetAnimType(AnimType type)
    {
        currentAnimType = type;
    }
    public AnimType GetCurrentAnimState()
    {
        return playerStateController.GetAnimType();
    }
    public void MarkAsDead()
    {
        IsDead = true;
    }
    private void OnDrawGizmos()
    {
        if (playerAttack != null)
        {
            playerAttack.DrawGizmos();
        }
    }
    public void ResetPlayer()
    {
        IsDead = false;
    }

    public void UpdateHpUI(float currentHealth)
    {
        if (hpbar != null)
            hpbar.fillAmount = currentHealth / data.maxHealth;
    }

    public void StartAttackCoroutine(IEnumerator routine)
    {
        StartCoroutine(routine); // 이건 MonoBehaviour라 가능함
    }

    public void TakeDamage(float damage)
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }

    // IKnockbackable 인터페이스 구현
    public void ApplyKnockback(Vector2 direction, float force)
    {
        if (playerHealth != null)
        {
            playerHealth.ApplyKnockback(direction, force);
        }
    }

    private void OnDestroy()
    {
        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.SaveEquippedSkills();
            Debug.Log("🧠 Player 사라짐 → 스킬 자동 저장");
        }
    }
    public Animator GetAnimator(
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "")
    {
        //Debug.Log($">> TRACE_에니메이터 접근 위치 : {file}에서 {line}번째 줄, {member}에서 접근합니다.");

        return animator;
    }

    private IEnumerator WaitForPYCanvas()
    {
        // PYCanvas가 존재하고 활성화될 때까지 대기
        yield return new WaitUntil(() =>
        {
            GameObject canvas = GameObject.Find("PYCanvas");
            return canvas != null && canvas.activeInHierarchy;
        });

        GameObject foundCanvas = GameObject.Find("PYCanvas");
        if (foundCanvas != null)
        {
            var foundHp = foundCanvas.transform.Find("hpbar") ?? foundCanvas.transform.Find("hpbar2/hpbar");
            if (foundHp != null)
            {
                hpbar = foundHp.GetComponent<UnityEngine.UI.Image>();
                Debug.Log(" 지연 후 PYCanvas에서 HP바 자동 연결 완료");
            }
            else
            {
                Debug.LogWarning(" PYCanvas 내부에서 hpbar 찾기 실패");
            }
        }
    }
    public void ApplyStagger(float duration)
    {
        isStaggered = true;
        staggerTimer = duration;
        rb.linearVelocity = Vector2.zero;
    }

    private void UpdateManaRecharge()
    {
        if (currentMana < data.maxMana)
        {
            manaTimer += Time.deltaTime;
            if (manaTimer >= data.manaRechargeTime)
            {
                currentMana++;
                manaTimer = 0f;
                Debug.Log($"마나 회복됨: 현재 마나 {currentMana}/{data.maxMana}");
                // UI 갱신 추가 예정
            }
        }
    }
    public bool TryUseMana(int amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            Debug.Log($"마나 {amount} 소모됨. 현재 마나: {currentMana}/{data.maxMana}");
            return true;
        }
        Debug.Log("마나 부족!");
        return false;
    }
    public float GetManaRechargeProgress()
    {
        return Mathf.Clamp01(manaTimer / data.manaRechargeTime);
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        gameObject.SetActive(visibleInScenes.Contains(scene.name));

        // 카메라 재연결
        var cam = Camera.main?.GetComponent<CameraController>();
        if (cam != null)
        {
            cameraController = cam;
            cam.target = transform;
        }
       
        ResetAllPlayerState();
    }
    public void ResetAllPlayerState()
    {
        Debug.Log("[PlayerManager] 상태 초기화 시도");

        // 체력, 상태 초기화
        playerHealth?.ResetHealth();
        playerStateController?.ForceSetIdle();

        // 컨트롤 관련 상태
        isDashing = false;
        isAction = false;
        IsDead = false;
        canControl = true;

        rb.linearVelocity = Vector2.zero;
        Debug.Log($"[PlayerManager] 상태 초기화 완료 → IsDead: {IsDead}, canControl: {canControl}");
    }
}

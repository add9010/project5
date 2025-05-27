using UnityEngine;
using System.Collections;
using TMPro;
using System.Runtime.CompilerServices;

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

    [Header("센서")]
    public PlayerSensor groundSensor;


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

    private void Awake()
    {
        if (Instance == null) Instance = this;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
    }

    private void Update()
    {
        if (IsDead) return;
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

        // 공격 처리
        if (!isAction && playerAttack.TryAttack())
            playerAttack.DoAttack();

        // 대시 처리 (항상 가능해야 함)
        if (!isAction && playerDash != null)
            playerDash.TryDash();
        // 대화 시스템
        playerDialog.HandleInput();
        playerDialog.HandleScan();
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            playerStateController.SetGrounded(true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            playerStateController.SetGrounded(false);
        }
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

    public Animator GetAnimator(
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "")
    {
        //Debug.Log($">> TRACE_에니메이터 접근 위치 : {file}에서 {line}번째 줄, {member}에서 접근합니다.");

        return animator;
    }
}

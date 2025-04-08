using UnityEngine;
using System.Collections;
using TMPro;

public class PlayerManager : MonoBehaviour, IDamageable, IKnockbackable
{
    public static PlayerManager Instance;
    
    [Header("데이터")]
    public PlayerData data;

    [Header("컴포넌트")]
    public Animator animator;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public CameraShake cameraShake;

    [Header("UI")]
    //public GameObject prfHpBar;
    ////public GameObject canvas;
    //private RectTransform hpBar;
    //private UnityEngine.UI.Image nowHpbar;

    [Header("공격 위치")]
    public Transform attackPos;

    [Header("센서")]
    public Sensor_HeroKnight groundSensor;


    public PlayerHealth playerHealth { get; private set; }
    public PlayerMove playerMove { get; private set; }
    public PlayerAttack playerAttack { get; private set; }
    public PlayerStateController playerStateController { get; private set; }
    public PlayerDash playerDash { get; private set; }
    public bool IsDead { get; private set; } = false;
    public bool CanDoubleJump { get; set; } = false;
    
    [Header("대화")]
    public DialogManager dialog;
    public PlayerDialog playerDialog { get; private set; }
    public bool isAction = false;
    public bool isDashing = false;
    private void Awake()
    {
        if (Instance == null) Instance = this;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        //hpBar = Instantiate(prfHpBar, canvas.transform).GetComponent<RectTransform>();
        //nowHpbar = hpBar.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
        playerStateController = new PlayerStateController(this);  // 이렇게 수정
        playerAttack = new PlayerAttack(this);
        playerHealth = new PlayerHealth(this);
        playerMove = new PlayerMove(this); // ⬅️ 모듈화된 이동 클래스 사용
        playerDialog = new PlayerDialog(this);// 대화창    if (Camera.main != null)
        playerDash = new PlayerDash(this);
        Camera.main.GetComponent<CameraFollow>().target = transform;

    }

    private void Update()
    {
        if (IsDead) return;
        // 체력바 UI 갱신
        //Vector3 hpBarPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * data.heightOffset);
        //hpBar.position = hpBarPos;
        //UpdateHpUI(playerHealth.currentHealth);
        float inputX = playerMove.GetHorizontalInput();
        bool grounded = groundSensor != null && groundSensor.State();
        bool isAttacking = playerAttack.IsAttacking;
        playerAttack.UpdateAttackPosition();
        playerAttack.Update();

        if (isAction)
            rb.linearVelocity = Vector2.zero; // ← 완전히 멈춤

        // 상태 업데이트 (애니메이션 및 상태 판단)
        playerStateController.UpdateState(inputX, grounded, isAttacking);


        // 점프 처리
        // 점프 처리 (대시 중에는 금지)
        if (!isAction && !isDashing && playerMove.TryJump())
            playerMove.DoJump();

        // 이동 처리 (대시 중에는 금지)
        if (!isAction && !isDashing)
            playerMove.Move(inputX);

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

    //public void UpdateHpUI(float currentHealth)
    //{
    //    if (nowHpbar != null)
    //        nowHpbar.fillAmount = currentHealth / data.maxHealth;
    //}

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

    //public void SetCharacterAttribute(string attribute)
    //{
    //    switch (attribute)
    //    {
    //        case "speed":
    //            data.speed *= 1.3f;
    //            Debug.Log("이동 속도 1.3배 증가");
    //            break;
    //        case "attack":
    //            data.attackPower *= 1.5f;
    //            Debug.Log("공격력 1.5배 증가");
    //            break;
    //        case "health":
    //            data.maxHealth *= 1.3f;
    //            Debug.Log("체력 1.3배 증가");
    //            break;
    //        case "random":
    //            CanDoubleJump = true;
    //            Debug.Log("축하합니다! 더블점프 해금");
    //            break;
    //    }
    //}
}

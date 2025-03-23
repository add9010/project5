using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [Header("������")]
    public PlayerData data;

    [Header("������Ʈ")]
    public Animator animator;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public CameraShake cameraShake;

    [Header("ü�� UI")]
    public GameObject prfHpBar;
    public GameObject canvas;
    private RectTransform hpBar;
    private UnityEngine.UI.Image nowHpbar;

    [Header("���� ��ġ")]
    public Transform attackPos;

    public PlayerHealth playerHealth { get; private set; }
    public bool IsDead { get; private set; } = false;
    public bool CanDoubleJump { get; set; } = false;
    public bool CanPerformDoubleJump { get; set; } = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        // ������Ʈ ĳ��
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // ü�¹� UI ����
        hpBar = Instantiate(prfHpBar, canvas.transform).GetComponent<RectTransform>();
        nowHpbar = hpBar.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();

        // PlayerHealth �ʱ�ȭ
        playerHealth = new PlayerHealth(this);
    }

    void Update()
    {
        if (IsDead) return;

        // ü�¹� UI ��ġ ����
        Vector3 hpBarPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + data.heightOffset, 0));
        hpBar.position = hpBarPos;

        // UI ������Ʈ�� playerHealth ���ο��� ó����
    }

    public void MarkAsDead()
    {
        IsDead = true;
    }

    public void ResetPlayer()
    {
        IsDead = false;
    }

    public void UpdateHpUI(float currentHealth)
    {
        if (nowHpbar != null)
            nowHpbar.fillAmount = currentHealth / data.maxHealth;
    }

    public void SetCharacterAttribute(string attribute)
    {
        switch (attribute)
        {
            case "speed":
                data.speed *= 1.3f;
                Debug.Log("�̵� �ӵ� 1.3�� ����");
                break;
            case "attack":
                data.attackPower *= 1.5f;
                Debug.Log("���ݷ� 1.5�� ����");
                break;
            case "health":
                data.maxHealth *= 1.3f;
                Debug.Log("ü�� 1.3�� ����");
                break;
            case "random":
                CanDoubleJump = true;
                Debug.Log("�����մϴ�! �������� �ر�");
                break;
        }
    }
}
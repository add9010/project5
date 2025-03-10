using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;  // ���� ���� ��ũ��Ʈ ����
    public float maxSpeed;          // �÷��̾��� �ִ� �ӵ�
    public float jumpPower;         // ���� �� �������� ���� ũ��
    Rigidbody2D rigid;              // Rigidbody2D ������Ʈ ����
    SpriteRenderer spriteRenderer;  // SpriteRenderer ������Ʈ ����
    Animator anim;                  // Animator ������Ʈ ����

    // �ʱ�ȭ
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // ���� ó��
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);  // �������� ���� ����
            anim.SetBool("isJumping", true);  // ���� ���� ����
        }

        // ���� �̵� �ӵ� ���� (Ű�� ���� �� ����)
        if (Input.GetButtonUp("Horizontal"))
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.normalized.x * 0.5f, rigid.linearVelocity.y);

        // ���⿡ ���� ��������Ʈ ���� ó��
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        // �ȱ� �ִϸ��̼� ���� ��ȯ
        if (Mathf.Abs(rigid.linearVelocity.x) < 0.4f)
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);
    }

    void FixedUpdate()
    {
        // ���� �Է¿� ���� �� ���ϱ�
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        // �ִ� �ӵ� ����
        if (rigid.linearVelocity.x > maxSpeed)
            rigid.linearVelocity = new Vector2(maxSpeed, rigid.linearVelocity.y);
        else if (rigid.linearVelocity.x < -maxSpeed)
            rigid.linearVelocity = new Vector2(-maxSpeed, rigid.linearVelocity.y);

        // ���� �� ���� ���� ����
        if (rigid.linearVelocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector2.down, new Color(0, 1, 0)); // ����׿� ���� ǥ��
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)  // �÷����� ���� �Ÿ� �̳��̸� ���� ���� ����
                    anim.SetBool("isJumping", false);
            }
        }
    }

    // ������ �浹 ó��
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            // ���� ����� �� ���� ó��
            if (rigid.linearVelocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
            }
            // �׷��� ������ ���� ó��
            else
                OnDamaged(collision.transform.position);
        }
    }

    // ������ �Ǵ� �������� ������ �浹 ó��
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            // ������ ������ ���� ���� �߰�
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");
            if (isBronze)
                gameManager.stagePoint += 50;
            else if (isSilver)
                gameManager.stagePoint += 100;
            else if (isGold)
                gameManager.stagePoint += 300;

            // ������ ��Ȱ��ȭ
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "Finish")
        {
            gameManager.NextStage();  // �������� ��ȯ ȣ��
        }
    }

    // �� ���� ó��
    void OnAttack(Transform enemy)
    {
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);  // �������� �ݹ߷� ����
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();  // ���� ���� ó�� �޼��� ȣ��
    }

    // �÷��̾� ���� ó��
    void OnDamaged(Vector2 targetPos)
    {
        gameManager.health--;  // ü�� ����

        // ���� ���� ����
        gameObject.layer = 11;  // ���� ���̾�� ����
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);  // ������ ���� ����

        // �ݹ߷� ����
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1; // ���� ���� ���
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

        anim.SetTrigger("doDamaged");  // ���� �ִϸ��̼� Ʈ����
        Invoke("OffDamaged", 1.5f);  // ���� �ð� �� ���� ���� ����
    }

    // ���� ���� ����
    void OffDamaged()
    {
        gameObject.layer = 10;  // ���� ���̾�� ����
        spriteRenderer.color = new Color(1, 1, 1, 1);  // ���� ���� ����
    }
}

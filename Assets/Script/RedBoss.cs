using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class Boss : Enemy
{
    public delegate void BossDeathHandler();
    public event BossDeathHandler OnBossDeath;  // ������ �׾��� �� ȣ��� �̺�Ʈ

    public float specialAttackCooldown = 2f;                // Ư�� ����(���Ȱ���) ��ٿ�
    public float PerformAreaAttackRange; // �������ݹݰ�
    public GameObject attackEffectPrefab;                   // ���� ���� ����Ʈ Prefab�� ������ ����
    private bool canUseSpecialAttack = true;
  

    // Start �޼��带 override�� ����
    protected override void Start()
    {
        base.Start(); // �θ� Ŭ������ Start ȣ��

        // RedBoss ������ ���� ����
        SetEnemyStatus("���庸�� ŷ", 1000, 20); // ���� �ʱ�ȭ
        Debug.Log("RedBoss Initialized");

        PerformAreaAttackRange = detectionRange / 2; // �������ݹݰ�
    }

    void Update()
    {
        // �⺻ Enemy�� Update ��� ����
        base.Update();

        // ���� Ư�� �ൿ �߰�
        if (canUseSpecialAttack && !isEnemyDead)
        {
            StartCoroutine(UseSpecialAttack());
        }
    }

    protected override void SpawnMark()
    {
        Debug.Log("���� ������ŷ�� ����� �߰��߽��ϴ�!!");
        if (markPrefab != null)
        {
            // ��Ŀ�� ������ ��ġ�� ���� ��ġ���� markYOffset��ŭ Y������ �ø�
            Vector3 spawnPosition = transform.position + new Vector3(0, 3f, 0); // markYOffset ����ŭ Y������ �̵�
            GameObject markInstance = Instantiate(markPrefab, spawnPosition, Quaternion.identity);

            // ������ ��Ŀ�� ���ʹ̸� �����ϵ��� ����
            Mark markScript = markInstance.GetComponent<Mark>();
            if (markScript != null)
            {
                markScript.enemy = transform; // ��Ŀ�� ���ʹ̸� �����ϵ��� ����
            }
        }
    }


    private IEnumerator UseSpecialAttack()
    {
        canUseSpecialAttack = false;

        // Ư�� ���� ���� (��: ���� ����)
        Debug.Log("���� ������ŷ�� ���� ������ �غ��մϴ�.");
        PerformAreaAttack();

        // ��ٿ� ���
        yield return new WaitForSeconds(specialAttackCooldown);
        canUseSpecialAttack = true;
    }


    private void PerformAreaAttack()
    {
        // ���� ���� �ִ� ��� Collider2D�� Ž��
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, PerformAreaAttackRange);


        // ���� ������ ����Ʈ ���� (���� ��ü�� ǥ��)
        if (attackEffectPrefab != null)
        {
            GameObject effectInstance = Instantiate(attackEffectPrefab, transform.position, Quaternion.identity);
            effectInstance.transform.localScale = new Vector3(detectionRange, PerformAreaAttackRange, 1f); // X, Y ũ�⸦ detectionRange�� ����
        }

        foreach (Collider2D hitObject in hitObjects)
        {
            // �÷��̾� �±׸� ���� ������Ʈ���� Ȯ��
            if (hitObject.CompareTag("Player"))
            {
                // �÷��̾��� ��ũ��Ʈ ��������
                HeroKnightUsing playerScript = hitObject.GetComponent<HeroKnightUsing>();

                if (playerScript != null && !playerScript.isDead)
                {   
                    playerScript.TakeDamage(atkDmg * 2);
                    Debug.Log($"���庸���� {hitObject.name}���� {atkDmg * 2}�� Ư�� �������� �������� �������ϴ�!");

                   
                }
            }
        }
    }



    // Gizmo: Ư�� ���� ������ �ð������� ǥ��
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();  // �⺻ Enemy�� �ð��� ���� ǥ��
        Gizmos.color = Color.yellow;  // Ư�� ���� ���� ����
        Gizmos.DrawWireSphere(transform.position, detectionRange);  // Ư�� ���� ����
    }

    // ���� �׾��� �� ȣ��Ǵ� �Լ�
    protected override void HandleWhenDead()
    {
        base.HandleWhenDead();  // �⺻ Enemy�� ���� ó��

       
        // ���� ���� �̺�Ʈ �߻�
        OnBossDeath?.Invoke();

        DropSpecialLoot();
    }

    // ���� ���� Ư�� ������ ��� �Լ�
    private void DropSpecialLoot()
    {
        // ���� ������ ������Ʈ ���� (��: Instantiation�� ���� ������ ���)
        Debug.Log("���彽���� ŷ�� óġ�Ͽ����ϴ�!");
        // ����: Instantiate(lootPrefab, transform.position, Quaternion.identity);
    }
}

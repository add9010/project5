using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Slime : Enemy
{
    protected Coroutine speedBoostCoroutine;

    protected virtual IEnumerator SpeedBoostRoutine()
    {
        while (!isEnemyDead)
        {
            // �̵� �ӵ� 2�� ����
            moveSpeed *= 2;
            Debug.Log("�̵� �ӵ� 2�� ����!");

            // 2�� ���� ����
            yield return new WaitForSeconds(1f);

            // �̵� �ӵ� ������� ����
            moveSpeed /= 2;
            Debug.Log("�̵� �ӵ� ����.");
            yield return new WaitForSeconds(5f); 
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (isEnemyDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerManager playerManager = collision.collider.GetComponentInParent<PlayerManager>();
            if (playerManager != null && !playerManager.IsDead)
            {
                playerManager.playerHealth.TakeDamage(atkDmg);
                Debug.Log($"{enemyName} ����: {atkDmg} ������");
            }
        }
    }

    protected override void SpawnMark()
    {
        base.SpawnMark();
        if (speedBoostCoroutine == null)
            speedBoostCoroutine = StartCoroutine(SpeedBoostRoutine());  // �̵� �ӵ� ���� �ڷ�ƾ ����
        
    }

}

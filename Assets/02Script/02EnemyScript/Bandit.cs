using System.Collections;
using UnityEngine;

public class Bandit : Enemy
{
    protected override void Start()
    {
        base.Start();
        SetEnemyStatus("산적", 90, 10, 3);
        attackCooldown = 3f;
        attackRange = 3f;
    }

    public override void PerformAttack()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= attackRange)
        {
            PlayerManager pm = player.GetComponent<PlayerManager>();
            if (pm != null && !pm.IsDead)
            {
                pm.playerHealth.TakeDamage(atkDmg);
                Debug.Log($"{enemyName} 근접 공격: {atkDmg} 데미지");
            }
        }
    }
}

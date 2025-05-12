using System.Collections;
using UnityEngine;

public class Thief : Enemy
{
    protected override void Start()
    {
        base.Start();
        SetEnemyStatus("좀도둑", 120, 10, 4);
        attackCooldown = 1f;
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

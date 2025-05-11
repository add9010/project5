using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill2 : MonoBehaviour
{
    private PlayerManager pm;

    public float chargeTime = 0.3f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public Vector2 hitBoxSize = new Vector2(1.5f, 1f);
    private HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>();

    public void Initialize(PlayerManager playerManager)
    {
        pm = playerManager;
    }

    public void Activate()
    {
        if (pm == null) return;

        pm.playerStateController.ForceSetSkill("Skill2");
        pm.playerStateController.LockSkillState(2f);
        pm.StartAttackCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        pm.isAction = true;
        pm.rb.linearVelocity = Vector2.zero;
        hitEnemies.Clear();

        yield return new WaitForSeconds(chargeTime); // 차징

        Vector2 dir = pm.spriteRenderer.flipX ? Vector2.left : Vector2.right;
        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            pm.rb.linearVelocity = dir * dashSpeed;

            Vector2 boxCenter = (Vector2)pm.transform.position + dir * 0.8f;
            Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, hitBoxSize, 0, LayerMask.GetMask("Enemy"));
            foreach (var col in hits)
            {
                if (hitEnemies.Contains(col)) continue;

                CombatManager.ApplyDamage(col.gameObject, pm.data.attackPower * 1.2f, 10f, pm.transform.position);
                hitEnemies.Add(col);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        pm.rb.linearVelocity = Vector2.zero;
        pm.isAction = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (pm == null) return;
        Vector2 dir = pm.spriteRenderer.flipX ? Vector2.left : Vector2.right;
        Vector2 boxCenter = (Vector2)transform.position + dir * 0.8f;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCenter, hitBoxSize);
    }
}

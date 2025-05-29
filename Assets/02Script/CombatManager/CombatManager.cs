using UnityEngine;

public static class CombatManager
{
    public static void ApplyDamage(GameObject target, float damage, float knockback, Vector2 sourcePos, float staggerDamage = 0)
    {
        if (target == null) return;

        IDamageable dmgTarget = target.GetComponent<IDamageable>() ?? target.GetComponentInParent<IDamageable>();
        dmgTarget?.TakeDamage(damage);

        IKnockbackable knockTarget = target.GetComponent<IKnockbackable>() ?? target.GetComponentInParent<IKnockbackable>();
        if (knockTarget != null)
        {
            Vector2 dir = ((Vector2)target.transform.position - sourcePos).normalized;
            knockTarget.ApplyKnockback(dir, knockback);
        }

        var enemy = target.GetComponent<Enemy>() ?? target.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            enemy.ReduceStagger(staggerDamage);
        }
    }
}

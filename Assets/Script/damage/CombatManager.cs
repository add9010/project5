using UnityEngine;

public static class CombatManager
{
    public static void ApplyDamage(IDamageable target, float damage, float knockback, Vector2 sourcePos)
    {
        if (target == null) return;

        Vector2 knockbackDir = ((Vector2)target.Position - sourcePos).normalized;

        DamageInfo info = new DamageInfo
        {
            damage = damage,
            knockback = knockbackDir * knockback
        };

        target.TakeDamage(info);
    }
}

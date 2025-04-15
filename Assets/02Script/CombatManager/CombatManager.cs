using UnityEngine;

public static class CombatManager
{
    public static void ApplyDamage(GameObject target, float damage, float knockback, Vector2 sourcePos)
    {
        if (target == null) return;

        // 데미지
        IDamageable dmgTarget = target.GetComponent<IDamageable>() ?? target.GetComponentInParent<IDamageable>();
        dmgTarget?.TakeDamage(damage);

        // 넉백
        IKnockbackable knockTarget = target.GetComponent<IKnockbackable>() ?? target.GetComponentInParent<IKnockbackable>();
        if (knockTarget != null)
        {
            Vector2 dir = ((Vector2)target.transform.position - sourcePos).normalized;
            knockTarget.ApplyKnockback(dir, knockback);
        }
    }
}

using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage);
}

public interface IKnockbackable
{
    void ApplyKnockback(Vector2 direction, float force);
}
public interface IStaggerable
{
    void ApplyStagger(float duration);
}

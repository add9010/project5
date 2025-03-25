using UnityEngine;

public interface IDamageable
{
    Vector2 Position { get; }
    void TakeDamage(DamageInfo info);
}
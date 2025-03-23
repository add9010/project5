using UnityEngine;

[System.Serializable]
public class PlayerData
{
    [Header("속성")]
    public float attackPower = 10.0f;
    public float speed = 4.0f;
    public float jumpForce = 7.5f;
    public float rollForce = 6.0f;
    public float attackKnockback = 8.0f;
    public float attackKnockbackThird = 800.0f;
    public float attackDuration = 0.2f;

    [Header("체력")]
    public float maxHealth = 100.0f;

    [Header("기타")]
    public Vector2 attackBoxSize = new Vector2(1.0f, 1.0f);
    public float heightOffset = 1.7f;
}
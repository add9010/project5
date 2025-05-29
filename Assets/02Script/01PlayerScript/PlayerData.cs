using UnityEngine;

[System.Serializable]
public class PlayerData
{
    [Header("속성")]
    public float attackPower = 10.0f;
    public float speed = 4.0f;
    public float jumpForce = 7.5f;
    public float dashForce = 10.0f;
    public float dashDuration = 0.2f;
    public float attackKnockback = 8.0f;
    public float attackKnockbackThird = 800.0f;
    public float attackDuration = 0.4f;

    [Header("체력")]
    public float maxHealth = 100.0f;

    [Header("마나")]
    public int maxMana = 3;       // 기본 최대 마나
    public float manaRechargeTime = 20f;  // 마나 1 충전에 걸리는 시간

    [Header("기타")]
    public Vector2 attackBoxSize = new Vector2(1.5f, 1.5f);
    public Vector2 attackBoxOffset = new Vector2(1.0f, 0f);
    public float heightOffset = 1.7f;

    [Header("카메라")]
    public float cameraFollowSpeed = 5f;
    public float cameraStopDistance = 2f;

    [Header("공격 전진 관련")]
    public float attackForwardSpeed = 4f;
    public float attackForwardDuration = 0.3f;


}

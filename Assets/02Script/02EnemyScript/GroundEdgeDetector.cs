using UnityEngine;
using System;

[RequireComponent(typeof(Collider2D))]
public class GroundEdgeDetector : MonoBehaviour
{
    [Header("Edge Check")]
    public LayerMask groundLayer;
    public float offsetX = 0.5f;  // 몸폭 절반 + 여유
    public float offsetY = 0f;    // 콜라이더 피벗 높이 차 보정
    public float rayDistance = 0.5f;  // 레이 길이

    public event Action OnEdgeDetected;        // 플랫폼 끝 감지 시 호출

    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        if (!IsGroundAhead)
            OnEdgeDetected?.Invoke();
    }

    /// <summary>
    /// 발앞에 땅이 있으면 true
    /// </summary>
    public bool IsGroundAhead
    {
        get
        {
            float dir = Mathf.Sign(transform.localScale.x);
            float footHeight = col.bounds.extents.y;
            Vector2 origin = (Vector2)transform.position
                             + Vector2.down * (footHeight + offsetY)
                             + Vector2.right * (offsetX * dir);

            Debug.DrawRay(origin, Vector2.down * rayDistance, Color.red);
            return Physics2D.Raycast(origin, Vector2.down, rayDistance, groundLayer);
        }
    }
}

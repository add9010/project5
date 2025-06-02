using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class JumpPad : MonoBehaviour
{
    public Sprite idleSprite;
    public Sprite[] activeSprites;
    public float animationFrameRate = 12f;
    public float jumpVelocity = 10f;

    private SpriteRenderer spriteRenderer;
    private bool isAnimating = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        var bc = GetComponent<BoxCollider2D>();
        if (bc != null)
        {
            // Platform Effector 사용할 때는 Is Trigger 해제
            if (bc.isTrigger)
                bc.isTrigger = false;
            // Used by Effector는 켜둔 상태여야 한다.
            bc.usedByEffector = true;
        }
    }

    void Start()
    {
        if (idleSprite != null)
            spriteRenderer.sprite = idleSprite;
    }

    // Trigger가 아닌 “Collision”으로 감지
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 이미 애니메이션 재생 중이면 무시
        if (isAnimating) return;

        if (collision.collider.CompareTag("Player"))
        {
            // 플레이어 Rigidbody2D에 위쪽 속도 부여
            Rigidbody2D playerRb = collision.collider.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, jumpVelocity);
            }

            // Active 애니메이션 재생
            if (activeSprites != null && activeSprites.Length > 0)
            {
                StartCoroutine(PlayActiveAnimation());
            }
        }
    }

    private IEnumerator PlayActiveAnimation()
    {
        isAnimating = true;
        float delay = 1f / Mathf.Max(animationFrameRate, 1f);

        for (int i = 0; i < activeSprites.Length; i++)
        {
            if (activeSprites[i] != null)
                spriteRenderer.sprite = activeSprites[i];
            yield return new WaitForSeconds(delay);
        }

        if (idleSprite != null)
            spriteRenderer.sprite = idleSprite;
        isAnimating = false;
    }
}

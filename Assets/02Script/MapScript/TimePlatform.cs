using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

[RequireComponent(typeof(TilemapRenderer))]
[RequireComponent(typeof(TilemapCollider2D))]
public class TimedTilemapPlatform : MonoBehaviour
{
    [Header("밟힌 후 비활성화까지 대기 시간(초)")]
    public float disableDelay = 2f;
    [Header("비활성화 후 재활성화 대기 시간(초)")]
    public float reenableDelay = 5f;

    private TilemapRenderer _tmRenderer;
    private TilemapCollider2D _tmCollider;
    private CompositeCollider2D _composite;  // CompositeCollider2D 사용 중이면

    private bool _triggered = false;

    void Awake()
    {
        _tmRenderer = GetComponent<TilemapRenderer>();
        _tmCollider = GetComponent<TilemapCollider2D>();
        _composite = GetComponent<CompositeCollider2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 이미 타이머가 돌고 있으면 무시
        if (_triggered) return;
        if (!collision.collider.CompareTag("Player")) return;

        _triggered = true;
        StartCoroutine(DisableAndReenable());
    }

    private IEnumerator DisableAndReenable()
    {
        // 1) 밟힌 뒤 대기
        yield return new WaitForSeconds(disableDelay);

        // 2) Tilemap 비활성화
        _tmRenderer.enabled = false;
        _tmCollider.enabled = false;
        if (_composite != null) _composite.enabled = false;

        // 3) 재활성화 대기
        yield return new WaitForSeconds(reenableDelay);

        // 4) 다시 활성화
        _tmRenderer.enabled = true;
        _tmCollider.enabled = true;
        if (_composite != null) _composite.enabled = true;

        _triggered = false;
    }
}

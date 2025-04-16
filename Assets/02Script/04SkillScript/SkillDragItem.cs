using UnityEngine;
using UnityEngine.EventSystems;

public class SkillDragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public SkillData skillData;

    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Vector2 originalAnchoredPosition; // 원래 위치 저장
    private Vector3 originalScale;            // 원래 스케일 저장
    public bool wasDroppedOnSlot = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;

        // 드래그 시작 시 원래 위치와 스케일 저장
        RectTransform rt = GetComponent<RectTransform>();
        if (rt != null)
        {
            originalAnchoredPosition = rt.anchoredPosition;
            originalScale = rt.localScale;
        }

        // 부모를 루트로 변경 (worldPosition 유지하지 않음)
        transform.SetParent(transform.root, false);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 슬롯에 드롭되지 않은 경우
        if (!wasDroppedOnSlot)
        {
            // 부모 복원
            if (originalParent != null)
            {
                transform.SetParent(originalParent, false);

                // 위치와 스케일 복원
                RectTransform rt = GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.anchoredPosition = originalAnchoredPosition; // 원래 위치로 복원
                    rt.localScale = originalScale;                 // 원래 스케일로 복원
                }
            }
            else
            {
                Debug.LogWarning("Original parent is null. Cannot reset position.");
            }
        }

        // 드래그 가능 상태 복원
        canvasGroup.blocksRaycasts = true;

        // 드롭 상태 초기화
        wasDroppedOnSlot = false;
    }
}

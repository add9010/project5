using UnityEngine;
using UnityEngine.EventSystems;

public class SkillDragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public SkillData skillData;

    private CanvasGroup canvasGroup;
    private Transform originalParent;
    public bool wasDroppedOnSlot = false;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root, true); // worldPosition 유지
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!wasDroppedOnSlot)
        {
            transform.SetParent(originalParent, false);
            transform.localPosition = Vector3.zero;
        }

        canvasGroup.blocksRaycasts = true;
        wasDroppedOnSlot = false; // 다음 드래그를 위해 리셋
    }
}

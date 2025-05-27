using UnityEngine;
using UnityEngine.EventSystems;

public class SkillDragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public SkillData skillData;
    public bool isLocked = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        // 복제본 생성
        GameObject dragCopy = Instantiate(gameObject, transform.root);
        dragCopy.name = skillData.skillName + "_DragCopy";

        var copyGroup = dragCopy.GetComponent<CanvasGroup>();
        if (copyGroup == null)
            copyGroup = dragCopy.AddComponent<CanvasGroup>();

        copyGroup.blocksRaycasts = false;

        var copyDrag = dragCopy.GetComponent<SkillDragItem>();
        copyDrag.isLocked = true;

        eventData.pointerDrag = dragCopy;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(gameObject); // 복제본은 항상 파괴
    }
}

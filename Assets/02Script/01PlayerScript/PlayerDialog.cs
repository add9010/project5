using UnityEngine;

public class PlayerDialog
{
    private PlayerManager manager;
    public DialogManager dialog;// 대화창 매니저
    private Vector3 dirVec = Vector3.zero;// 방향 벡터
    GameObject scanObject;
    
    public PlayerDialog(PlayerManager manager)
    {
        this.manager = manager;
    }

    public void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            dirVec = Vector3.left;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            dirVec = Vector3.right;

        if (Input.GetKeyDown(KeyCode.F) && scanObject != null)
        {
            DialogTrigger dt = scanObject.GetComponent<DialogTrigger>();
            if (dt != null)
            {
                dt.TryStartDialogue();
            }
        }
    }



    public void HandleScan()
    {
        // raycast를 통한 오브젝트 스캔
        Debug.DrawRay(manager.rb.position, dirVec * 0.7f, Color.green);

        RaycastHit2D rayHit = Physics2D.Raycast(manager.rb.position, dirVec, 0.7f,
            LayerMask.GetMask("Object"));

        if (rayHit.collider != null)
        {
            scanObject = rayHit.collider.gameObject;
        }
        else
        {
            scanObject = null;
        }
    }

}

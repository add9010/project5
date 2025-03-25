using UnityEngine;

public class PlayerDialog
{
    private PlayerManager manager;
    public DialogManager dialog;// ��ȭâ �Ŵ���
    private Vector3 dirVec = Vector3.zero;// ���� ����
    GameObject scanObject;
    
    public PlayerDialog(PlayerManager manager)
    {
        this.manager = manager;
    }

    public void HandleInput()
    {
        if (manager.isAction) return;

        if (Input.GetKeyDown(KeyCode.A))
            dirVec = Vector3.left;
        else if (Input.GetKeyDown(KeyCode.D))
            dirVec = Vector3.right;

        if (Input.GetKeyDown(KeyCode.LeftControl) && scanObject != null)
        {
            manager.dialog.Action(scanObject);
        }
    }


    public void HandleScan()
    {
        // raycast�� ���� ������Ʈ ��ĵ
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

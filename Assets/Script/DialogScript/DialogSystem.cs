using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Net.Http.Headers;

public class DialogSystem : MonoBehaviour
{
    [SerializeField]
    private Speaker[] speakers;//��ȭ�� �����ϴ� ĳ���͵��� ui�迭
    [SerializeField]
    private DialogData[] dialogs;//���� �б��� ��� ��� �迭
    [SerializeField]
    private bool isAutoStart = true;//�ڵ� ���� ����
    private bool isFirst = true;//���� 1ȸ�� ȣ���ϱ� ���� ����
    private int currentDialogIndex = -1;//���� ��� ����
    private int currentSpeakerIndex = 0;//���� �����ϴ� ȭ���� speakers �迭 ����\

    private void Awake()
    {
        Setup();
    }

    private void Setup()
    {
        for (int i = 0; i < speakers.Length; i++)
        {
            SetActiveObjects(speakers[i], false);
            speakers[i].spriteRenderer.gameObject.SetActive(true);
        }
    }

    public bool UpdateDialog()
    {
        //��� �б� 1ȸ�� ȣ��
        if (isFirst == true)
        {
            //�ʱ�ȭ ĳ���� �̹��� Ȱ��ȭ ���� UI ��Ȱ��ȭ
            Setup();
            //�ڵ�������� �����Ǿ� ������ ù��° ��� ȣ��
            if (isAutoStart) SetNextDialog();

            isFirst = false;
        }

        if(Input.GetMouseButtonDown(0))
        {
            //��簡 ������ ���� ��� ȣ��
            if (dialogs.Length>currentDialogIndex+1)
            {
                SetNextDialog();
            }
            //��簡 ������ ��ȭ ����
            else
            {
                for(int i = 0; i < speakers.Length; i++)
                {
                    SetActiveObjects(speakers[i], false);
                    speakers[i].spriteRenderer.gameObject.SetActive(false);
                }
                //��ȭ ����
                return true;
            }
        }
        return false;
    }
    
    private void SetNextDialog()
    {
        SetActiveObjects(speakers[currentSpeakerIndex], false);
        currentDialogIndex++;
        currentSpeakerIndex = dialogs[currentDialogIndex].speakerIndex;
        SetActiveObjects(speakers[currentSpeakerIndex], true);
        speakers[currentSpeakerIndex].textName.text = dialogs[currentDialogIndex].name;
        speakers[currentSpeakerIndex].textDialog.text = dialogs[currentDialogIndex].dialogue;
    }

    private void SetActiveObjects(Speaker speaker, bool visible)
    {
        speaker.imageDialog.gameObject.SetActive(visible);
        speaker.textName.gameObject.SetActive(visible);
        speaker.textDialog.gameObject.SetActive(visible);

        speaker.objectArrow.SetActive(false);

        Color color = speaker.spriteRenderer.color;
        color.a = visible == true ? 1 : 0.2f;
        speaker.spriteRenderer.color = color;
    }
}

[System.Serializable]
public class Speaker
{
    public SpriteRenderer spriteRenderer;   //ĳ���� �̹���
    public Image imageDialog;               //��ȭâ �̹���
    public TextMeshProUGUI textName;        //���� ������� ĳ���� �̸� ��� �ؽ�Ʈ
    public TextMeshProUGUI textDialog;      //���� ��� ��� �ؽ�Ʈ
    public GameObject objectArrow;          //��簡 �Ϸ�� ��µǴ� Ŀ�� ������Ʈ
}

[System.Serializable]
public class DialogData
{
    public int speakerIndex;//ȭ���� speakers �迭 ����
    public string name;//ȭ�� �̸�
    [TextArea(3, 5)]
    public string dialogue;//���
}

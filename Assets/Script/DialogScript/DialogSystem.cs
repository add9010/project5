using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Net.Http.Headers;

public class DialogSystem : MonoBehaviour
{
    [SerializeField]
    private Speaker[] speakers;//대화에 참여하는 캐릭터들의 ui배열
    [SerializeField]
    private DialogData[] dialogs;//현재 분기의 대사 목록 배열
    [SerializeField]
    private bool isAutoStart = true;//자동 시작 여부
    private bool isFirst = true;//최초 1회만 호출하기 위한 변수
    private int currentDialogIndex = -1;//현재 대사 순번
    private int currentSpeakerIndex = 0;//현재 말을하는 화자의 speakers 배열 순번\

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
        //대사 분기 1회만 호출
        if (isFirst == true)
        {
            //초기화 캐릭터 이미지 활성화 관련 UI 비활성화
            Setup();
            //자동재생으로 설정되어 있으면 첫번째 대사 호출
            if (isAutoStart) SetNextDialog();

            isFirst = false;
        }

        if(Input.GetMouseButtonDown(0))
        {
            //대사가 남으면 다음 대사 호출
            if (dialogs.Length>currentDialogIndex+1)
            {
                SetNextDialog();
            }
            //대사가 끝나면 대화 종료
            else
            {
                for(int i = 0; i < speakers.Length; i++)
                {
                    SetActiveObjects(speakers[i], false);
                    speakers[i].spriteRenderer.gameObject.SetActive(false);
                }
                //대화 종료
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
    public SpriteRenderer spriteRenderer;   //캐릭터 이미지
    public Image imageDialog;               //대화창 이미지
    public TextMeshProUGUI textName;        //현재 대사중인 캐릭터 이름 출력 텍스트
    public TextMeshProUGUI textDialog;      //현재 대사 출력 텍스트
    public GameObject objectArrow;          //대사가 완료시 출력되는 커서 오브젝트
}

[System.Serializable]
public class DialogData
{
    public int speakerIndex;//화자의 speakers 배열 순번
    public string name;//화자 이름
    [TextArea(3, 5)]
    public string dialogue;//대사
}

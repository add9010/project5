using UnityEngine;
using UnityEngine.SceneManagement;

public class EscMenuController : MonoBehaviour
{
    public GameObject escPanel;     // ESC 메뉴 전체
    public GameObject noticePanel;  // 경고창 (나가기 확인)

    private bool isMenuOpen = false;

    void Start()
    {
        if (escPanel == null)
            escPanel = GameObject.Find("ESC");

        if (noticePanel == null)
            noticePanel = GameObject.Find("Notice");
        escPanel.SetActive(false);
        noticePanel.SetActive(false);
    }

    void Update()
    {
        escPanel.transform.SetAsLastSibling();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (noticePanel.activeSelf)
            {
                noticePanel.SetActive(false); // ESC 누르면 우선 Notice만 닫기
                return;
            }

            isMenuOpen = !isMenuOpen;
            escPanel.SetActive(isMenuOpen);

            Time.timeScale = isMenuOpen ? 0f : 1f; // 일시정지
        }
    }

    // 버튼 함수들
    public void OnClickOption()
    {
        Debug.Log("옵션 눌림 - 나중에 구현");
    }

    public void OnClickTitle()
    {
        noticePanel.SetActive(true);
        pendingAction = ReturnToTitle;
    }

    public void OnClickExit()
    {
        noticePanel.SetActive(true);
        pendingAction = QuitGame;
    }

    public void OnClickCancel()
    {
        noticePanel.SetActive(false);
    }

    public void OnClickConfirm()
    {
        noticePanel.SetActive(false);
        pendingAction?.Invoke();
    }

    // ---------------- 내부 처리용 ----------------

    private System.Action pendingAction;

    private void ReturnToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene");
    }

    private void QuitGame()
    {
        Debug.Log("게임 종료");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class EscMenuController : MonoBehaviour
{
    [Header("인게임 ESC 메뉴")]
    public GameObject escPanel;
    public GameObject noticePanel;

    [Header("타이틀 씬 옵션 패널 (런타임 할당)")]
    private GameObject optionPanel;

    private bool isMenuOpen = false;
    private void Awake()
    {
        // 씬 로드 시마다 Option Panel을 찾아 연결
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void Start()
    {
        if (escPanel == null)
            escPanel = GameObject.Find("ESC");

        if (noticePanel == null)
            noticePanel = GameObject.Find("Notice");
        escPanel.SetActive(false);
        noticePanel.SetActive(false);
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TitleScene")
        {
            // 1) 씬 내에 있는 "Main Menu Canvas"를 찾아서
            var menuCanvas = GameObject.Find("Main Menu Canvas");
            if (menuCanvas != null)
            {
                // 2) 그 자식 중 "Option" 오브젝트를 찾아서
                var optTf = menuCanvas.transform.Find("Option");
                optionPanel = optTf != null ? optTf.gameObject : null;
            }
            else
            {
                Debug.LogWarning("EscMenuController: Main Menu Canvas를 찾을 수 없습니다!");
                optionPanel = null;
            }
        }
        else
        {
            optionPanel = null;
        }

        // in-game ESC 메뉴는 항상 닫아 두기
        if (escPanel != null) escPanel.SetActive(false);
        if (noticePanel != null) noticePanel.SetActive(false);
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
        // in-game 메뉴 닫기
        isMenuOpen = false;
        escPanel.SetActive(false);
        Time.timeScale = 1f;

        // 이제 언제나 퍼시스트된 OptionMenu를 열고 닫습니다.
        if (OptionMenu.Instance != null)
        {
            if (OptionMenu.Instance.gameObject.activeSelf)
                OptionMenu.Instance.CloseOption();
            else
                OptionMenu.Instance.OpenOption();
        }
        else
        {
            Debug.LogError("EscMenuController: OptionMenu.Instance가 없습니다!");
        }
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

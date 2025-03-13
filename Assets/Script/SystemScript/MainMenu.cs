using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour//,IPointerEnterHandler,IPointerExitHandler
{
    public FadeManager fadeManager; // 페이드 매니저 연결


    public void OnClickNewGame()
    {
        Debug.Log("새 게임 시작");

        // 페이드 아웃 시작
        if (fadeManager != null)
        {
            fadeManager.RegisterCallback(() => SceneManager.LoadScene("Loding")); // 페이드 아웃 후 씬 이동
            fadeManager.FadeOut();
        }
        else
        {
            SceneManager.LoadScene("Loding"); // 페이드 매니저가 없으면 그냥 씬 이동
        }
    }

    public void OnClickLoad()
    {
        Debug.Log("불러오기");
    }

    public void OnClickOption()
    {
        Debug.Log("옵션");
    }

    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}


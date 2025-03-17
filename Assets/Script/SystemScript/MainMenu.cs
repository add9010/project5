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

        // 게임 데이터 불러오기
        DateManager.Instance.LoadGameData();

        // 불러온 데이터가 적용되었는지 확인
        for (int i = 0; i < DateManager.Instance.data.isUnlock.Length; i++)
        {
            Debug.Log($"{i}번 챕터 잠금 해제 여부 : " + DateManager.Instance.data.isUnlock[i]);
        }

        // 씬 전환 (예: 게임 씬으로 이동)
        if (fadeManager != null)
        {
            fadeManager.RegisterCallback(() => SceneManager.LoadScene("Floor1")); // 불러온 후 게임씬 이동
            fadeManager.FadeOut();
        }
        else
        {
            SceneManager.LoadScene("Floor1");
        }
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


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

        GameManager.Instance.gameData = new Data(); // 새 데이터 생성
        GameManager.Instance.SaveGame();

        GameManager.Instance.nextSceneName = "VillageStage"; // ✅ 목적지 설정

        if (fadeManager != null)
        {
            fadeManager.RegisterCallback(() => SceneManager.LoadScene("Loding"));
            fadeManager.FadeOut();
        }
        else
        {
            SceneManager.LoadScene("Loding");
        }
    }


    public void OnClickLoad()
    {
        Debug.Log("불러오기");

        Data loadedData = DataManager.Instance.LoadData();
        string savedScene = loadedData.savedSceneName;

        GameManager.Instance.gameData = loadedData;
        GameManager.Instance.nextSceneName = savedScene;


        if (fadeManager != null)
        {
            fadeManager.RegisterCallback(() => SceneManager.LoadScene("Loding"));
            fadeManager.FadeOut();
        }
        else
        {
            SceneManager.LoadScene("Loding");
        }
    }

    public void OnClickBossDirect()
    {
        Debug.Log("보스 바로 가기");

        GameManager.Instance.nextSceneName = "Boss1"; // 저장 ❌

        if (fadeManager != null)
        {
            fadeManager.RegisterCallback(() => SceneManager.LoadScene("Loding"));
            fadeManager.FadeOut();
        }
        else
        {
            SceneManager.LoadScene("Loding");
        }
    }


    // 🔹 씬이 로드된 후 실행
    private void OnGameSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ✅ nextSceneName과 로드된 씬이 같을 때만 실행
        if (GameManager.Instance != null &&
            scene.name == GameManager.Instance.nextSceneName)
        {
            SceneManager.sceneLoaded -= OnGameSceneLoaded;
            Invoke(nameof(DelayedApplyGameState), 1.0f);
        }
    }


    // 🔹 1초 후 실행될 함수
    private void DelayedApplyGameState()
    {
        GameManager.Instance.ApplyGameState();
    }
    public void OnClickOption()
    {
        Debug.Log("옵션");
        if (OptionMenu.Instance != null)
        {
            OptionMenu.Instance.OpenOption();
        }
        else
        {
            Debug.LogError("OptionMenu.Instance가 없습니다!");
        }
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


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

        // 새로운 게임 데이터 생성 및 저장
        GameManager.Instance.gameData = new Data(); // 새 게임 데이터 초기화
        GameManager.Instance.SaveGame(); // 초기 데이터 저장

        // 씬 이동 (예: 로딩 씬 → 게임 씬)
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

        // 🔹 저장된 데이터 불러오기
        GameManager.Instance.LoadGame();

        // 🔹 씬 이동 후 일정 시간 뒤 ApplyGameState() 실행
        SceneManager.sceneLoaded += OnGameSceneLoaded;

        if (fadeManager != null)
        {
            fadeManager.RegisterCallback(() => SceneManager.LoadScene("Floor1")); // ✅ 타이틀 씬에서 게임 씬으로 이동
            fadeManager.FadeOut();
        }
        else
        {
            SceneManager.LoadScene("GameScene");
        }
    }

    // 🔹 씬이 로드된 후 실행
    private void OnGameSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Floor1") // GameScene이 로드될 때만 실행
        {
            SceneManager.sceneLoaded -= OnGameSceneLoaded; // 중복 실행 방지
            Invoke(nameof(DelayedApplyGameState), 1.0f); //  1초 후 실행 (플레이어가 로드될 시간 확보)
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


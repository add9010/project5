using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Data gameData = new Data();
    public string nextSceneName = "VillageStage"; // 기본값

    private void Awake()
    {
        if (Instance == null)
        {
            transform.SetParent(null);
            Instance = this;
        }
        else
        {
            Destroy(gameObject); 
        }
    }
    private void Start()
    {
        // “Loding” 씬이 로드될 때 실행될 콜백 등록
        SceneManager.sceneLoaded += OnAnySceneLoaded;
    }
    private void OnDestroy()
    {
        // 혹시 GameManager가 파괴될 때 콜백 해제
        SceneManager.sceneLoaded -= OnAnySceneLoaded;
    }
    private void OnAnySceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1) 만약 현재 로드된 씬이 “Loding” 씬이라면, 다음에 지정해 둔 nextSceneName 씬으로 바로 이동
        if (scene.name == "Loding")
        {
            // Loding 씬이 완전히 열린 직후 → 즉시 nextSceneName 씬을 로드
            // FadeManager나 로딩 화면이 끝났을 때 호출하고 싶다면, 
            // Loding 씬 안에 간단한 스크립트(예: LodingController)가 있어서 
            // “Loding 애니메이션이 끝나면 GameManager.Instance.LoadTargetScene() 호출” 형태로 만들어도 됩니다.
            SceneManager.LoadScene(nextSceneName);
            return;
        }

        // 2) 만약 현재 로드된 씬이 “nextSceneName”이라면, 그 씬에서 플레이어 위치를 복원
        if (scene.name == nextSceneName)
        {
            // 복원(지연을 주고 싶다면 Invoke로도 지연 가능)
            ApplyGameState();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            TryResetPlayerState();
            ReturnToTitle();
        }
    }

    public void SaveGame()
    {
        SavePlayerPosition();
        DataManager.Instance.SaveData(gameData);
    }

    private void SavePlayerPosition()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            gameData.playerX = player.transform.position.x;
            gameData.playerY = player.transform.position.y;
            gameData.playerZ = player.transform.position.z;

            gameData.savedSceneName = SceneManager.GetActiveScene().name; // ✅ 현재 씬 이름 저장
        }
    }

    public void LoadGame()
    {
        gameData = DataManager.Instance.LoadData();
        nextSceneName = gameData.savedSceneName; // ✅ 저장된 씬으로 이동하도록 설정
        ApplyGameState();

        if (SceneManager.GetActiveScene().name != nextSceneName)
        {
            SceneManager.sceneLoaded += OnSceneLoadedForLoad;
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            ApplyGameState();
        }
    }
    private void OnSceneLoadedForLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == nextSceneName)
        {
            SceneManager.sceneLoaded -= OnSceneLoadedForLoad;
            Invoke(nameof(DelayedApplyGameState), 1.0f);
        }
    }

    public void ApplyGameState()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene != gameData.savedSceneName)
        {
            //Debug.LogWarning("현재 씬이 저장된 씬과 다릅니다. 복원 대기 중...");
            return;
        }

        Invoke(nameof(DelayedApplyGameState), 0.5f);
    }

    private void DelayedApplyGameState()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            player.transform.position = new Vector3(
                gameData.playerX,
                gameData.playerY,
                gameData.playerZ
            );
        }
        else
        {
            Debug.LogWarning("플레이어를 찾을 수 없습니다. 복원 재시도...");
            Invoke(nameof(DelayedApplyGameState), 0.5f);
        }
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void LoadSceneWithFade(string sceneName)
    {
        FadeManager fade = FindAnyObjectByType<FadeManager>();
        if (fade != null)
        {
            fade.RegisterCallback(() => SceneManager.LoadScene(sceneName));
            fade.FadeOut();
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
    private void TryResetPlayerState()
    {
        var player = GameObject.FindWithTag("Player");
        if (player == null) return;

        var pm = player.GetComponent<PlayerManager>();
        if (pm != null)
            pm.ResetAllPlayerState();
    }
}

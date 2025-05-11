using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Data gameData = new Data();
    public string nextSceneName = "Stage1"; // 기본값

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ✅ 씬 이동 시 파괴되지 않도록 함
        }
        else
        {
            Destroy(gameObject); // ✅ 중복 GameManager 방지
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReturnToTitle();
        }
    }

    public void SaveGame()
    {
        SavePlayerPosition();
        DateManager.Instance.SaveData(gameData);
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
        gameData = DateManager.Instance.LoadData();
        nextSceneName = gameData.savedSceneName; // ✅ 저장된 씬으로 이동하도록 설정
        ApplyGameState();
    }

    public void ApplyGameState()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene != gameData.savedSceneName)
        {
            Debug.LogWarning("현재 씬이 저장된 씬과 다릅니다. 복원 대기 중...");
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
}

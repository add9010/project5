using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Data gameData = new Data();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReturnToTitle();
        }
    }

    // 데이터 저장하기
    public void SaveGame()
    {
        SavePlayerPosition(); // 🔹 현재 플레이어 위치 저장
        DateManager.Instance.SaveData(gameData);
        Debug.Log($"GameManager: 데이터 저장 완료! 위치: {gameData.playerX}, {gameData.playerY}, {gameData.playerZ}");
    }

    // 🔹 현재 플레이어 위치를 저장하는 함수
    private void SavePlayerPosition()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            gameData.playerX = player.transform.position.x;
            gameData.playerY = player.transform.position.y;
            gameData.playerZ = player.transform.position.z;
            Debug.Log($"플레이어 위치 저장 완료: {gameData.playerX}, {gameData.playerY}, {gameData.playerZ}");
        }
        else
        {
            Debug.LogWarning("플레이어를 찾을 수 없습니다! 위치를 저장할 수 없습니다.");
        }
    }
    // 불러온 데이터를 게임에 적용하는 함수
    public void LoadGame()
    {
        gameData = DateManager.Instance.LoadData();
        Debug.Log($"GameManager: 데이터 불러오기 완료! 위치: {gameData.playerX}, {gameData.playerY}, {gameData.playerZ}");

        ApplyGameState(); // 🔹 불러온 데이터 기반으로 게임 상태 적용
    }

    // 🔹 불러온 데이터를 게임에 적용하는 함수 (플레이어 위치 복원)
    public void ApplyGameState()
    {
        // ✅ 현재 씬이 "GameScene"이 아니면 실행하지 않음
        if (SceneManager.GetActiveScene().name != "GameScene")
        {
            Debug.LogWarning("🚨 현재 씬이 GameScene이 아닙니다! 플레이어 위치 복원을 건너뜁니다.");
            return;
        }

        Invoke(nameof(DelayedApplyGameState), 0.5f); // 🚀 0.5초 뒤 실행 (플레이어 로딩 시간 확보)
    }

    private void DelayedApplyGameState()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            player.transform.position = new Vector3(gameData.playerX, gameData.playerY, gameData.playerZ);
            Debug.Log($"플레이어 위치 복원 완료: {gameData.playerX}, {gameData.playerY}, {gameData.playerZ}");
        }
        else
        {
            Debug.LogWarning(" 플레이어를 찾을 수 없습니다. 다시 시도합니다...");
            Invoke(nameof(DelayedApplyGameState), 0.5f); // 🚀 0.5초 후 다시 시도
        }
    }
    public void ReturnToTitle()
    {
        Debug.Log("타이틀 화면으로 이동");
        SceneManager.LoadScene("TitleScene"); // "TitleScene"은 실제 타이틀 씬 이름으로 변경해야 함
    }


}
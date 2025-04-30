using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    public GameObject[] maps;               // 메인 맵 리스트
    public Transform[] startPoints;          // 메인 맵 스타트 포인트 리스트

    public GameObject[] secretMaps;          // 비밀방 맵 리스트
    public Transform[] secretStartPoints;    // 비밀방 스타트 포인트 리스트
 
    private int currentMapIndex = 0;
    private bool isInSecretRoom = false;     // 지금 SecretRoom 안에 있는지 체크

    private int riverClearCount = 0;
    public GameObject golemPathEndPoint; // 골렘으로 가는 EndPoint 연결

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

    private void Start()
    {
        for (int i = 0; i < maps.Length; i++)
            maps[i].SetActive(i == currentMapIndex);

        for (int i = 0; i < secretMaps.Length; i++)
            secretMaps[i].SetActive(false);

        MovePlayerToStart();
    }

    public void GoToNextMap()
    {
        if (isInSecretRoom)
        {
            // Secret방 클리어 후 → 다음 메인맵 이동
            foreach (var sMap in secretMaps)
                sMap.SetActive(false);

            isInSecretRoom = false; // 비밀방 빠져나왔으니 false로
        }
        else
        {
            maps[currentMapIndex].SetActive(false);
            currentMapIndex++;
        }

        if (currentMapIndex < maps.Length)
        {
            maps[currentMapIndex].SetActive(true);
            MovePlayerToStart();
        }
        else
        {
            Debug.Log("모든 맵 완료!");
            // TODO: 보스전 or 엔딩 처리
        }
    }

    public void GoToSecretMap(int secretIndex)
    {
        foreach (var map in maps) map.SetActive(false);
        foreach (var sMap in secretMaps) sMap.SetActive(false);

        if (secretIndex >= 0 && secretIndex < secretMaps.Length)
        {
            secretMaps[secretIndex].SetActive(true);
            isInSecretRoom = true; // Secret방 안으로 진입했음
            MovePlayerToSecretStart(secretIndex);
        }
        else
        {
            Debug.LogWarning("비밀맵 인덱스 오류!");
        }
    }

    private void MovePlayerToStart()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && startPoints.Length > currentMapIndex)
        {
            player.transform.position = startPoints[currentMapIndex].position;
        }
    }

    private void MovePlayerToSecretStart(int secretIndex)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && secretStartPoints.Length > secretIndex)
        {
            player.transform.position = secretStartPoints[secretIndex].position;
        }
    }
  
    public void ClearRiverStage()
    {
        riverClearCount++;

        if (riverClearCount >= 4) // RiverStage_1 ~ RiverStage_4 모두 완료했을 때
        {
            if (golemPathEndPoint != null)
            {
                golemPathEndPoint.SetActive(true); // 골렘 방향 EndPoint 열기
                Debug.Log("골렘 스테이지 진입 경로 오픈!");
            }
        }
    }
    public string GetCurrentMapName()
    {
        if (currentMapIndex >= 0 && currentMapIndex < maps.Length)
        {
            return maps[currentMapIndex].name;
        }
        return "";
    }

}

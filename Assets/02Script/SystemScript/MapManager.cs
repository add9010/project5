using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    public GameObject[] maps;               // 메인 맵 리스트
    public Transform[] startPoints;          // 메인 맵 스타트 포인트 리스트

    public GameObject[] secretMaps;          // 비밀방 맵 리스트
    public Transform[] secretStartPoints;    // 비밀방 스타트 포인트 리스트

    public GameObject[] ShopMasp;           // 상점 맵 리스트
    public Transform[] ShopStartPoints;      // 상점 스타트 포인트 리스트

    private int currentMapIndex = 0;
    private bool isInSecretRoom = false;     // 지금 SecretRoom 안에 있는지 체크
    private bool isInShop = false; // 상점 안에 있는지 여부

    private int riverClearCount = 0;
    public GameObject golemPathEndPoint; // 골렘으로 가는 EndPoint 연결

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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
            foreach (var sMap in secretMaps)
                sMap.SetActive(false);

            isInSecretRoom = false;
        }
        else if (isInShop)
        {
            foreach (var shopMap in ShopMasp)
                shopMap.SetActive(false);

            isInShop = false;
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
    public void GoToShopMap(int shopIndex)
    {
        foreach (var map in maps) map.SetActive(false);
        foreach (var sMap in secretMaps) sMap.SetActive(false);
        foreach (var shopMap in ShopMasp) shopMap.SetActive(false); // 상점 맵들 비활성화

        if (shopIndex >= 0 && shopIndex < ShopMasp.Length)
        {
            ShopMasp[shopIndex].SetActive(true);
            isInShop = true; // 상점으로 진입했음
            MovePlayerToShopStart(shopIndex);
        }
        else
        {
            Debug.LogWarning("상점 인덱스 오류!");
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
    private void MovePlayerToShopStart(int shopIndex)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && ShopStartPoints.Length > shopIndex)
        {
            player.transform.position = ShopStartPoints[shopIndex].position;
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
    public void ForceMoveToMap(int mapIndex)
    {
        if (mapIndex >= 0 && mapIndex < maps.Length)
        {
            maps[currentMapIndex].SetActive(false);
            currentMapIndex = mapIndex;
            maps[currentMapIndex].SetActive(true);
            MovePlayerToStart();
        }
    }

}

using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    public GameObject[] maps;               // 메인 맵 리스트
    public Transform[] startPoints;          // 메인 맵 스타트 포인트 리스트

    public GameObject[] secretMaps;          // 비밀방 맵 리스트
    public Transform[] secretStartPoints;    // 비밀방 스타트 포인트 리스트

    public GameObject[] ShopMaps;           // 상점 맵 리스트
    public Transform[] ShopStartPoints;      // 상점 스타트 포인트 리스트

    private int currentMapIndex = 0;
    private bool isInSecretRoom = false;     // 지금 SecretRoom 안에 있는지 체크
    private bool isInShop = false; // 상점 안에 있는지 여부

    private int previousMapIndex;           // 상점 입장 전, 활성화되어 있던 메인 맵 인덱스
    private Vector3 previousPlayerPosition; // 상점 입장 전, 플레이어가 있던 월드 좌표
    private bool hasPreviousState = false;  // “이전 맵/위치 정보가 저장되어 있는지” 여부

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

        foreach (var sMap in secretMaps)
            sMap.SetActive(false);

        MovePlayerToStart();

        SaveMapState();
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
            foreach (var shopMap in ShopMaps)
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

    public void SaveMapState()
    {
        previousMapIndex = currentMapIndex;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            previousPlayerPosition = player.transform.position;
        else
            previousPlayerPosition = Vector3.zero;

        hasPreviousState = true;
    }
    public void GoToShopMap(int shopIndex)
    {
        SaveMapState();            // 진입 전 상태 저장
        isInShop = true;
        foreach (var map in maps) map.SetActive(false);
        foreach (var sMap in secretMaps) sMap.SetActive(false);
        foreach (var shopMap in ShopMaps) shopMap.SetActive(false); // 상점 맵들 비활성화

        if (shopIndex >= 0 && shopIndex < ShopMaps.Length)
        {
            ShopMaps[shopIndex].SetActive(true);
            isInShop = true; // 상점으로 진입했음
            MovePlayerToShopStart(shopIndex);
        }
        else
        {
            Debug.LogWarning("상점 인덱스 오류!");
        }
    }
    public void ReturnToPreviousMap()
    {
        if (!hasPreviousState)
        {
            Debug.LogWarning("이전 맵 상태가 저장되어 있지 않습니다. SaveMapState()를 먼저 호출해야 합니다.");
            return;
        }

        // 현재 활성화된 상점맵들 비활성화
        foreach (var shopMap in ShopMaps)
            shopMap.SetActive(false);
        isInShop = false;

        // 이전에 저장된 메인맵 인덱스를 활성화
        maps[previousMapIndex].SetActive(true);
        currentMapIndex = previousMapIndex;

        // 플레이어를 저장된 위치로 이동
        MovePlayerToPosition(previousPlayerPosition);

        hasPreviousState = false;
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
    private void MovePlayerToPosition(Vector3 worldPos)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = worldPos;
        }
    }
    public void ClearRiverStage()
    {
        StoryManager.Instance.SetProgress("RiverStage4Clear");
        GameManager.Instance.gameData.clearedStoryKeys.Add("RiverStage4Clear");
        GameManager.Instance.SaveGame();
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

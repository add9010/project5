using UnityEngine;

public static class BootStrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void LoadManagersContainer()
    {
        // 이미 존재하면 중복 방지
        if (Object.FindAnyObjectByType<ManagerContainer>() != null)
            return;

        // Resources/ManagersContainer.prefab 경로에서 로드
        var prefab = Resources.Load<GameObject>("ManagersContainer");
        if (prefab == null)
        {
            Debug.LogError("Bootstrapper: 'Resources/ManagersContainer.prefab' 을 찾을 수 없습니다!");
            return;
        }

        // Instantiate + 파괴되지 않도록
        var go = Object.Instantiate(prefab);
        Object.DontDestroyOnLoad(go);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHide : MonoBehaviour
{
    [Tooltip("이 씬 이름들에서는 플레이어가 숨겨집니다")]
    public string[] inactiveScenes;

    private Renderer[] renderers;
    private Behaviour[] behaviours;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        behaviours = GetComponentsInChildren<Behaviour>(true);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single); // 초기 적용
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool shouldShow = true;
        foreach (string s in inactiveScenes)
        {
            if (scene.name == s)
            {
                shouldShow = false;
                break;
            }
        }

        // 딱 이것만: 렌더러, 애니메이터, 주요 동작 꺼주기
        foreach (var r in renderers)
            if (r != null) r.enabled = shouldShow;

        foreach (var b in behaviours)
        {
            if (b == null || b == this) continue;

            if (b is Animator || b.GetType().Name.Contains("Move") || b.GetType().Name.Contains("Attack"))
                b.enabled = shouldShow;
        }

        Debug.Log($"[PlayerHide] 씬 {scene.name} → {(shouldShow ? "보임" : "숨김")}");
    }
}

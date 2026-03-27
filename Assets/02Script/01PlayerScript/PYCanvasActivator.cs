using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PYCanvasActivator : MonoBehaviour
{
    [Tooltip("감시할 캔버스 오브젝트 (본인 직접 연결 가능)")]
    public GameObject targetCanvas;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        QueueSceneCheck();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        QueueSceneCheck();
    }

    private void QueueSceneCheck()
    {
        StartCoroutine(CheckPlayerExists());
    }

    private IEnumerator CheckPlayerExists()
    {
        yield return new WaitForSecondsRealtime(0.1f); // 씬 안정화 기다림

        if (targetCanvas == null)
            targetCanvas = GameObject.Find("PYCanvas");

        if (targetCanvas == null)
        {
            yield break;
        }

        bool playerExists = GameObject.FindGameObjectWithTag("Player") != null;
        targetCanvas.SetActive(playerExists);

    }
}

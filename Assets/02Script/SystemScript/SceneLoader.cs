using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public float delayTime = 3f; // 대기 시간 (3초)
    public Text loadingText; // UI 텍스트 연결

    private string[] tips =
    {
        "슬라임은 기본 몬스터입니다.",
        "강한 무기를 얻으려면 던전을 탐험하세요!",
        "포션을 사용하면 체력을 회복할 수 있습니다.",
        "어두운 곳에서는 조명을 활용하세요.",
        "속성 공격은 적에게 추가 피해를 줄 수 있습니다.",
        "특정 몬스터는 특정 약점을 가지고 있습니다."
    };

    void Start()
    {
        ShowRandomTip(); // 랜덤 문구 출력
        StartCoroutine(LoadNextScene()); // 씬 이동 코루틴 실행
    }

    void ShowRandomTip()
    {
        if (loadingText != null)
        {
            int randomIndex = Random.Range(0, tips.Length);
            loadingText.text = tips[randomIndex];
        }
        else
        {
            Debug.LogError("SceneLoader: UI 텍스트가 연결되지 않았습니다!");
        }
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene("Boss1"); // Floor1으로 이동
    }
}

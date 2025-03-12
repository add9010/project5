using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Image 사용을 위해 추가

public class FadeManager : MonoBehaviour
{
    public bool isFadeIn; // true=FadeIn, false=FadeOut
    public GameObject panel; // 페이드 패널 (Image 컴포넌트 필요)
    private Action onCompleteCallback; // Fade 완료 후 실행할 함수
    private Image panelImage; // Image 컴포넌트 캐싱

    void Start()
    {
        if (!panel)
        {
            Debug.LogError("Panel 오브젝트를 찾을 수 없습니다.");
            throw new MissingComponentException();
        }

        panelImage = panel.GetComponent<Image>(); // Image 컴포넌트 가져오기
        if (panelImage == null)
        {
            Debug.LogError("Panel에 Image 컴포넌트가 없습니다!");
            throw new MissingComponentException();
        }

        if (isFadeIn) // Fade In 모드
        {
            panel.SetActive(true); // 패널 활성화
            StartCoroutine(CoFadeIn());
        }
        else
        {
            panel.SetActive(false); // 패널 비활성화
        }
    }

    public void FadeOut()
    {
        panel.SetActive(true); // 패널 활성화
        Debug.Log("FadeCanvasController_ Fade Out 시작");
        StartCoroutine(CoFadeOut());
    }

    IEnumerator CoFadeIn()
    {
        float elapsedTime = 0f;
        float fadeDuration = 1.2f; // 페이드 인 시간

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            panelImage.color = new Color(0f, 0f, 0f, alpha); // 검정색 패널 알파값 조절
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        panelImage.color = new Color(0f, 0f, 0f, 0f); // 완전히 투명
        panel.SetActive(false); // 패널 비활성화
        onCompleteCallback?.Invoke(); // 콜백 실행
    }

    IEnumerator CoFadeOut()
    {
        float elapsedTime = 0f;
        float fadeDuration = 1.2f; // 페이드 아웃 시간

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            panelImage.color = new Color(0f, 0f, 0f, alpha); // 검정색 패널 알파값 조절
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        panelImage.color = new Color(0f, 0f, 0f, 1f); // 완전히 검정색
        Debug.Log("Fade Out 끝");
        onCompleteCallback?.Invoke(); // 콜백 실행
    }

    public void RegisterCallback(Action callback) // 페이드 아웃 완료 후 실행할 콜백 등록
    {
        onCompleteCallback = callback;
    }
}
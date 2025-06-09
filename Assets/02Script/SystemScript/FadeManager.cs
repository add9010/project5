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

    [Header("페이드 설정")]
    public float fadeDuration = 1.2f;

    void Start()
    {
        if (!panel)
        {
            //Debug.LogError("Panel 오브젝트를 찾을 수 없습니다.");
            throw new MissingComponentException();
        }

        panelImage = panel.GetComponent<Image>(); // Image 컴포넌트 가져오기
        if (panelImage == null)
        {
            //Debug.LogError("Panel에 Image 컴포넌트가 없습니다!");
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
        //Debug.Log("FadeCanvasController_ Fade Out 시작");
        StartCoroutine(CoFadeOut());
    }

    IEnumerator CoFadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration) // ← 여기 변경
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            panelImage.color = new Color(0f, 0f, 0f, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        panelImage.color = new Color(0f, 0f, 0f, 0f);
        panel.SetActive(false);
        onCompleteCallback?.Invoke();
    }

    IEnumerator CoFadeOut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration) // ← 여기도 동일하게
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            panelImage.color = new Color(0f, 0f, 0f, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        panelImage.color = new Color(0f, 0f, 0f, 1f);
        onCompleteCallback?.Invoke();
    }
    public void FadeOut(Action callback)
    {
        RegisterCallback(callback); // 콜백 등록
        FadeOut();                  // 기존 페이드아웃 호출
    }
    public void RegisterCallback(Action callback) // 페이드 아웃 완료 후 실행할 콜백 등록
    {
        onCompleteCallback = callback;
    }
}

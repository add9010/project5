using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ArrowBlink : MonoBehaviour
{
    [SerializeField]
    private float fadeTime;
    private Image fadeImage;

    private void Awake()
    {
        fadeImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        StartCoroutine("FadeInOut");
    }

    private void OnDisable()
    {
        StopCoroutine("FadeInOut");
    }

    private IEnumerator FadeInOut()
    {
        while (true)
        {
            yield return StartCoroutine(Fade(1, 0));//Fade In
            yield return StartCoroutine(Fade(0, 1));//Fade Out
        }
    }

    private IEnumerator Fade(float start, float end)
    {
        float currentTime = 0;
        float percent = 0;
        while (percent < 1)
        {
            currentTime += Time.deltaTime;
            percent = currentTime / fadeTime;

            Color color = fadeImage.color;
            color.a = Mathf.Lerp(start, end, percent);
            fadeImage.color = color;

            yield return null;
        }
    }
}

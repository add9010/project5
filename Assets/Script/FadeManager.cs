using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Image ����� ���� �߰�

public class FadeManager : MonoBehaviour
{
    public bool isFadeIn; // true=FadeIn, false=FadeOut
    public GameObject panel; // ���̵� �г� (Image ������Ʈ �ʿ�)
    private Action onCompleteCallback; // Fade �Ϸ� �� ������ �Լ�
    private Image panelImage; // Image ������Ʈ ĳ��

    void Start()
    {
        if (!panel)
        {
            Debug.LogError("Panel ������Ʈ�� ã�� �� �����ϴ�.");
            throw new MissingComponentException();
        }

        panelImage = panel.GetComponent<Image>(); // Image ������Ʈ ��������
        if (panelImage == null)
        {
            Debug.LogError("Panel�� Image ������Ʈ�� �����ϴ�!");
            throw new MissingComponentException();
        }

        if (isFadeIn) // Fade In ���
        {
            panel.SetActive(true); // �г� Ȱ��ȭ
            StartCoroutine(CoFadeIn());
        }
        else
        {
            panel.SetActive(false); // �г� ��Ȱ��ȭ
        }
    }

    public void FadeOut()
    {
        panel.SetActive(true); // �г� Ȱ��ȭ
        Debug.Log("FadeCanvasController_ Fade Out ����");
        StartCoroutine(CoFadeOut());
    }

    IEnumerator CoFadeIn()
    {
        float elapsedTime = 0f;
        float fadeDuration = 1.2f; // ���̵� �� �ð�

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            panelImage.color = new Color(0f, 0f, 0f, alpha); // ������ �г� ���İ� ����
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        panelImage.color = new Color(0f, 0f, 0f, 0f); // ������ ����
        panel.SetActive(false); // �г� ��Ȱ��ȭ
        onCompleteCallback?.Invoke(); // �ݹ� ����
    }

    IEnumerator CoFadeOut()
    {
        float elapsedTime = 0f;
        float fadeDuration = 1.2f; // ���̵� �ƿ� �ð�

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            panelImage.color = new Color(0f, 0f, 0f, alpha); // ������ �г� ���İ� ����
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        panelImage.color = new Color(0f, 0f, 0f, 1f); // ������ ������
        Debug.Log("Fade Out ��");
        onCompleteCallback?.Invoke(); // �ݹ� ����
    }

    public void RegisterCallback(Action callback) // ���̵� �ƿ� �Ϸ� �� ������ �ݹ� ���
    {
        onCompleteCallback = callback;
    }
}
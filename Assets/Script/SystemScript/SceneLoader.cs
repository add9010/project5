using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public float delayTime = 3f; // ��� �ð� (3��)
    public Text loadingText; // UI �ؽ�Ʈ ����

    private string[] tips =
    {
        "�������� �⺻ �����Դϴ�.",
        "���� ���⸦ �������� ������ Ž���ϼ���!",
        "������ ����ϸ� ü���� ȸ���� �� �ֽ��ϴ�.",
        "��ο� �������� ������ Ȱ���ϼ���.",
        "�Ӽ� ������ ������ �߰� ���ظ� �� �� �ֽ��ϴ�.",
        "Ư�� ���ʹ� Ư�� ������ ������ �ֽ��ϴ�."
    };

    void Start()
    {
        ShowRandomTip(); // ���� ���� ���
        StartCoroutine(LoadNextScene()); // �� �̵� �ڷ�ƾ ����
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
            Debug.LogError("SceneLoader: UI �ؽ�Ʈ�� ������� �ʾҽ��ϴ�!");
        }
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene("Floor1"); // Floor1���� �̵�
    }
}
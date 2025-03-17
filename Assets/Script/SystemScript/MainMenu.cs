using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour//,IPointerEnterHandler,IPointerExitHandler
{
    public FadeManager fadeManager; // ���̵� �Ŵ��� ����


    public void OnClickNewGame()
    {
        Debug.Log("�� ���� ����");

        // ���̵� �ƿ� ����
        if (fadeManager != null)
        {
            fadeManager.RegisterCallback(() => SceneManager.LoadScene("Loding")); // ���̵� �ƿ� �� �� �̵�
            fadeManager.FadeOut();
        }
        else
        {
            SceneManager.LoadScene("Loding"); // ���̵� �Ŵ����� ������ �׳� �� �̵�
        }
    }

    public void OnClickLoad()
    {
        Debug.Log("�ҷ�����");

        // ���� ������ �ҷ�����
        DateManager.Instance.LoadGameData();

        // �ҷ��� �����Ͱ� ����Ǿ����� Ȯ��
        for (int i = 0; i < DateManager.Instance.data.isUnlock.Length; i++)
        {
            Debug.Log($"{i}�� é�� ��� ���� ���� : " + DateManager.Instance.data.isUnlock[i]);
        }

        // �� ��ȯ (��: ���� ������ �̵�)
        if (fadeManager != null)
        {
            fadeManager.RegisterCallback(() => SceneManager.LoadScene("Floor1")); // �ҷ��� �� ���Ӿ� �̵�
            fadeManager.FadeOut();
        }
        else
        {
            SceneManager.LoadScene("Floor1");
        }
    }


    public void OnClickOption()
    {
        Debug.Log("�ɼ�");
    }

    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}


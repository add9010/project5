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


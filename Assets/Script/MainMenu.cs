using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour//,IPointerEnterHandler,IPointerExitHandler
{
    /*
        public enum BTNType
        {
            New,
            Contiune,
            Option,
            Sound,
            Back,
            Quit
        }
    
    public Transform buttonScale;
    Vector3 defaultScale;
   
    private void Start()
    {
        defaultScale = buttonScale.localScale;
    }
     */
    public void OnClickNewGame()
    {
        Debug.Log("�� ����");
        SceneManager.LoadScene("Floor3");
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
    /*
    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonScale.localScale = defaultScale * 1.2f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonScale.localScale = defaultScale;
    }
    */
}


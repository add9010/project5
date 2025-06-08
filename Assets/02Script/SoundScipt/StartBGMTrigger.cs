using UnityEngine;

public class StartBGMTrigger : MonoBehaviour
{
    public AudioClip selectBGM;

    private void Start()
    {
        if (SoundManager.Instance != null && selectBGM != null)
        {
            SoundManager.Instance.PlayBGM(selectBGM);
        }
    }
}

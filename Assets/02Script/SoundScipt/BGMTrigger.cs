using UnityEngine;

public class BGMTrigger : MonoBehaviour
{
    public AudioClip bgmClip;
    private bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasPlayed)
        {
            SoundManager.Instance.PlayBGM(bgmClip); // 여기에 Clip만 넘김
            hasPlayed = true;
        }
    }
}

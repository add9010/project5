using UnityEngine;

public class BGMTrigger : MonoBehaviour
{
    public AudioClip bgmClip;
    void Start()
    {
        // 이거 한 줄이면 로딩 시 클립이 메모리에 올라감
        AudioClip preload = bgmClip;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.Instance.PlayBGM(bgmClip);
            Debug.Log("배경음 재생 시작됨: " + bgmClip.name);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.Instance.StopBGM();
            Debug.Log("플레이어가 지역에서 나감 → BGM 정지");
        }
    }
}

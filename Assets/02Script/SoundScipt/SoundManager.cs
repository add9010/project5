using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource bgmSource;
    public AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null)
        {
            //Debug.LogWarning("클립이 null입니다.");
            return;
        }

        if (!bgmSource.gameObject.activeInHierarchy || !bgmSource.enabled)
        {
            //Debug.LogError("bgmSource가 비활성화되어 있습니다!");
            return;
        }

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip, volume);
    }
    public void StopBGM()
    {
        if (bgmSource == null)
        {
            //Debug.LogWarning("bgmSource가 null입니다. 이미 제거된 상태입니다.");
            return;
        }

        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
            //Debug.Log("BGM 정지됨");
        }
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class OptionMenu : MonoBehaviour
{
    // 싱글톤 패턴
    public static OptionMenu Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (optionPanel == null)
            Debug.LogError("OptionMenu: optionPanel이 할당되지 않았습니다!");
        else
            optionPanel.SetActive(false);
    }
    
    // ------------------ 옵션 설정 관련 변수 ------------------
    [Header("옵션창")]
    public GameObject optionPanel;
    public GameObject warningPopup;

    private float savedMasterVol;
    private float savedBGMVol;
    private float savedSFXVol;
    private int savedResolutionIndex;

    [Header("슬라이더")]
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    [Header("음소거 버튼 이미지")]
    public Image masterMuteIcon;
    public Image bgmMuteIcon;
    public Image sfxMuteIcon;

    [Header("음소거 스프라이트")]
    public Sprite muteOnSprite;
    public Sprite muteOffSprite;

    [Header("오디오")]
    public AudioMixer audioMixer;
    public string masterParam = "MasterVolume";
    public string bgmParam = "BGMVolume";
    public string sfxParam = "SFXVolume";

    private bool isMasterMuted = false;
    private bool isBGMMuted = false;
    private bool isSFXMuted = false;

    private float prevMasterVol = 1f;
    private float prevBGMVol = 1f;
    private float prevSFXVol = 1f;

    private float masterRatio = 1f;
    private float bgmBaseVolume = 1f;
    private float sfxBaseVolume = 1f;

    public TextMeshProUGUI masterVolumeText;
    public TextMeshProUGUI bgmVolumeText;
    public TextMeshProUGUI sfxVolumeText;

    // ------------------ 해상도 설정 ------------------
    [Header("해상도 설정")]
    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions = new List<Resolution>
    {
        new Resolution { width = 2560, height = 1440 },
        new Resolution { width = 1920, height = 1080 },
        new Resolution { width = 1600, height = 900 },
        new Resolution { width = 1280, height = 720 },
    };

    // ------------------ 초기화 ------------------
    private void Start()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
            masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        if (PlayerPrefs.HasKey("BGMVolume"))
            bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume");
        if (PlayerPrefs.HasKey("SFXVolume"))
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");

        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            int index = PlayerPrefs.GetInt("ResolutionIndex");
            resolutionDropdown.value = index;
            OnResolutionChange(index);
        }
        else
        {
            Screen.SetResolution(filteredResolutions[0].width, filteredResolutions[0].height, FullScreenMode.Windowed);
        }

        prevMasterVol = masterSlider.value;
        prevBGMVol = bgmSlider.value;
        prevSFXVol = sfxSlider.value;

        savedMasterVol = masterSlider.value;
        savedBGMVol = bgmSlider.value;
        savedSFXVol = sfxSlider.value;
        savedResolutionIndex = resolutionDropdown.value;

        isMasterMuted = Mathf.Approximately(masterSlider.value, 0f);
        isBGMMuted = Mathf.Approximately(bgmSlider.value, 0f);
        isSFXMuted = Mathf.Approximately(sfxSlider.value, 0f);

        UpdateMuteIcon(masterMuteIcon, isMasterMuted);
        UpdateMuteIcon(bgmMuteIcon, isBGMMuted);
        UpdateMuteIcon(sfxMuteIcon, isSFXMuted);

        if (optionPanel != null)
            optionPanel.SetActive(false);

        InitResolutionDropdown();

        OnMasterVolumeChange();
        OnBGMVolumeChange();
        OnSFXVolumeChange();
    }

    // ------------------ 옵션창 열고 닫기 ------------------
    public void OpenOption()
    {
        if (optionPanel != null)
        {
            optionPanel.SetActive(true);
            Time.timeScale = 0f;  // 열 때 일시정지
        }
    }

    public void CloseOption()
    {
        if (optionPanel != null)
        {
            optionPanel.SetActive(false);
            Time.timeScale = 1f;  // 닫을 때 다시 재개
        }
    }
    public void TryCloseOption() => warningPopup.SetActive(true);

    public void ConfirmCloseOption()
    {
        masterSlider.SetValueWithoutNotify(savedMasterVol);
        bgmSlider.SetValueWithoutNotify(savedBGMVol);
        sfxSlider.SetValueWithoutNotify(savedSFXVol);

        masterVolumeText.text = Mathf.RoundToInt(masterSlider.value * 100).ToString();
        bgmVolumeText.text = Mathf.RoundToInt(bgmSlider.value * 100).ToString();
        sfxVolumeText.text = Mathf.RoundToInt(sfxSlider.value * 100).ToString();

        UpdateMuteIcon(masterMuteIcon, isMasterMuted);
        UpdateMuteIcon(bgmMuteIcon, isBGMMuted);
        UpdateMuteIcon(sfxMuteIcon, isSFXMuted);

        resolutionDropdown.SetValueWithoutNotify(savedResolutionIndex);
        resolutionDropdown.RefreshShownValue();
        OnResolutionChange(savedResolutionIndex);

        ApplySliderVolume(masterParam, savedMasterVol);
        ApplySliderVolume(bgmParam, savedBGMVol);
        ApplySliderVolume(sfxParam, savedSFXVol);

        optionPanel.SetActive(false);
        warningPopup.SetActive(false);
        Time.timeScale = 1f;
    }

    public void CancelCloseOption() => warningPopup.SetActive(false);

    // ------------------ 볼륨 조절 ------------------
    public void OnMasterVolumeChange()
    {
        masterRatio = masterSlider.value;
        masterVolumeText.text = Mathf.RoundToInt(masterRatio * 100).ToString();

        // 시각적으로도 동기화: BGM/SFX 슬라이더 위치를 비례로 반영
        bgmSlider.SetValueWithoutNotify(bgmBaseVolume * masterRatio);
        sfxSlider.SetValueWithoutNotify(sfxBaseVolume * masterRatio);
        // 실제 믹서     
        ApplySliderVolume(bgmParam, bgmBaseVolume * masterRatio);
        ApplySliderVolume(sfxParam, sfxBaseVolume * masterRatio);
        // 텍스트도 같이 반영
        bgmVolumeText.text = Mathf.RoundToInt(bgmBaseVolume * masterRatio * 100).ToString();
        sfxVolumeText.text = Mathf.RoundToInt(sfxBaseVolume * masterRatio * 100).ToString();
    }

    public void OnBGMVolumeChange()
    {
        bgmBaseVolume = bgmSlider.value;
        bgmVolumeText.text = Mathf.RoundToInt(bgmBaseVolume * 100).ToString();
        ApplySliderVolume(bgmParam, bgmBaseVolume * masterRatio);
    }

    public void OnSFXVolumeChange()
    {
        sfxBaseVolume = sfxSlider.value;
        sfxVolumeText.text = Mathf.RoundToInt(sfxBaseVolume * 100).ToString();
        ApplySliderVolume(sfxParam, sfxBaseVolume * masterRatio);
    }

    private void ApplySliderVolume(string paramName, float value)
    {
        float dB = value > 0.0001f ? Mathf.Log10(value) * 20f : -80f;
        audioMixer.SetFloat(paramName, dB);
    }

    // ------------------ 음소거 토글 ------------------
    public void ToggleMasterMute()
    {
        isMasterMuted = !isMasterMuted;

        if (isMasterMuted)
        {
            prevMasterVol = masterSlider.value;
            prevBGMVol = bgmSlider.value;
            prevSFXVol = sfxSlider.value;

            masterSlider.SetValueWithoutNotify(0f);
            bgmSlider.SetValueWithoutNotify(0f);
            sfxSlider.SetValueWithoutNotify(0f);

            masterVolumeText.text = "0";
            bgmVolumeText.text = "0";
            sfxVolumeText.text = "0";

            ApplySliderVolume(masterParam, 0f);
            ApplySliderVolume(bgmParam, 0f);
            ApplySliderVolume(sfxParam, 0f);

            UpdateMuteIcon(masterMuteIcon, true);
            UpdateMuteIcon(bgmMuteIcon, true);
            UpdateMuteIcon(sfxMuteIcon, true);

            isBGMMuted = true;
            isSFXMuted = true;
        }
        else
        {
            masterSlider.SetValueWithoutNotify(prevMasterVol);
            bgmSlider.SetValueWithoutNotify(prevBGMVol);
            sfxSlider.SetValueWithoutNotify(prevSFXVol);

            masterVolumeText.text = Mathf.RoundToInt(prevMasterVol * 100).ToString();
            bgmVolumeText.text = Mathf.RoundToInt(prevBGMVol * 100).ToString();
            sfxVolumeText.text = Mathf.RoundToInt(prevSFXVol * 100).ToString();

            ApplySliderVolume(masterParam, prevMasterVol);
            ApplySliderVolume(bgmParam, prevBGMVol);
            ApplySliderVolume(sfxParam, prevSFXVol);

            UpdateMuteIcon(masterMuteIcon, false);
            UpdateMuteIcon(bgmMuteIcon, false);
            UpdateMuteIcon(sfxMuteIcon, false);

            isBGMMuted = false;
            isSFXMuted = false;
        }
    }


    public void ToggleBGMMute()
    {
        isBGMMuted = !isBGMMuted;

        if (isBGMMuted)
        {
            prevBGMVol = bgmSlider.value;
            bgmSlider.SetValueWithoutNotify(0f);
            ApplySliderVolume(bgmParam, 0f);
        }
        else
        {
            bgmSlider.SetValueWithoutNotify(prevBGMVol);
            ApplySliderVolume(bgmParam, prevBGMVol);
        }

        UpdateMuteIcon(bgmMuteIcon, isBGMMuted);
        bgmVolumeText.text = isBGMMuted ? "0" : Mathf.RoundToInt(prevBGMVol * 100).ToString();

    }

    public void ToggleSFXMute()
    {
        isSFXMuted = !isSFXMuted;

        if (isSFXMuted)
        {
            prevSFXVol = sfxSlider.value;
            sfxSlider.SetValueWithoutNotify(0f);
            ApplySliderVolume(sfxParam, 0f);
        }
        else
        {
            sfxSlider.SetValueWithoutNotify(prevSFXVol);
            ApplySliderVolume(sfxParam, prevSFXVol);
        }

        UpdateMuteIcon(sfxMuteIcon, isSFXMuted);
        sfxVolumeText.text = isSFXMuted ? "0" : Mathf.RoundToInt(prevSFXVol * 100).ToString();

    }

    // ------------------ 아이콘 스프라이트 갱신 ------------------
    private void UpdateMuteIcon(Image targetImage, bool isMuted)
    {
        if (targetImage != null)
            targetImage.sprite = isMuted ? muteOffSprite : muteOnSprite;
    }

    // ------------------ 해상도 설정 ------------------
    public void OnResolutionChange(int index)
    {
        Resolution res = filteredResolutions[index];
        Screen.SetResolution(res.width, res.height, FullScreenMode.Windowed);
    }

    private void InitResolutionDropdown()
    {
        List<string> options = new List<string>();
        int currentIndex = 0;

        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string option = filteredResolutions[i].width + " x " + filteredResolutions[i].height;
            options.Add(option);

            if (Screen.width == filteredResolutions[i].width &&
                Screen.height == filteredResolutions[i].height)
            {
                currentIndex = i;
            }
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentIndex;
        resolutionDropdown.RefreshShownValue();
    }

    // ------------------ 옵션 저장 ------------------
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
        PlayerPrefs.SetFloat("BGMVolume", bgmSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionDropdown.value);
        PlayerPrefs.Save();

        savedMasterVol = masterSlider.value;
        savedBGMVol = bgmSlider.value;
        savedSFXVol = sfxSlider.value;
        savedResolutionIndex = resolutionDropdown.value;

        Debug.Log("옵션 저장 완료");
    }
}

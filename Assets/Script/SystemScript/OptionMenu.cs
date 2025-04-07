using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;


public class OptionMenu : MonoBehaviour
{
    [Header("옵션창")]
    public GameObject optionPanel;
    public GameObject warningPopup;
    //------------------- 옵션창 초기화   -------------------
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

    // 내부 상태 저장
    private bool isMasterMuted = false;
    private bool isBGMMuted = false;
    private bool isSFXMuted = false;

    private float prevMasterVol = 1f;
    private float prevBGMVol = 1f;
    private float prevSFXVol = 1f;

    public TextMeshProUGUI masterVolumeText;
    public TextMeshProUGUI bgmVolumeText;
    public TextMeshProUGUI sfxVolumeText;

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
            OnResolutionChange(index); // 즉시 적용
        }
        else
        {
            // 기본 해상도 설정
            Screen.SetResolution(filteredResolutions[0].width, filteredResolutions[0].height, FullScreenMode.Windowed);
        }

        // 초기 슬라이더 값
        ApplySliderVolume(masterParam, masterSlider.value);
        ApplySliderVolume(bgmParam, bgmSlider.value);
        ApplySliderVolume(sfxParam, sfxSlider.value);

        prevMasterVol = masterSlider.value;
        prevBGMVol = bgmSlider.value;
        prevSFXVol = sfxSlider.value;

        savedMasterVol = masterSlider.value;
        savedBGMVol = bgmSlider.value;
        savedSFXVol = sfxSlider.value;
        savedResolutionIndex = resolutionDropdown.value;

        // 음소거 상태 판단
        isMasterMuted = Mathf.Approximately(masterSlider.value, 0f);
        isBGMMuted = Mathf.Approximately(bgmSlider.value, 0f);
        isSFXMuted = Mathf.Approximately(sfxSlider.value, 0f);

        // 아이콘 갱신
        UpdateMuteIcon(masterMuteIcon, isMasterMuted);
        UpdateMuteIcon(bgmMuteIcon, isBGMMuted);
        UpdateMuteIcon(sfxMuteIcon, isSFXMuted);

        if (optionPanel != null)
            optionPanel.SetActive(false);

        // 필터링된 해상도 목록 가져오기
        InitResolutionDropdown();

        // 텍스트 동기화 (볼륨 % 표시)
        OnMasterVolumeChange();
        OnBGMVolumeChange();
        OnSFXVolumeChange();

    }

    // ------------------ 옵션창 열고 닫기 ------------------

    public void OpenOption() => optionPanel?.SetActive(true);
    public void CloseOption() => optionPanel?.SetActive(false);
    public void TryCloseOption() => warningPopup.SetActive(true);
    public void ConfirmCloseOption()
    {
        // 슬라이더 UI, 텍스트, 음소거 아이콘 복원
        masterSlider.SetValueWithoutNotify(savedMasterVol);
        bgmSlider.SetValueWithoutNotify(savedBGMVol);
        sfxSlider.SetValueWithoutNotify(savedSFXVol);

        masterVolumeText.text = Mathf.RoundToInt(masterSlider.value * 100).ToString();
        bgmVolumeText.text = Mathf.RoundToInt(bgmSlider.value * 100).ToString();
        sfxVolumeText.text = Mathf.RoundToInt(sfxSlider.value * 100).ToString();

        UpdateMuteIcon(masterMuteIcon, isMasterMuted);
        UpdateMuteIcon(bgmMuteIcon, isBGMMuted);
        UpdateMuteIcon(sfxMuteIcon, isSFXMuted);
     
        // 해상도 복원
        resolutionDropdown.SetValueWithoutNotify(savedResolutionIndex);
        resolutionDropdown.RefreshShownValue();
        OnResolutionChange(savedResolutionIndex); 

        // AudioMixer에도 복원
        ApplySliderVolume(masterParam, savedMasterVol);
        ApplySliderVolume(bgmParam, savedBGMVol);
        ApplySliderVolume(sfxParam, savedSFXVol);

        optionPanel.SetActive(false);
        warningPopup.SetActive(false);
    }

    public void CancelCloseOption() => warningPopup.SetActive(false);
    
    // ------------------ 볼륨 조절 ------------------

    public void OnMasterVolumeChange()
    {
        float value = masterSlider.value;
        ApplySliderVolume(masterParam, value);
        masterVolumeText.text = Mathf.RoundToInt(value * 100).ToString();
    }

    public void OnBGMVolumeChange()
    {
        float value = bgmSlider.value;
        ApplySliderVolume(bgmParam, value);
        bgmVolumeText.text = Mathf.RoundToInt(value * 100).ToString();
    }

    public void OnSFXVolumeChange()
    {
        float value = sfxSlider.value;
        ApplySliderVolume(sfxParam, value);
        sfxVolumeText.text = Mathf.RoundToInt(value * 100).ToString();
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
    }

    // ------------------ 아이콘 스프라이트 갱신 ------------------

    private void UpdateMuteIcon(Image targetImage, bool isMuted)
    {
        if (targetImage != null)
        {
            targetImage.sprite = isMuted ? muteOffSprite : muteOnSprite;
        }
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
    // ------------------ 옵션 변경 저장 ------------------
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
        PlayerPrefs.SetFloat("BGMVolume", bgmSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionDropdown.value);
        PlayerPrefs.Save();

        // 저장된 값도 덮어쓰기
        savedMasterVol = masterSlider.value;
        savedBGMVol = bgmSlider.value;
        savedSFXVol = sfxSlider.value;
        savedResolutionIndex = resolutionDropdown.value;

        Debug.Log("옵션 저장 완료");
    }

}

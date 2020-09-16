using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Option : BaseUI
{
    enum EOptionOption
    {
        Volume,
        Video,
        Game,
        Notice,
        Account
    }
    Option_Preview m_optionPreview;
    EOptionOption m_currOption;
    Dictionary<EOptionOption, GameObject> m_optionDic = new Dictionary<EOptionOption, GameObject>();
    GameObject m_gameWindow;
    GameObject m_noticeWindow;
    GameObject m_accountWindow;

    // Sound
    GameObject m_volumeWindow;
    Slider m_musicSlider;
    Toggle m_musicToggle;
    Slider m_soundSlider;
    Toggle m_soundToggle;

    float m_prevMusicValue;
    float m_prevSoundValue;
    bool m_prevUseMusic;
    bool m_prevUseSound;

    // Video
    GameObject m_videoWindow;
    Toggle m_weatherToggle;
    Toggle m_bloomToggle;
    Toggle m_ambientToggle;
    Toggle m_outlineToggle;
    Toggle m_fogToggle;

    bool m_prevUseWeather;
    bool m_prevUseBloom;
    bool m_prevUseAmbient;
    bool m_prevUseOutline;
    bool m_prevUseFog;

    // Game
    Dropdown m_frameDropDown;
    Dropdown m_autoAimDropDown;
    Dropdown m_joystickDropDown;
    Toggle m_cameraHoldToggle;

    int m_prevFrameHandle;
    int m_prevJoysitckHandle;
    bool m_prevHoldCam;
    int m_prevAutoAim;

    protected override void InitUI()
    {
        m_optionPreview = GetComponentInChildren<Option_Preview>(true).Init();
        transform.Find("Exit").GetComponent<Button>().onClick.AddListener(OnClickCancle);
        transform.Find("Save").GetComponent<Button>().onClick.AddListener(OnClickSave);
        transform.Find("Quit").GetComponent<Button>().onClick.AddListener(NetworkMng.Instance.RequestLogoutCharacter);
        Transform BTNGrid = transform.Find("ButtonGroup");

        // Volume
        m_volumeWindow = transform.Find("VolumeWindow").gameObject;
        BTNGrid.Find("VolumeBTN").GetComponent<Button>().onClick.AddListener(()=> ShowOption(EOptionOption.Volume));
        m_musicSlider = m_volumeWindow.transform.Find("Music").GetComponentInChildren<Slider>();
        m_musicSlider.onValueChanged.AddListener((float a) => GameSystem.MusicVolume = a);
        m_musicToggle = m_volumeWindow.transform.Find("Music").GetComponentInChildren<Toggle>();
        m_musicToggle.onValueChanged.AddListener((bool a) => GameSystem.UseMusic = a);
        m_soundSlider = m_volumeWindow.transform.Find("Sound").GetComponentInChildren<Slider>();
        m_soundSlider.onValueChanged.AddListener((float a) => GameSystem.SoundVolume = a);
        m_soundToggle = m_volumeWindow.transform.Find("Sound").GetComponentInChildren<Toggle>();
        m_soundToggle.onValueChanged.AddListener((bool a) => GameSystem.UseSound = a);

        // Video
        m_videoWindow = transform.Find("VideoWindow").gameObject;
        BTNGrid.Find("VideoBTN").GetComponent<Button>().onClick.AddListener(() => ShowOption(EOptionOption.Video));
        m_bloomToggle = m_videoWindow.transform.Find("Bloom").GetComponent<Toggle>();
        m_bloomToggle.onValueChanged.AddListener((bool a) => GameSystem.UseBloom = a);
        m_bloomToggle.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => m_optionPreview.Open("BloomPreview", "화면의 밝은 영역을 번지게하여 부드럽게 보이게 합니다.<color=red>\n높은 사양을 요구합니다.</color>"));
        m_ambientToggle = m_videoWindow.transform.Find("Ambient").GetComponent<Toggle>();
        m_ambientToggle.onValueChanged.AddListener((bool a) => GameSystem.UseAmbient = a);
        m_ambientToggle.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => m_optionPreview.Open("AmbientPreview", "어두운 영역을 더 어둡게 처리하여 깊이감을 증가시킵니다.<color=red>\n높은 사양을 요구합니다</color>"));
        m_outlineToggle = m_videoWindow.transform.Find("Outline").GetComponent<Toggle>();
        m_outlineToggle.onValueChanged.AddListener((bool a) => GameSystem.UseOutline = a);
        m_outlineToggle.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => m_optionPreview.Open("OutlinePreview", "특정상태에서의 외곽선효과를 보일지 결정합니다."));
        m_fogToggle = m_videoWindow.transform.Find("Fog").GetComponent<Toggle>();
        m_fogToggle.onValueChanged.AddListener((bool a) => GameSystem.UseFog = a);
        m_weatherToggle = m_videoWindow.transform.Find("Weather").GetComponent<Toggle>();
        m_weatherToggle.onValueChanged.AddListener((bool a) => GameSystem.UseWeather = a);
        m_videoWindow.SetActive(false);

        // Game
        m_gameWindow = transform.Find("GameWindow").gameObject;
        BTNGrid.Find("GameBTN").GetComponent<Button>().onClick.AddListener(() => ShowOption(EOptionOption.Game));
        m_frameDropDown = m_gameWindow.transform.Find("FrameSelect").GetComponent<Dropdown>();
        m_frameDropDown.onValueChanged.AddListener((int a) => GameSystem.FPS = a);
        m_joystickDropDown = m_gameWindow.transform.Find("JoystickSelect").GetComponent<Dropdown>();
        m_joystickDropDown.onValueChanged.AddListener((int a) => GameSystem.Joystick = a);
        m_cameraHoldToggle = m_gameWindow.transform.Find("CameraHold").GetComponent<Toggle>();
        m_cameraHoldToggle.onValueChanged.AddListener((bool a) => GameSystem.PlayerCameraHoldRot = a);
        m_autoAimDropDown = m_gameWindow.transform.Find("AutoAim").GetComponent<Dropdown>();
        m_autoAimDropDown.onValueChanged.AddListener((int a) => GameSystem.AutoAim = a);
        m_gameWindow.SetActive(false);

        m_optionDic.Add(EOptionOption.Volume, m_volumeWindow);
        m_optionDic.Add(EOptionOption.Video, m_videoWindow);
        m_optionDic.Add(EOptionOption.Game, m_gameWindow);
        m_optionDic.Add(EOptionOption.Notice, m_noticeWindow);
        m_optionDic.Add(EOptionOption.Account, m_accountWindow);
    }
    public override void Open()
    {
        m_prevMusicValue = GameSystem.MusicVolume;
        m_prevUseMusic = GameSystem.UseMusic;
        m_prevSoundValue = GameSystem.SoundVolume;
        m_prevUseSound = GameSystem.UseSound;

        m_musicSlider.value = GameSystem.MusicVolume;
        m_musicToggle.isOn = GameSystem.UseMusic;
        m_soundSlider.value = GameSystem.SoundVolume;
        m_soundToggle.isOn = GameSystem.UseSound;

        m_prevUseWeather = GameSystem.UseWeather;
        m_prevUseBloom = GameSystem.UseBloom;
        m_prevUseAmbient = GameSystem.UseAmbient;
        m_prevUseOutline = GameSystem.UseOutline;
        m_prevUseFog = GameSystem.UseFog;

        m_weatherToggle.isOn = GameSystem.UseWeather;
        m_bloomToggle.isOn = GameSystem.UseBloom;
        m_ambientToggle.isOn = GameSystem.UseAmbient;
        m_outlineToggle.isOn = GameSystem.UseOutline;
        m_fogToggle.isOn = GameSystem.UseFog;

        m_prevFrameHandle = GameSystem.FPS;
        m_prevJoysitckHandle = GameSystem.Joystick;
        m_prevAutoAim = GameSystem.AutoAim;
        m_prevHoldCam = GameSystem.PlayerCameraHoldRot;

        m_frameDropDown.value = GameSystem.FPS;
        m_joystickDropDown.value = GameSystem.Joystick;
        m_autoAimDropDown.value = GameSystem.AutoAim;
        m_cameraHoldToggle.isOn = GameSystem.PlayerCameraHoldRot;

        ShowOption(m_currOption);
        gameObject.SetActive(true);
    }
    public override void Close()
    {
        m_optionPreview.Close();
        gameObject.SetActive(false);
    }
    void ShowOption(EOptionOption option)
    {
        if (m_currOption == option)
            return;

        m_optionDic[m_currOption].SetActive(false);
        m_currOption = option;
        m_optionDic[m_currOption].SetActive(true);
    }
    void OnClickCancle()
    {
        GameSystem.MusicVolume = m_prevMusicValue;
        GameSystem.UseMusic = m_prevUseMusic;
        GameSystem.SoundVolume = m_prevSoundValue;
        GameSystem.UseSound = m_prevUseSound;

        GameSystem.UseWeather = m_prevUseWeather;
        GameSystem.UseBloom = m_prevUseBloom;
        GameSystem.UseAmbient = m_prevUseAmbient;
        GameSystem.UseOutline = m_prevUseOutline;
        GameSystem.UseFog = m_prevUseFog;

        GameSystem.FPS = m_prevFrameHandle;
        GameSystem.Joystick = m_prevJoysitckHandle;
        GameSystem.AutoAim = m_prevAutoAim;
        GameSystem.PlayerCameraHoldRot = m_prevHoldCam;

        Close();
    }
    void OnClickSave()
    {
        GameSystem.MusicVolume = m_musicSlider.value;
        GameSystem.UseMusic = m_musicToggle.isOn;
        GameSystem.SoundVolume = m_soundSlider.value;
        GameSystem.UseSound = m_soundToggle.isOn;

        GameSystem.UseWeather = m_weatherToggle.isOn;
        GameSystem.UseBloom = m_bloomToggle.isOn;
        GameSystem.UseAmbient = m_ambientToggle.isOn;
        GameSystem.UseOutline = m_outlineToggle.isOn;
        GameSystem.UseFog = m_fogToggle.isOn;

        GameSystem.FPS = m_frameDropDown.value;
        GameSystem.Joystick = m_joystickDropDown.value;
        GameSystem.AutoAim = m_autoAimDropDown.value;
        GameSystem.PlayerCameraHoldRot = m_cameraHoldToggle.isOn;

        GameSystem.SaveFileStream();
        Close();
    }
}

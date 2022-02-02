using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    private MainManager mainManager;

    [Space(4)]
    [SerializeField] private CanvasGroup settingsCanvas;
    [SerializeField] private CanvasGroup canvasGroupFade;
    [SerializeField] private GameObject btnMainMenu;
    [SerializeField] private GameObject btnReset;
    [SerializeField] private Button btnCloseSettingsMenu;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider sldrMainV;
    [SerializeField] private Slider sldrSfxV;
    [SerializeField] private Slider sldrUiV;
    [SerializeField] private Slider sldrMusicV;
    [SerializeField] private Toggle tglFullscreen;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualtyDropdown;
    private Resolution[] _resolutions;
    private bool canPause;
    private PlayerActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerActions();
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
    }

    private void Start()
    {
        canvasGroupFade.gameObject.SetActive(false);
        canvasGroupFade.alpha = 0;
        btnCloseSettingsMenu.onClick.AddListener(() => {
            canPause = true;
            GamePause(false);
            CloseSettingsMenu();
            if (SceneManager.GetActiveScene().name.CompareTo("Testing") == 0)
            {
                CursorController.instance.ControlCursor(CursorController.CursorState.Locked);
            }
            else
            {
                CursorController.instance.ControlCursor(CursorController.CursorState.Unlocked);
            }
        });
        canPause = true;
        GamePause(false);
        //Resolutions
        _resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < _resolutions.Length; i++)
        {
            string option = _resolutions[i].width + "x" + _resolutions[i].height;
            options.Add(option);

            if (_resolutions[i].width == Screen.width && _resolutions[i].height == Screen.width)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(delegate { SetResolution(resolutionDropdown.value); });
        //Game Quality
        qualtyDropdown.onValueChanged.AddListener(delegate { SetQuality(qualtyDropdown.value); });
        //All volume controls
        sldrMainV.onValueChanged.AddListener(delegate { SetMainVolume(sldrMainV.value); });
        sldrSfxV.onValueChanged.AddListener(delegate { SetSfxMainVolume(sldrSfxV.value); });
        sldrMusicV.onValueChanged.AddListener(delegate { SetMusicVolume(sldrMusicV.value); });
        sldrUiV.onValueChanged.AddListener(delegate { SetUiMainVolume(sldrUiV.value); });
        //Full screen control
        tglFullscreen.onValueChanged.AddListener(delegate { SetFullScreen(tglFullscreen.isOn); });
        //ESC key actions
        inputActions.Land.Pause.performed += ctx => {
            if (SceneManager.GetActiveScene().name.CompareTo("Main Menu") != 0)
            {
                if (canPause)
                {
                    //MainManager.instance.GetPlayer().GetComponent<PlayerInput>().SwitchCurrentActionMap("Menu Controls");
                    //Debug.Log(MainManager.instance.GetPlayer().GetComponent<PlayerInput>().currentActionMap.name.ToString());
                    canPause = false;
                    GamePause(true);
                    OpenSettingsMenu();
                    if (SceneManager.GetActiveScene().name.CompareTo("Testing") == 0)
                    {
                        CursorController.instance.ControlCursor(CursorController.CursorState.Unlocked);
                    }
                    else
                    {
                        CursorController.instance.ControlCursor(CursorController.CursorState.Locked);
                    }

                }
                else
                {
                    //MainManager.instance.GetPlayer().GetComponent<PlayerInput>().SwitchCurrentActionMap("Land");
                    //Debug.Log(MainManager.instance.GetPlayer().GetComponent<PlayerInput>().currentActionMap.name.ToString());
                    canPause = true;
                    GamePause(false);
                    CloseSettingsMenu();
                    if (SceneManager.GetActiveScene().name.CompareTo("Testing") == 0)
                    {
                        CursorController.instance.ControlCursor(CursorController.CursorState.Locked);
                    }
                    else
                    {
                        CursorController.instance.ControlCursor(CursorController.CursorState.Unlocked);
                    }
                }
            }
        };

    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        LoadPlayerPrefsSettingsValues();

        if (arg0.name.CompareTo("Testing") == 0)
        {
            mainManager = FindObjectOfType<MainManager>();
        }

        if (string.Compare(arg0.name, "Testing") != 0)
        {
            btnMainMenu.SetActive(false);
            btnReset.SetActive(false);
        }
        else
        {
            btnMainMenu.SetActive(true);
            btnReset.SetActive(true);
        }
    }

    private void SceneManager_sceneUnloaded(Scene arg0)
    {
        UpdatePlayerPrefsSettingsValues();
    }

    private void UpdatePlayerPrefsSettingsValues()
    {
        //Resolutions
        PlayerPrefs.SetInt("Resolution", resolutionDropdown.value);

        //Game Quality
        PlayerPrefs.SetInt("GameQuality", QualitySettings.GetQualityLevel());

        //All volume vals
        PlayerPrefs.SetFloat("Volume_Main", sldrMainV.value);
        PlayerPrefs.SetFloat("Volume_Sfx", sldrSfxV.value);
        PlayerPrefs.SetFloat("Volume_Music", sldrMusicV.value);
        PlayerPrefs.SetFloat("Volume_Ui", sldrUiV.value);

        //Full screen control
        if (tglFullscreen.isOn)
        {
            PlayerPrefs.SetInt("Fullscreen", 1);
        }
        else
        {
            PlayerPrefs.SetInt("Fullscreen", 0);
        }
    }

    private void LoadPlayerPrefsSettingsValues()
    {
        /*//Resolutions
        resolutionDropdown.value = PlayerPrefs.GetInt("Resolution");
        resolutionDropdown.RefreshShownValue();
        SetResolution(resolutionDropdown.value);*/

        //Game Quality
        qualtyDropdown.value = PlayerPrefs.GetInt("GameQuality");
        SetQuality(qualtyDropdown.value);

        //All volume vals
        sldrMainV.value = PlayerPrefs.GetFloat("Volume_Main");
        SetMainVolume(sldrMainV.value);
        sldrSfxV.value = PlayerPrefs.GetFloat("Volume_Sfx");
        SetSfxMainVolume(sldrSfxV.value);
        sldrMusicV.value = PlayerPrefs.GetFloat("Volume_Music");
        SetMusicVolume(sldrMusicV.value);
        sldrUiV.value = PlayerPrefs.GetFloat("Volume_Ui");
        SetUiMainVolume(sldrUiV.value);

        //Full screen control
        int isFullScreen = PlayerPrefs.GetInt("Fullscreen");
        if (isFullScreen == 1)
        {
            tglFullscreen.isOn = true;
        }
        else
        {
            tglFullscreen.isOn = false;
        }
        SetFullScreen(tglFullscreen.isOn);
    }

    void GamePause(bool x)
    {
        if (SceneManager.GetActiveScene().name.CompareTo("MainMenu") != 0)
        {
            if (mainManager) mainManager.onGamePauseCallBack.Invoke(x);
        }
    }

    #region VolumeControls
    public void SetMainVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
    }

    public void SetSfxMainVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
    }

    public void SetUiMainVolume(float volume)
    {
        audioMixer.SetFloat("UiVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }
    #endregion

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        //Debug.Log(QualitySettings.GetQualityLevel());
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = _resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void LoadMainMenu()
    {
        canPause = true;
        GamePause(false);
        CloseSettingsMenu();
        canvasGroupFade.gameObject.SetActive(true);
        LeanTween.alphaCanvas(canvasGroupFade, 1, 2f).setOnComplete(() => { SceneManager.LoadScene("Main Menu"); });
    }

    public LTDescr OpenSettingsMenu()
    {
        settingsCanvas.gameObject.SetActive(true);
        LTDescr fadeTween = LeanTween.alphaCanvas(settingsCanvas, 1, 0.5f);
        return fadeTween;
    }

    public LTDescr CloseSettingsMenu()
    {
        LTDescr fadeTween = LeanTween.alphaCanvas(settingsCanvas, 0, 0.5f).setOnComplete(() => { settingsCanvas.gameObject.SetActive(false);});
        return fadeTween;
    }

    public void ApplicationExit()
    {
        Application.Quit();
    }

    public void ResetLevel()
    {
        CursorController.instance.ControlCursor(CursorController.CursorState.Locked);
        CloseSettingsMenu();
        GamePause(false);
        canvasGroupFade.gameObject.SetActive(true);
        LeanTween.alphaCanvas(canvasGroupFade, 1, 2f).setOnComplete(() => { SceneManager.LoadScene(SceneManager.GetActiveScene().name.ToString()); });
    }

    private void OnEnable()
    {
        inputActions.Land.Enable();
    }

    private void OnDisable()
    {
        inputActions.Land.Disable();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }
}

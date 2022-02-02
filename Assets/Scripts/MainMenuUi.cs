using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class MainMenuUi : MonoBehaviour
{
    [SerializeField] private CanvasGroup imgFadeIn;
    [SerializeField] private CanvasGroup settingsCanvas;
    [SerializeField] private string musicToPlay;

    private void Awake()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "Main Menu")
        {
            CursorController.instance.ControlCursor(CursorController.CursorState.Unlocked);
        }
    }

    private void Start()
    {
        CursorController.instance.ControlCursor(CursorController.CursorState.Unlocked);
        AudioManager.instance.PlaySounds(musicToPlay);
        LeanTween.alphaCanvas(imgFadeIn, 0, 3f).setOnComplete(() => { imgFadeIn.gameObject.SetActive(false); });
    }

    public void OpenSettingsMenu()
    {
        settingsCanvas.gameObject.SetActive(true);
        LeanTween.alphaCanvas(settingsCanvas, 1, 1.5f);
    }

    public void ExitGame()
    {
        AudioManager.instance.StopSounds(musicToPlay);
        Application.Quit();
    }

    public void LoadScene(string sceneName)
    {
        imgFadeIn.gameObject.SetActive(true);
        LeanTween.alphaCanvas(imgFadeIn, 1, 3f).setOnComplete(() => {
            AudioManager.instance.StopSounds(musicToPlay);
            SceneManager.LoadScene(sceneName);
        });
    }
}

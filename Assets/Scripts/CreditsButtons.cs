using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CreditsButtons : MonoBehaviour
{
    [SerializeField] private CanvasGroup imgFadeInCanvas;
    [SerializeField] private CanvasGroup creditsButtons;
    [SerializeField] private float delay;
    [SerializeField] private string musicToPlay;

    private void Start()
    {
        CursorController.instance.ControlCursor(CursorController.CursorState.Unlocked);
        AudioManager.instance.PlaySounds(musicToPlay);
        /*LeanTween.delayedCall(delay, () =>
        {
            creditsButtons.gameObject.SetActive(true);
            LeanTween.alphaCanvas(creditsButtons, 1, 0.5f);
        });*/
    }

    public void LoadNextScene(string nextSceneLoad)
    {
        imgFadeInCanvas.gameObject.SetActive(true);
        LeanTween.alphaCanvas(imgFadeInCanvas, 1, 2f).setOnComplete(() => {
            AudioManager.instance.PlaySounds(musicToPlay);
            SceneManager.LoadScene(nextSceneLoad);
        });
    }
}

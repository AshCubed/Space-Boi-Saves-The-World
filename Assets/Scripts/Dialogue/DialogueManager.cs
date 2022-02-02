using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private PlayableDirector _playableDirector;
    [SerializeField] private GameObject _dialogueCanvas;
    [SerializeField] private CanvasGroup imgFadeInCanvas;
    [SerializeField] private TextMeshProUGUI tmpDialogueText;
    [SerializeField] private List<DialogueData> dialogue;
    [SerializeField] private string sceneAudio;
    private int placeInDialogeList;
    private int placeInCurrentDialogueBlock;

    // Start is called before the first frame update
    void Start()
    {
        CursorController.instance.ControlCursor(CursorController.CursorState.Unlocked);
        AudioManager.instance.PlaySounds(sceneAudio);
        placeInDialogeList = 0;
        placeInCurrentDialogueBlock = 0;
    }

    public void PauseTimeline()
    {
        _playableDirector.Pause();
        _dialogueCanvas.SetActive(true);
        LoadNextDialoge();
    }

    public void ContinueTimeline()
    {
        _dialogueCanvas.SetActive(false);
        _playableDirector.Play();
    }

    public void LoadNewScene(string sceneName)
    {
        imgFadeInCanvas.gameObject.SetActive(true);
        LeanTween.alphaCanvas(imgFadeInCanvas, 1, 2f).setOnComplete(() => {
            AudioManager.instance.PlaySounds(sceneAudio);
            SceneManager.LoadScene(sceneName);
        });
    }

    public void LoadNextDialoge()
    {
        if (placeInCurrentDialogueBlock < dialogue[placeInDialogeList].conversationBlock.Count)
        {
            tmpDialogueText.text = dialogue[placeInDialogeList].conversationBlock[placeInCurrentDialogueBlock];
            placeInCurrentDialogueBlock++;
        }
        else
        {
            placeInCurrentDialogueBlock = 0;
            placeInDialogeList++;
            ContinueTimeline();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    [SerializeField] private string mainPostProcessingTag;

    //A bool which controls if the player can move their camera
    //and if they can phyiscally move
    public bool canPlayerMove { get; set; }

    public bool isGamePaused { get; set; }

    public bool isGameOver { get; set; }

    public enum GameOverState {TIMER, STABILITY };

    public delegate void OnGameOver(GameOverState gameOverState);
    public OnGameOver onGameOverCallBack;

    public delegate void OnGamePause(bool pauseStatus);
    public OnGamePause onGamePauseCallBack;

    void Awake()
    {
        canPlayerMove = true;
        isGamePaused = false;
        isGameOver = false;
    }

    private void Start()
    {
        onGameOverCallBack += GameOver;
        onGamePauseCallBack += GamePause;
    }

    private void OnDestroy()
    {
        onGameOverCallBack -= GameOver;
        onGamePauseCallBack -= GamePause;
    }

    void GameOver(GameOverState gameOverState)
    {
        canPlayerMove = false;
        isGameOver = true;
    }

    void GamePause(bool pauseStatus)
    {
        canPlayerMove = !pauseStatus;
        isGamePaused = pauseStatus;
    }

    public string GetMainPostProcessingTag()
    {
        return mainPostProcessingTag;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private MainManager mainManager;
    [SerializeField] private float startingTime = 10f;
    private float _currentTime = 0f;
    private bool _isCountingDown;

    [SerializeField] TextMeshProUGUI countdownText;

    void Start()
    {
        _isCountingDown = true;
        _currentTime = startingTime;
        mainManager.onGameOverCallBack += OnGameOver;
        mainManager.onGamePauseCallBack += OnGamePaused;
    }

    private void OnDestroy()
    {
        mainManager.onGameOverCallBack -= OnGameOver;
        mainManager.onGamePauseCallBack -= OnGamePaused;
    }

    void Update()
    {
        if (_isCountingDown)
        {
            _currentTime -= 1 * Time.deltaTime;
            DisplayTime(_currentTime);

            if (_currentTime <= 0)
            {
                _currentTime = 0;
                _isCountingDown = false;
                countdownText.color = Color.green;
            }
        }
    }

    private void DisplayTime(float timeToDisplay)
    {
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
            mainManager.onGameOverCallBack.Invoke(MainManager.GameOverState.TIMER);
        }

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public string GetCurrentTime()
    {
        float minutes = Mathf.FloorToInt(_currentTime / 60);
        float seconds = Mathf.FloorToInt(_currentTime % 60);

        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void OnGameOver(MainManager.GameOverState gameOverState)
    {
        _isCountingDown = false;
    }

    private void OnGamePaused(bool isPaused)
    {
        _isCountingDown = !isPaused;
    }
}

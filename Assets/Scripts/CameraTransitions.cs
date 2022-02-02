using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraTransitions : MonoBehaviour
{
    #region Singleton

    //basic singleton instance
    public static CameraTransitions instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Camera Transitions found!");
            return;
        }

        instance = this;
    }
    //basic singleton instance

    #endregion

    private GameObject mainCineCam;
    private GameObject _currentActiveCineCam;
    private CinemachineBrain _cinemachineBrain;

    private void Start()
    {
        mainCineCam = GameObject.FindGameObjectWithTag("MainCineCam");
        if (!mainCineCam)
        {
            Debug.Log("NO MAIN_CINE_CAM IN CameraTransitions", gameObject);
        }
        _cinemachineBrain = FindObjectOfType<CinemachineBrain>();
    }

    public GameObject ReturnCurrentActiveCam()
    {
        return _currentActiveCineCam;
    }

    public void SetNewCam(GameObject newCam)
    {
/*        MainManager.instance.GetPlayer().GetComponentInChildren<CinemachineInputProvider>().enabled = false;
        if (MainManager.instance.GetPlayer())
        {
            MainManager.instance.canPlayerMove = false;
        }
        else
        {
            Debug.Log("NO PLAYER IN MAIN MANAGER");
        }*/
        if (_currentActiveCineCam) _currentActiveCineCam.SetActive(false);
        mainCineCam.SetActive(false);
        newCam.SetActive(true);
        _currentActiveCineCam = newCam;
    }

    public void SetToMainCam()
    {
        if (_currentActiveCineCam != mainCineCam)
        {
            if (_currentActiveCineCam != null)
            {
                _currentActiveCineCam.SetActive(false);
                mainCineCam.SetActive(true);
/*                MainManager.instance.GetPlayer().GetComponentInChildren<CinemachineInputProvider>().enabled = true;
                if (MainManager.instance.GetPlayer())
                {
                    MainManager.instance.canPlayerMove = true;
                }
                else
                {
                    Debug.Log("NO PLAYER IN MAIN MANAGER");
                }*/
                _currentActiveCineCam = null;
            }
            else
            {
                if (mainCineCam) mainCineCam.SetActive(true);
            }
        }
    }
}

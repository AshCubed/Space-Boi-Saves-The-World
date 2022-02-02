using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    #region Singleton

    //basic singleton instance
    public static CursorController instance;

    void Awake()
    {
        inputActions = new PlayerActions();

        if (instance != null)
        {
            Debug.LogWarning("More than one instance of CursorController found!");
            return;
        }

        instance = this;
    }
    //basic singleton instance

    #endregion

    public enum CursorState { Locked, Unlocked };
    private CursorState cursorState;
    private PlayerActions inputActions;

    // Start is called before the first frame update
    void Start()
    {
/*        cursorState = CursorController.CursorState.Locked;
        ControlCursor(cursorState);*/
        /*inputActions.Land.CursorControl.performed += ctx =>
        {
            if (cursorState == CursorState.Locked)
            {
                cursorState = CursorState.Unlocked;
                ControlCursor(cursorState);
            }
            else
            {
                cursorState = CursorState.Locked;
                ControlCursor(cursorState);
            }
        };*/
    }

    // Update is called once per frame
    void Update()
    {
        /*ControlCursor(cursorState);
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (cursorState == CursorState.Locked)
            {
                cursorState = CursorState.Unlocked;
                ControlCursor(cursorState);
            }
            else
            {
                cursorState = CursorState.Locked;
                ControlCursor(cursorState);
            }
        }*/
    }

    public void ControlCursor(CursorState state)
    {
        cursorState = state;
        switch (cursorState)
        {
            case CursorState.Locked:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
            case CursorState.Unlocked:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            default:
                break;
        }
    }

    private void OnEnable()
    {
        inputActions.Land.Enable();
    }

    private void OnDisable()
    {
        inputActions.Land.Disable();
    }
}

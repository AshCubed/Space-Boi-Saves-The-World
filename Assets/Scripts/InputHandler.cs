using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [Header("New Input System Vars")]
    [SerializeField] private PlayerInput _playerInput;
    private PlayerActions _inputActions;
    private string _currentControlScheme;

    bool _canPlayerMove;

    public Vector2 _currentMovement { get; private set;}
    public Vector2 _currentMousePos { get; private set; }
    public bool _pickUpItem { get; private set; }

    private void Awake()
    {
        _canPlayerMove = true;
        _inputActions = new PlayerActions();
    }

    // Start is called before the first frame update
    void Start()
    {
        _currentControlScheme = _playerInput.currentControlScheme;

        //Movement XY Event
        _inputActions.Land.Movement.performed += ctx => { if (_canPlayerMove) _currentMovement = ctx.ReadValue<Vector2>(); };
        _inputActions.Land.Movement.canceled += ctx => { _currentMovement = Vector2.zero; };


        //Movement XY Event
        _inputActions.Land.CameraMovement.performed += ctx => { if (_canPlayerMove) _currentMousePos = ctx.ReadValue<Vector2>(); };
        _inputActions.Land.CameraMovement.canceled += ctx => { _currentMousePos = Vector2.zero; };

        //Pick Up Item
        _inputActions.Land.PickUpItem.started += ctx => { if (_canPlayerMove) _pickUpItem = true; };
        _inputActions.Land.PickUpItem.canceled += ctx => { if (_canPlayerMove) _pickUpItem = false; };

        //Cursor Control
        //Handled in Cursor Controller script

        //Pause Control
        //Handled in Settings Manager
    }

    public void SetPlayerCanMove(bool canMove)
    {
        _canPlayerMove = canMove;
    }

    public bool GetPlayerCanMove()
    {
        return _canPlayerMove;
    }

    private void OnEnable()
    {
        _inputActions.Land.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Land.Disable();
    }
}

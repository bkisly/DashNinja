using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Direction
{
    Up,
    Down,
    Left,
    Right,
}

public delegate void InputMoveEventHandler(Direction direction);

[DefaultExecutionOrder(-1)]
public class InputManager : Singleton<InputManager>
{
    private PlayerControls _playerControls;
    public event InputMoveEventHandler KeyPressed;

    public delegate void TouchEventHandler(Vector2 position, float time);
    public event TouchEventHandler TouchStarted;
    public event TouchEventHandler TouchEnded;

    private void Awake()
    {
        _playerControls = new();
    }

    private void Start()
    {
        _playerControls.Keyboard.Up.started += _ => OnKeyPressed(Direction.Up);
        _playerControls.Keyboard.Down.started += _ => OnKeyPressed(Direction.Down);
        _playerControls.Keyboard.Left.started += _ => OnKeyPressed(Direction.Left);
        _playerControls.Keyboard.Right.started += _ => OnKeyPressed(Direction.Right);

        _playerControls.Touch.PrimaryContact.started += PrimaryContact_started;
        _playerControls.Touch.PrimaryContact.canceled += PrimaryContact_canceled;
    }

    private void PrimaryContact_started(InputAction.CallbackContext ctx)
    {
        Vector3 touchPos = ScreenCoordinatesToWorld(_playerControls.Touch.PrimaryPosition.ReadValue<Vector2>());
        OnTouchStarted(touchPos, (float)ctx.time);
    }

    private void PrimaryContact_canceled(InputAction.CallbackContext ctx)
    {
        Vector3 touchPos = ScreenCoordinatesToWorld(_playerControls.Touch.PrimaryPosition.ReadValue<Vector2>());
        OnTouchEnded(touchPos, (float)ctx.time);
    }

    private void OnEnable()
    {
        _playerControls.Enable();
    }

    private void OnDisable()
    {
        _playerControls.Disable();
    }

    public static Vector3 ScreenCoordinatesToWorld(Vector2 screenCoordinates)
        => Camera.main.ScreenToWorldPoint(new(screenCoordinates.x, screenCoordinates.y, Camera.main.nearClipPlane));

    private void OnKeyPressed(Direction direction) => KeyPressed?.Invoke(direction);
    private void OnTouchStarted(Vector2 position, float time) => TouchStarted?.Invoke(position, time);
    private void OnTouchEnded(Vector2 position, float time) => TouchEnded?.Invoke(position, time);
}

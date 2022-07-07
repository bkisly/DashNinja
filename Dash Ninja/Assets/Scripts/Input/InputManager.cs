using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Up,
    Down,
    Left,
    Right,
}

public class InputManager : Singleton<InputManager>
{
    private PlayerControls _playerControls;

    public delegate void KeyPressedEventHandler(Direction direction);
    public event KeyPressedEventHandler KeyPressed;

    private void Awake()
    {
        _playerControls = new();
    }

    private void Start()
    {
        _playerControls.KeyboardMovement.Up.started += _ => OnKeyPressed(Direction.Up);
        _playerControls.KeyboardMovement.Down.started += _ => OnKeyPressed(Direction.Down);
        _playerControls.KeyboardMovement.Left.started += _ => OnKeyPressed(Direction.Left);
        _playerControls.KeyboardMovement.Right.started += _ => OnKeyPressed(Direction.Right);
    }

    private void OnEnable()
    {
        _playerControls.Enable();
    }

    private void OnDisable()
    {
        _playerControls.Disable();
    }

    private void OnKeyPressed(Direction direction) => KeyPressed?.Invoke(direction);
}

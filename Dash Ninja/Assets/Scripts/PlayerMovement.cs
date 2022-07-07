using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveTime = .1f;

    private float _currentMoveTime = 0f;
    private Vector3 _newPosition = new();
    private bool _isMoving = false;

    private void Start()
    {
        InputManager.Instance.KeyPressed += InputManager_KeyPressed;
    }

    private void Update()
    {
        if(_isMoving)
        {
            _currentMoveTime += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, _newPosition, _currentMoveTime / moveTime);

            if(_currentMoveTime >= moveTime)
            {
                _currentMoveTime = 0f;
                _isMoving = false;
            }
        }
    }

    private void InputManager_KeyPressed(Direction direction)
    {
        if (_currentMoveTime == 0f)
        {
            switch (direction)
            {
                case Direction.Up:
                    _newPosition = transform.position + Vector3.forward;
                    break;
                case Direction.Down:
                    _newPosition = transform.position + Vector3.back;
                    break;
                case Direction.Left:
                    _newPosition = transform.position + Vector3.left;
                    break;
                case Direction.Right:
                    _newPosition = transform.position + Vector3.right;
                    break;
            }

            _isMoving = true;
        }
    }
}

using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveTime = .1f;
    [SerializeField] private FieldDetector fieldDetector;

    public event EventHandler MovementFinished;

    private float _currentMoveTime = 0f;
    private Vector3 _newPosition = new();
    private bool _isMoving = false;

    public bool IsMoving => _isMoving;

    private void Start()
    {
        InputManager.Instance.KeyPressed += InputMoveDetected;
        SwipeManager.Instance.Swiped += InputMoveDetected;
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
                OnMovementFinished();
            }
        }
    }

    private void OnDestroy()
    {
        InputManager.Instance.KeyPressed -= InputMoveDetected;
        SwipeManager.Instance.Swiped -= InputMoveDetected;
    }

    private void InputMoveDetected(Direction direction)
    {
        if (_currentMoveTime == 0f && fieldDetector.CurrentFieldType != FieldType.None)
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

    private void OnMovementFinished() => MovementFinished?.Invoke(this, new());
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeManager : Singleton<SwipeManager>
{
    [SerializeField, Range(.3f, 1.5f)] private float maxSwipeTime = .5f;
    [SerializeField, Range(.01f, .5f)] private float minSwipeDistance = .02f;
    [SerializeField, Range(.5f, .95f)] private float swipeDirectionTolerance = .8f;

    private float _touchStartTime, _touchEndTime = 0f;
    private Vector2 _touchStartPos, _touchEndPos = new();

    public event InputMoveEventHandler Swiped;

    private void Awake()
    {
        InputManager.Instance.TouchStarted += InputManager_TouchStarted;
        InputManager.Instance.TouchEnded += InputManager_TouchEnded;
    }

    private void InputManager_TouchStarted(Vector2 position, float time)
    {
        _touchStartPos = position;
        _touchStartTime = time;
    }

    private void InputManager_TouchEnded(Vector2 position, float time)
    {
        _touchEndPos = position;
        _touchEndTime = time;
        DetectSwipe();
    }

    private void DetectSwipe()
    {
        if(Vector2.Distance(_touchStartPos, _touchEndPos) >= minSwipeDistance && _touchEndTime - _touchStartTime <= maxSwipeTime)
        {
            Vector2 swipeDirection = (_touchEndPos - _touchStartPos).normalized;

            if (Vector2.Dot(swipeDirection, Vector2.left) >= swipeDirectionTolerance) OnSwiped(Direction.Left);
            else if (Vector2.Dot(swipeDirection, Vector2.up) >= swipeDirectionTolerance) OnSwiped(Direction.Up);
            else if (Vector2.Dot(swipeDirection, Vector2.right) >= swipeDirectionTolerance) OnSwiped(Direction.Right);
            else if (Vector2.Dot(swipeDirection, Vector2.down) >= swipeDirectionTolerance) OnSwiped(Direction.Down);
        }
    }

    private void OnSwiped(Direction direction) => Swiped?.Invoke(direction);
}

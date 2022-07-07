using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeManager : Singleton<SwipeManager>
{
    [SerializeField, Range(.3f, 1.5f)] private float maxSwipeTime = .5f;
    [SerializeField, Range(.01f, .5f)] private float minSwipeDistance = .02f;

    private float _touchStartTime, _touchEndTime;
    private Vector2 _touchStartPos, _touchEndPos;

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
        DetectTouch();
    }

    private void DetectTouch()
    {
        if(Vector2.Distance(_touchStartPos, _touchEndPos) >= minSwipeDistance && _touchEndTime - _touchStartTime <= maxSwipeTime)
        {
            Debug.Log("Swipe!");
        }
    }
}

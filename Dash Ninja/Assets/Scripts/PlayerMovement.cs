using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private void Start()
    {
        InputManager.Instance.KeyPressed += InputManager_KeyPressed;
    }

    private void InputManager_KeyPressed(Direction direction)
    {
        Debug.Log(direction.ToString());
    }
}

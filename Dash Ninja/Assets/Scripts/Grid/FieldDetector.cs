using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FieldType
{
    Normal,
    Start,
    Finish,
    Dangerous,
    None
}

public class FieldDetector : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;

    public delegate void FieldChangedEventHandler(FieldType fieldType);
    public event FieldChangedEventHandler FieldChanged;

    private FieldType _currentFieldType = FieldType.None;

    private void Start()
    {
        playerMovement.MovementFinished += (_, _) => OnFieldChanged(_currentFieldType);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "NormalField":
                _currentFieldType = FieldType.Normal;
                break;
            case "StartField":
                _currentFieldType = FieldType.Start;
                break;
            case "FinishField":
                _currentFieldType = FieldType.Finish;
                break;
            case "DangerousField":
                _currentFieldType = FieldType.Dangerous;
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _currentFieldType = FieldType.None;
    }

    private void OnFieldChanged(FieldType fieldType)
    {
        // Debug.Log($"Stepping on {fieldType}");
        FieldChanged?.Invoke(fieldType);
    }
}

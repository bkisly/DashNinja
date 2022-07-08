using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraConfig : MonoBehaviour
{
    [SerializeField] private CinemachineFreeLook cameraComponent;

    private void Start()
    {
        GameManager.Instance.PlayerSpawned += GameManager_PlayerSpawned;
    }

    private void GameManager_PlayerSpawned(GameObject player)
    {
        cameraComponent.Follow = player.transform;
        cameraComponent.LookAt = player.transform;
    }
}

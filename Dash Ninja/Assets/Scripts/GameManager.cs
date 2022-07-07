using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    #region Adjustable fields

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private float playerSpawnOffset = 1f;

    [Range(0f, 5f)]
    [SerializeField] 
    private float playerSpawnDelay = .5f;

    #endregion

    public GameObject Player { get; private set; }

    public delegate void PlayerSpawnedEventHandler(GameObject player);
    public PlayerSpawnedEventHandler PlayerSpawned;

    private FieldDetector _fieldDetector;

    private void Awake()
    {
        GridGenerator.Instance.GridGenerated += startPosition => StartCoroutine(SpawnPlayer(startPosition));
    }

    private IEnumerator SpawnPlayer(Vector3 startPosition)
    {
        startPosition.y = playerSpawnOffset;
        yield return new WaitForSeconds(playerSpawnDelay);
        Player = Instantiate(playerPrefab, startPosition, new());
        OnPlayerSpawned(Player);
    }

    private void OnPlayerSpawned(GameObject player)
    {
        if (_fieldDetector == null)
        {
            _fieldDetector = player.GetComponentInChildren<FieldDetector>();
            _fieldDetector.FieldChanged += fieldType => Debug.Log($"Stepping on {fieldType}");
        }
        
        PlayerSpawned?.Invoke(player);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public uint CurrentLevelId { get; private set; }
    public float PlayerSpawnOffset { get { return playerSpawnOffset; } }

    public delegate void PlayerSpawnedEventHandler(GameObject player);
    public event PlayerSpawnedEventHandler PlayerSpawned;
    public event EventHandler LevelLoaded;

    private FieldDetector _fieldDetector;

    private void Start()
    {
        InitializeGameplay();
    }

    private IEnumerator SpawnPlayer(Vector3 startPosition)
    {
        startPosition.y = playerSpawnOffset;
        yield return new WaitForSeconds(playerSpawnDelay);
        Player = Instantiate(playerPrefab, startPosition, new());
        OnPlayerSpawned(Player);
    }

    private void InitializeGameplay()
    {
        DontDestroyOnLoad(gameObject);
        CurrentLevelId++;
        GridGenerator.Instance.GridGenerated += position => StartCoroutine(SpawnPlayer(position));
        SceneManager.LoadScene(1);
        OnLevelLoaded();
    }

    private void NextLevel()
    {
        CurrentLevelId++;
        SceneManager.LoadScene(1);
        OnLevelLoaded();
    }

    public void Restart()
    {
        CurrentLevelId = 0;
        PlayerStats.Instance.ResetStats();
        GridGenerator.Instance.ResetGrid();
        NextLevel();
    }

    private void OnPlayerSpawned(GameObject player)
    {
        if (_fieldDetector == null)
        {
            _fieldDetector = player.GetComponentInChildren<FieldDetector>();
            _fieldDetector.FieldChanged += FieldDetector_FieldChanged;
        }
        
        PlayerSpawned?.Invoke(player);
    }

    private void FieldDetector_FieldChanged(FieldType fieldType)
    {
        if(fieldType == FieldType.Finish)
        {
            Debug.Log("Level complete.");
            NextLevel();
        }
    }

    private void OnLevelLoaded() => LevelLoaded?.Invoke(this, new());
}

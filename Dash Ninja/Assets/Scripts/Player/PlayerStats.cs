using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Singleton<PlayerStats>
{
    #region Adjustable fields

    [Header("Time points")]

    [SerializeField, Range(5f, 120f)] private float maxTimePoints = 30f;
    [SerializeField, Range(0f, 120f)] private float timePointsPerSecond = 1f;
    [SerializeField, Range(.01f, 1f)] private float timePointsIncrement = .1f;
    [SerializeField, Range(0f, 50f)] private float pointsToAddWhenFinish = 5f;

    [Header("Player settings")]

    [SerializeField, Range(1, 20)] private uint maxLives = 3;
    [SerializeField, Range(.1f, 50f)] private float damage = 5f;
    [SerializeField, Range(0f, 1f)] private float respawnDelay = .3f;

    #endregion

    private GameManager _gameManager;
    private FieldDetector _fieldDetector;
    private bool _playerDead = false;

    [SerializeField] private float _timePoints;
    public float TimePoints { get => _timePoints; }
    public uint Lives { get; private set; }
    public float Score { get; private set; }

    public event EventHandler PlayerDied;

    private void Awake()
    {
        _gameManager = GameManager.Instance;
        _gameManager.PlayerSpawned += GameManager_PlayerSpawned;
        _gameManager.LevelLoaded += (_, _) => { if (_gameManager.CurrentLevelId > 1) timePointsPerSecond += timePointsIncrement; };
    }

    private void Start()
    {
        _timePoints = maxTimePoints;
        Lives = maxLives;
    }

    private void Update()
    {
        if (!_playerDead)
        {
            _timePoints -= timePointsPerSecond * Time.deltaTime;
            if (_timePoints <= 0) OnPlayerDied();
        }
    }
    private void GameManager_PlayerSpawned(GameObject player)
    {
        _fieldDetector = player.GetComponentInChildren<FieldDetector>();
        _fieldDetector.FieldChanged += FieldDetector_FieldChanged;
    }

    private void FieldDetector_FieldChanged(FieldType fieldType)
    {
        switch (fieldType)
        {
            case FieldType.Dangerous:
                DealDamage();
                break;
            case FieldType.None:
                StartCoroutine(RespawnPlayer());
                break;
            case FieldType.Finish:
                FinishLevel();
                break;
        }
    }

    private void DealDamage()
    {
        if (damage >= _timePoints)
        {
            _timePoints = 0;
            OnPlayerDied();
        }
        else _timePoints -= damage;
    }

    private IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(respawnDelay);
        Lives--;

        if (Lives > 0)
        {
            Vector3 playerSpawnPos = GridGenerator.Instance.StartPosition + new Vector3(0, _gameManager.PlayerSpawnOffset);
            _gameManager.Player.transform.position = playerSpawnPos;
        }
        else OnPlayerDied();
    }

    private void FinishLevel()
    {
        Score += _timePoints;
        _timePoints += pointsToAddWhenFinish;
        if(_timePoints > maxTimePoints) maxTimePoints = _timePoints;
    }

    private void OnPlayerDied()
    {
        _playerDead = true;
        Destroy(_gameManager.Player);
        PlayerDied?.Invoke(this, new());
    }
}

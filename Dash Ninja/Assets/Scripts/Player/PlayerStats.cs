using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Singleton<PlayerStats>
{
    #region Adjustable fields

    [Header("Time points")]

    [SerializeField, Range(5f, 120f), Tooltip("Base amount of time points with which the player starts the game.")] 
    private float maxTimePoints = 30f;
    [SerializeField, Range(0f, 120f), Tooltip("Determines how many time points are reduced by every second.")] 
    private float timePointsPerSecond = 1f;
    [SerializeField, Range(.01f, 1f), Tooltip("The value which timePointsPerSecond is increased by when the player finishes a level.")] 
    private float timePointsIncrement = .1f;
    [SerializeField, Range(0f, 50f), Tooltip("Amount of time points which are added when the player finishes a level.")] 
    private float pointsToAddWhenFinish = 5f;

    [Header("Player settings")]

    [SerializeField, Range(1, 20), Tooltip("Base amount of lives with which the player starts the game.")] 
    private uint maxLives = 3;
    [SerializeField, Range(.1f, 50f), Tooltip("Amount of time points being reduced with every step on a dangerous field.")] 
    private float damage = 5f;
    [SerializeField, Range(0f, 1f), Tooltip("Time in seconds before the player gets respawned after falling off the grid.")] 
    private float respawnDelay = .3f;

    #endregion

    private GameManager _gameManager;
    private FieldDetector _fieldDetector;
    private bool _dead = false;
    private bool _invulnerable = false;
    [SerializeField] private float _invulnerabilityTime = 0f;

    [SerializeField] private float _timePoints;
    public float TimePoints => _timePoints;
    public uint Lives { get; private set; }
    public uint MaxLives => maxLives;
    public float Score { get; private set; }

    public delegate void InvulnerabilityChangedEventHandler(bool invulnerable);
    public event InvulnerabilityChangedEventHandler InvulnerabilityChanged;
    public event EventHandler PlayerDied;
    public event StatsChangedEventHandler StatsChanged;

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
        OnStatsChanged(new() { MaxTimePoints = maxTimePoints, Lives = Lives, MaxLives = maxLives});
    }

    private void Update()
    {
        if (!_dead)
        {
            _timePoints -= timePointsPerSecond * Time.deltaTime;
            if (_timePoints <= 0) OnPlayerDied();
        }

        if(_invulnerable)
        {
            _invulnerabilityTime -= Time.deltaTime;
            if (_invulnerabilityTime <= 0)
            {
                _invulnerable = false;
                OnInvulnerabilityChanged(_invulnerable);
            }
        }
        else
        {
            if(_gameManager.Player != null)
                if (_fieldDetector.CurrentFieldType == FieldType.Dangerous && !_gameManager.Player.GetComponent<PlayerMovement>().IsMoving)
                    DealDamage();
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

    public void AddTimePoints(float amount)
    {
        maxTimePoints = Mathf.Max(maxTimePoints, _timePoints += amount);
        OnStatsChanged(new() { MaxTimePoints = maxTimePoints });
    }

    public void SetInvulnerable(float time)
    {
        _invulnerabilityTime = Mathf.Min(_timePoints / timePointsPerSecond, time);
        _invulnerable = true;
        OnInvulnerabilityChanged(_invulnerable);
    }

    public void IncrementLives()
    {
        maxLives = (uint)Mathf.Max(maxLives, ++Lives);
        OnStatsChanged(new() { Lives = Lives, MaxLives = maxLives });
    }

    private void DealDamage()
    {
        if (!_invulnerable)
        {
            if (damage >= _timePoints)
            {
                _timePoints = 0;
                OnPlayerDied();
            }
            else
            {
                _timePoints -= damage;
                SetInvulnerable(1f);
            }
        }
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

        OnStatsChanged(new() { Lives = Lives });
    }

    private void FinishLevel()
    {
        Score += _timePoints;
        _timePoints += pointsToAddWhenFinish;
        if(_timePoints > maxTimePoints) maxTimePoints = _timePoints;

        OnStatsChanged(new() { Score = Score, MaxTimePoints = maxTimePoints });
    }

    public void ResetStats()
    {
        Score = 0;
        _timePoints = maxTimePoints = 30f;
        timePointsPerSecond = 1f;
        Lives = maxLives = 3;
        _dead = false;

        OnStatsChanged(new() { Score = Score, MaxLives = MaxLives, MaxTimePoints = maxTimePoints, Lives = Lives });
    }

    private void OnPlayerDied()
    {
        _dead = true;
        _timePoints = 0;
        Destroy(_gameManager.Player);
        PlayerDied?.Invoke(this, new());
    }

    private void OnStatsChanged(StatsChangedEventArgs e)
    {
        StatsChanged?.Invoke(e);
    }

    private void OnInvulnerabilityChanged(bool invulnerable)
    {
        InvulnerabilityChanged?.Invoke(invulnerable);
    }
}

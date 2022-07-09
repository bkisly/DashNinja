using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUIManager : MonoBehaviour
{
    [SerializeField] private Slider timePointsBar;
    [SerializeField] private Text timePointsText, livesText, scoreText;

    private PlayerStats _playerStats;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _playerStats = PlayerStats.Instance;
        _playerStats.StatsChanged += PlayerStats_StatsChanged;
    }

    private void LateUpdate()
    {
        timePointsBar.value = _playerStats.TimePoints;
        timePointsText.text = _playerStats.TimePoints.ToString("F3");
    }

    private void PlayerStats_StatsChanged(StatsChangedEventArgs e)
    {
        livesText.text = $"{e.Lives ?? _playerStats.Lives}/{e.MaxLives ?? _playerStats.MaxLives}";

        if (e.Score.HasValue) scoreText.text = e.Score.Value.ToString("F3");
        if (e.MaxTimePoints.HasValue) timePointsBar.maxValue = e.MaxTimePoints.Value;
    }
}
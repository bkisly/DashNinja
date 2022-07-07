using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    #region Adjustable fields

    [SerializeField] private GameObject player;
    [SerializeField] private uint currentLevelId = 1;
    [SerializeField] private float playerSpawnOffset = 1f;

    [Range(0f, 5f)]
    [SerializeField] 
    private float playerSpawnDelay = .5f;

    #endregion

    private void Awake()
    {
        GridGenerator.Instance.GridGenerated += startPosition => StartCoroutine(SpawnPlayer(startPosition));
    }

    private IEnumerator SpawnPlayer(Vector3 startPosition)
    {
        startPosition.y = playerSpawnOffset;
        yield return new WaitForSeconds(playerSpawnDelay);
        Instantiate(player, startPosition, new());
    }
}

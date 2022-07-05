using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [Header("Fields prefabs")]

    [SerializeField] private GameObject normalField;
    [SerializeField] private GameObject startField;
    [SerializeField] private GameObject finishField;
    [SerializeField] private GameObject dangerousField;

    [Header("Grid properties")]

    [SerializeField]
    [Range(2, 20)]
    private int gridSize = 8;

    private readonly System.Random _random = new();
    private readonly Dictionary<Vector2Int, GameObject> _coordinatesToFields = new();

    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
    }

    /// <summary>
    /// Generates a new level grid.
    /// </summary>
    private void GenerateGrid()
    {
        // 1. Find coordinates for start and end

        Vector2Int startCoordinates = new(_random.Next(gridSize), 0);
        Vector2Int finishCoordinates = new(_random.Next(gridSize), gridSize - 1);

        Debug.Log($"Start: {startCoordinates}, end: {finishCoordinates}");

        _coordinatesToFields.Add(startCoordinates, startField);
        _coordinatesToFields.Add(finishCoordinates, finishField);

        // 2. Find random route from start to end

        var route = FindRandomRoute(startCoordinates, finishCoordinates);
        foreach (Vector2Int coordinate in route)
        {
            Debug.Log(coordinate.ToString());
            _coordinatesToFields.Add(coordinate, normalField);
        }

        Debug.Log($"Route length: {route.Count()}");
    }

    /// <summary>
    /// Generates a route from start to end, which length is less than 50% of grid fields amount.
    /// </summary>
    /// <param name="start">Start point</param>
    /// <param name="end">End point</param>
    /// <returns>IEnumerable of fields cooridantes, that are members of the route.</returns>
    private IEnumerable<Vector2Int> FindRandomRoute(Vector2Int start, Vector2Int end)
    {
        HashSet<Vector2Int> result = new();
        Vector2Int currentCoordinate = start;

        while(currentCoordinate != end)
        {
            bool foundNext = false;

            while(!foundNext)
            {
                int x, y;

                // Choose the random direction, which doesn't go outside the grid
                do
                {
                    x = _random.Next(currentCoordinate.x == 0 ? 0 : -1, currentCoordinate.x == gridSize - 1 ? 1 : 2);
                    y = _random.Next(currentCoordinate.y == 0 ? 0 : -1, currentCoordinate.y == gridSize - 1 ? 1 : 2);
                }
                while ((x == 0 && y == 0) || (x != 0 && y != 0));

                currentCoordinate += new Vector2Int(x, y);

                // Check if the current coordinate doesn't reach start, add to results if it isn't already there
                if(currentCoordinate != start)
                {
                    if(currentCoordinate != end) result.Add(currentCoordinate);
                    foundNext = true;
                }
            }
        }

        // Check if the route length isn't too high, in this case start the process again
        if (result.Count >= gridSize * gridSize / 2) return FindRandomRoute(start, end);
        else return result;
    }
}

enum FieldsTypes
{
    Normal,
    Start,
    Finish,
    Dangerous
}

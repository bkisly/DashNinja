using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : Singleton<GridGenerator>
{
    #region Adjustable fields

    [Header("Fields prefabs")]

    [SerializeField] private GameObject normalField;
    [SerializeField] private GameObject startField;
    [SerializeField] private GameObject finishField;
    [SerializeField] private GameObject dangerousField;

    [Header("Items prefabs")]
    [SerializeField] private GameObject goldItem;
    [SerializeField] private GameObject vialItem;
    [SerializeField] private GameObject fulgentPearlItem;

    [Header("Grid properties")]

    [SerializeField, Range(2, 20), Tooltip("Base width and length of the grid.")]
    private int gridSize = 8;
    [SerializeField, Range(0, .5f), Tooltip("Delay in seconds between spawning the following grid blocks.")]
    private float spawnDelay = 0.025f;
    [SerializeField, Range(.5f, 3f)]
    private float itemSpawnOffset = 1f;

    #endregion

    public delegate void GridGeneratedEventHandler(Vector3 startPosition);
    public event GridGeneratedEventHandler GridGenerated;

    public Vector3 StartPosition { get; private set; }

    private readonly System.Random _random = new();
    private readonly Dictionary<Vector2Int, GameObject> _coordinatesToFields = new();
    private readonly Dictionary<Vector2Int, GameObject> _coordinatesToItems = new();

    private float _minAmountOfDangerous = .25f;
    private float _maxAmountOfDangerous = .3f;
    private const float _maxAmountOfItems = .05f;

    private void Awake()
    {
        GameManager.Instance.LevelLoaded += (_, _) => GenerateGrid();
    }

    /// <summary>
    /// Sets grid properties to its initial values.
    /// </summary>
    public void ResetGrid()
    {
        gridSize = 8;
        _minAmountOfDangerous = .25f;
        _maxAmountOfDangerous = .3f;
    }

    /// <summary>
    /// Generates a new level grid.
    /// </summary>
    private void GenerateGrid()
    {
        UpdateGridSettings();
        _coordinatesToFields.Clear();
        _coordinatesToItems.Clear();

        // 1. Find coordinates for start and end

        Vector2Int startCoordinates = new(_random.Next(gridSize), 0);
        Vector2Int finishCoordinates = new(_random.Next(gridSize), gridSize - 1);

        _coordinatesToFields.Add(startCoordinates, startField);
        _coordinatesToFields.Add(finishCoordinates, finishField);

        // 2. Find random route from start to end

        foreach (Vector2Int coordinate in FindRandomRoute(startCoordinates, finishCoordinates))
            _coordinatesToFields.Add(coordinate, normalField);

        // 3. Find dangerous fields coordinates

        var dangerousCoordinates = GetDangerousFieldsCoordinates((uint)UnityEngine.Random.Range(
            _minAmountOfDangerous * Mathf.Pow(gridSize, 2),
            _maxAmountOfDangerous * Mathf.Pow(gridSize, 2)));

        foreach (Vector2Int coordinate in dangerousCoordinates)
            _coordinatesToFields.Add(coordinate, dangerousField);

        // 4. Add normal fields in order to fill empty spaces

        for(int x = 0; x < gridSize; x++)
        {
            for(int y = 0; y < gridSize; y++)
            {
                if(!_coordinatesToFields.ContainsKey(new Vector2Int(x, y)))
                    _coordinatesToFields.Add(new Vector2Int(x, y), normalField);
            }
        }

        // 5. Generate items positions

        GenerateItemsPositions();

        // 6. Instatiate the grid

        StartPosition = GridCoordinatesToWorld(startCoordinates);
        OnGridGenerated(StartPosition);
        StartCoroutine(InstantiateFields());
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

    /// <summary>
    /// Generates the collection of random coordinates for dangerous fields.
    /// </summary>
    /// <param name="amount">Amount of dangerous fields</param>
    /// <returns>IEnumerable of random dangerous fields coordinates.</returns>
    private IEnumerable<Vector2Int> GetDangerousFieldsCoordinates(uint amount)
    {
        for(int i = 0; i < amount; i++)
        {
            Vector2Int coordinates = new();

            do
            {
                coordinates.x = _random.Next(gridSize);
                coordinates.y = _random.Next(gridSize);
            }
            while (_coordinatesToFields.ContainsKey(coordinates));

            yield return coordinates;
        }
    }

    /// <summary>
    /// Fills the local dictionary which maps grid coordinates to item objects to spawn
    /// </summary>
    private void GenerateItemsPositions()
    {
        int itemsAmount = _random.Next((int)(_maxAmountOfItems * Mathf.Pow(gridSize, 2)));
        IList<Vector2Int> normalFieldsCoordinates = (from Vector2Int coord in _coordinatesToFields.Keys
                                      where _coordinatesToFields[coord].CompareTag("NormalField")
                                      select coord).ToList();

        for (int i = 0; i < itemsAmount; i++)
        {
            // 1. Select item type to spawn

            double itemSpawnChance = _random.NextDouble();
            ItemType itemType = itemSpawnChance switch
            {
                (>= 0) and (< .8) => ItemType.Gold,
                (>= .8) and (< .95) => ItemType.Vial,
                _ => ItemType.FulgentPearl
            };

            // 2. Select item coordinates and remove them from the list

            Vector2Int randomCoordinate = normalFieldsCoordinates[_random.Next(normalFieldsCoordinates.Count)];
            normalFieldsCoordinates.Remove(randomCoordinate);

            // 3. Add item to the dictionary

            _coordinatesToItems.Add(randomCoordinate, itemType switch
            {
                ItemType.Gold => goldItem,
                ItemType.Vial => vialItem,
                _ => fulgentPearlItem
            });
        }
    }

    /// <summary>
    /// Spawns the fields objects on the proper positions.
    /// </summary>
    private IEnumerator InstantiateFields()
    {
        yield return new WaitForSeconds(.01f);

        var sortedCoordinates = from Vector2Int coordinate in _coordinatesToFields.Keys
                                orderby coordinate.y, coordinate.x
                                select coordinate;

        foreach (Vector2Int coordinates in sortedCoordinates)
        {
            Instantiate(
                _coordinatesToFields[coordinates], 
                GridCoordinatesToWorld(coordinates), 
                new Quaternion(), 
                GameObject.FindGameObjectsWithTag("GridHolder").FirstOrDefault().transform
            );

            if (_coordinatesToItems.ContainsKey(coordinates))
            {
                print("lol");
                Instantiate(
                    _coordinatesToItems[coordinates],
                    GridCoordinatesToWorld(coordinates) + new Vector3(0, itemSpawnOffset),
                    new Quaternion(),
                    GameObject.FindGameObjectsWithTag("ItemsHolder").FirstOrDefault().transform
                );
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    /// <summary>
    /// Performs grid properties increment appropriate for finishing the level.
    /// </summary>
    private void UpdateGridSettings()
    {
        if(GameManager.Instance.CurrentLevelId % 5 == 0) gridSize += 2;

        if(_maxAmountOfDangerous < .5f)
        {
            _minAmountOfDangerous += .02f;
            _maxAmountOfDangerous += .02f;
        }
    }

    private void OnGridGenerated(Vector3 startPosition)
    {
        GridGenerated?.Invoke(startPosition);
    }
    private static Vector3 GridCoordinatesToWorld(Vector2Int coordinates) => new(coordinates.x, 0, coordinates.y);
}

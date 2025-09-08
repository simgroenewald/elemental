using GLTFast.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public class DungeonRoom : MonoBehaviour 
{

    public Structure structure = new Structure();
    public RoomType roomType;
    public ElementTheme theme;
    public Vector2 nodeGraphPosition;
    public GameObject roomObject;

    public DungeonRoom parent;
    public List<DungeonRoom> children = new();
    public Dictionary<Direction, DungeonRoom> connections = new(); // Direction FROM this room to another room

    public BoundsInt bounds;
    public BoundsInt outerBounds;
    public Dictionary<Direction, List<Vector2Int>> edgeLists = new();
    public List<HashSet<Vector2Int>> roomAreas;

    public List<BoundsInt> subRooms;
    public int subRoomMinWidth;
    public int subRoomMinHeight;
    public int[,] tiles;

    public List<Doorway> doorways = new List<Doorway>();
    public List<Doorway> exitDoorways = new List<Doorway>();
    public GameObject doorPrefab;

    public List<Enemy> enemies = new List<Enemy>();

    public bool isEntered = false;
    public bool isComplete = false;
    public bool previouslyEntered = false;
    private bool allEnemiesDefeated = false;

    public bool IsLeaf => children.Count == 0;
    public bool CanHaveMoreChildren => children.Count < 3;

    public DungeonRoom Initialise(RoomType type, Vector2 pos, GameObject roomObject)
    {
        roomType = type;
        theme = (ElementTheme)UnityEngine.Random.Range(0, 4);
        nodeGraphPosition = pos;
        this.roomObject = roomObject;
        parent = null;
        children = new();
        SetName();
        return this;
    }

    public void Update()
    {
        allEnemiesDefeated = true;
        if (isEntered && !isComplete)
        {
            foreach (var enemy in enemies)
            {
                if (enemy && !enemy.characterState.isDead)
                {
                    allEnemiesDefeated = false;
                    break;
                }
            }
            if (allEnemiesDefeated)
            {
                CompleteRoom();
            }
        }
    }

    public void UpdateObjectName()
    {
        roomObject.name = $"DungeonRoom_{roomType}_{theme}";
    }

    public int GetDoorDepth(DoorType doorType)
    {
        if (theme == ElementTheme.Air && doorType == DoorType.FrontDoor)
        {
            return 2;
        } else
        {
            return 1;
        }
    }

    public void SpawnRoomEnemies(int enemyCount)
    {
        SimpleEnemyInitialiser enemyInitialiser = GetComponent<SimpleEnemyInitialiser>();
        Transform roomTransform = GetComponent<Transform>();
        List<GameObject> enemyPrefabs = enemyInitialiser.GetEnemyPrefabs(theme);

        foreach (var enemyPrefab in enemyPrefabs)
        {
            for (int i = 0; i < enemyCount; i++)
            {

                Vector2Int spawnPosition2D = GetValidSpawnPosition();

                // Convert tile position to world position
                Vector3 worldPosition = this.structure.tilemapLayers.grid.CellToWorld((Vector3Int)spawnPosition2D);
                // Center the position in the tile
                worldPosition += this.structure.tilemapLayers.grid.cellSize * 0.16f;

                // Instantiate enemy
                GameObject enemyGO = Instantiate(enemyPrefab, worldPosition, Quaternion.identity, roomTransform);

                enemyGO.name = $"Enemy_{i + 1}";

                // Ensure proper 2D rotation (no X or Y rotation)
                enemyGO.transform.rotation = Quaternion.identity;

                Enemy enemy = enemyGO.GetComponent<Enemy>();
                enemy.SetRoom(this);
                enemies.Add(enemy);
            }
        }

        if (roomType == RoomType.MiniBoss)
        {
            GameObject miniBossPrefab = enemyInitialiser.GetMiniBossPrefab(theme);
            Vector2Int spawnPosition2D = GetValidSpawnPosition();

            // Convert tile position to world position
            Vector3 worldPosition = this.structure.tilemapLayers.grid.CellToWorld((Vector3Int)spawnPosition2D);
            // Center the position in the tile
            worldPosition += this.structure.tilemapLayers.grid.cellSize * 0.16f;

            // Instantiate enemy
            GameObject miniBossGO = Instantiate(miniBossPrefab, worldPosition, Quaternion.identity, roomTransform);

            miniBossGO.name = $"Miniboss";

            // Ensure proper 2D rotation (no X or Y rotation)
            miniBossGO.transform.rotation = Quaternion.identity;

            Enemy miniBoss = miniBossGO.GetComponent<Enemy>();
            miniBoss.SetRoom(this);
            enemies.Add(miniBoss);
        }
    }

/*     public void SpawnRoomItems(int itemCount)
    {
        SimpleItemInitialiser itemInitialiser = GetComponent<SimpleItemInitialiser>();
        Transform roomTransform = GetComponent<Transform>();
        List<GameObject> itemPrefabs = itemInitialiser.GetItemPrefabs();

        foreach (var itemPrefab in itemPrefabs)
        {
            for (int i = 0; i < itemCount; i++)
            {

                Vector2Int spawnPosition2D = GetValidSpawnPosition();

                // Convert tile position to world position
                Vector3 worldPosition = this.structure.tilemapLayers.grid.CellToWorld((Vector3Int)spawnPosition2D);
                // Center the position in the tile
                worldPosition += this.structure.tilemapLayers.grid.cellSize * 0.16f;

                // Instantiate enemy
                GameObject itemGO = Instantiate(itemPrefab, worldPosition, Quaternion.identity, roomTransform);

                itemGO.name = $"Item_{i + 1}";

                // Ensure proper 2D rotation (no X or Y rotation)
                itemGO.transform.rotation = Quaternion.identity;
            }
        }
    }*/


    public Vector2Int GetValidSpawnPosition()
    {
        return structure.floorPositions.ElementAt(UnityEngine.Random.Range(0, structure.floorPositions.Count));
    }

    private void SetName()
    {
        structure.name = roomType + " " + theme;
    }


    public void AddDoorway(Doorway doorway)
    {
        doorways.Add(doorway);
    }

    public void AddExitDoorway(Doorway doorway)
    {
        exitDoorways.Add(doorway);
    }

    public void EnterRoom()
    {
        isEntered = true;
    }

    public void ExitRoom()
    {
        if (isComplete)
        {
            isEntered = false;
            previouslyEntered = true;
        }
    }

    public void CompleteRoom()
    {
        isComplete = true;
        StaticEventHandler.CallRoomCompleteEvent(this);
    }

    public void DrawRoomTiles()
    {
        SetRoomDoorClosedTiles();

        Tilemap baseTilemap = structure.tilemapLayers.baseTilemap;
        Tilemap baseDecorTilemap = structure.tilemapLayers.baseDecorationTilemap;
        Tilemap frontTilemap = structure.tilemapLayers.frontTilemap;
        Tilemap frontDecorTilemap = structure.tilemapLayers.frontDecorationTilemap;
        Tilemap collisionTilemap = structure.tilemapLayers.collisionTilemap;

        foreach (var tile in structure.structureTiles)
        {
            baseTilemap.SetTile((Vector3Int)tile.position, tile.baseTile);
            baseDecorTilemap.SetTile((Vector3Int)tile.position, tile.baseDecorTile);
            frontTilemap.SetTile((Vector3Int)tile.position, tile.frontTile);
            frontDecorTilemap.SetTile((Vector3Int)tile.position, tile.frontDecorTile);
            collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
        }

    }

    private void SetRoomDoorOpenTiles()
    {
        foreach (var door in doorways)
        {
            door.SetOpenDoorTiles();
        }
    }



    private void SetRoomDoorClosedTiles()
    {
        foreach (var door in doorways)
        {
            foreach (var tile in door.structure.structureTiles)
            {
                StructureTile removedTile = structure.RemoveTileAtPosition(tile.position);
                door.AppendClosedDoorTiles(removedTile);
            }
        }
    }

    public void displayInfo()
    {
    Debug.Log("Hello");
        /*        Debug.Log($"--- Room Info ---");
                Debug.Log("Room Type: " + roomType);
                Debug.Log("Room Theme: " + theme);
                Debug.Log("Children: " + children.Count);
                Debug.Log("Dimensions: " + bounds.size.x + " x " + bounds.size.y);
                Debug.Log("Position: " + outerBounds.position.x + ", " + outerBounds.position.y);*/
    }
}
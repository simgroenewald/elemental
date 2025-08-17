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

    public bool IsLeaf => children.Count == 0;
    public bool CanHaveMoreChildren => children.Count < 3;

    public DungeonRoom Initialise(RoomType type, Vector2 pos)
    {
        roomType = type;
        theme = (ElementTheme)UnityEngine.Random.Range(0, 4);
        nodeGraphPosition = pos;
        parent = null;
        children = new();
        SetName();
        return this;
    }

    public void SpawnRoomEnemies(int enemyCount)
    {
        SimpleEnemyInitializer enemyInitializer = GetComponent<SimpleEnemyInitializer>();
        Transform roomTransform = GetComponent<Transform>();
        List<GameObject> enemyPrefabs = enemyInitializer.GetEnemyPrefabs();

        foreach (var enemyPrefab in enemyPrefabs)
        {
            for (int i = 0; i < enemyCount; i++)
            {

                Vector2Int spawnPosition2D = GetEnemyStartPosition();

                // Convert tile position to world position
                Vector3 worldPosition = this.structure.tilemapLayers.grid.CellToWorld((Vector3Int)spawnPosition2D);
                // Center the position in the tile
                worldPosition += this.structure.tilemapLayers.grid.cellSize * 0.16f;

                // Instantiate enemy
                GameObject enemyGO = Instantiate(enemyPrefab, worldPosition, Quaternion.identity, roomTransform);

                enemyGO.name = $"Enemy_{i + 1}";

                // Ensure proper 2D rotation (no X or Y rotation)
                enemyGO.transform.rotation = Quaternion.identity;

                Debug.Log($"Spawned enemy at floor position: {spawnPosition2D}");
            }
        }
    }


    public Vector2Int GetEnemyStartPosition()
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
    }

    public void DrawRoomTiles()
    {
        SetRoomDoorClosedTiles();

        Tilemap baseTilemap = structure.tilemapLayers.baseTilemap;
        Tilemap frontTilemap = structure.tilemapLayers.frontTilemap;
        Tilemap collisionTilemap = structure.tilemapLayers.collisionTilemap;

        foreach (var tile in structure.structureTiles)
        {
            baseTilemap.SetTile((Vector3Int)tile.position, tile.baseTile);
            frontTilemap.SetTile((Vector3Int)tile.position, tile.frontTile);
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
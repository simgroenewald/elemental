using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public class DungeonRoom : Structure 
{
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

    public bool isEntered = false;
    public bool isComplete = false;
    public bool previouslyEntered = false;

    public bool IsLeaf => children.Count == 0;
    public bool CanHaveMoreChildren => children.Count < 3;

    public DungeonRoom(RoomType type, Vector2 pos)
    {
        roomType = type;
        theme = (ElementTheme)UnityEngine.Random.Range(0, 4);
        nodeGraphPosition = pos;
        parent = null;
        children = new();
        SetName();
    }

    private void SetName()
    {
        name = roomType + " " + theme;
    }


    public void AddDoorway(Vector2Int midpoint, int width, ConnectorOrientation orientation)
    {
        Doorway doorway = new Doorway(midpoint, width, orientation);
        doorways.Add(doorway);
    }

    public void DrawRoomTiles()
    {
        Tilemap baseTilemap = structureTilemap.tilemapLayers.baseTilemap;
        Tilemap frontTilemap = structureTilemap.tilemapLayers.frontTilemap;
        Tilemap collisionTilemap = structureTilemap.tilemapLayers.collisionTilemap;

        foreach (var tile in structureTiles)
        {
            baseTilemap.SetTile((Vector3Int)tile.position, tile.baseTile);
            frontTilemap.SetTile((Vector3Int)tile.position, tile.frontTile);
            collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
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
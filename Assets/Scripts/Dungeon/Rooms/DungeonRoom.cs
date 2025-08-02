using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public GameObject doorPrefab;

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

    private void SetName()
    {
        structure.name = roomType + " " + theme;
    }


    public void AddDoorway(Doorway doorway)
    {
        doorways.Add(doorway);
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

    public void DrawOpenRoomDoorwayTiles()
    {
        foreach (var door in doorways)
        {
            door.DrawOpenDoorTiles(structure.tilemapLayers);
        }
    }

    public void DrawClosedRoomDoorwayTiles()
    {
        foreach (var door in doorways)
        {
            door.DrawClosedDoorTiles(structure.tilemapLayers);
        }
    }

    private void SetRoomDoorClosedTiles()
    {
        foreach (var door in doorways)
        {
            foreach (var tile in door.structureTiles)
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
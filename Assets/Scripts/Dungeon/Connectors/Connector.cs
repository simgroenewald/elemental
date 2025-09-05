using UnityEngine;
using System;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem.Controls;

public class Connector : MonoBehaviour
{
    public Structure structure = new Structure();

    public DungeonRoom parentRoom;
    public DungeonRoom childRoom;
    public Vector2Int start;
    public Vector2Int end;
    public ConnectorOrientation orientation;

    public Bridge bridgeMain;
    public Bridge bridgeStart;
    public Bridge bridgeEnd;
    public Platform platform;

    public bool isStraight;
    public bool isBossRoomConnector = false;

    public Connector Initialise(Vector2Int _start, Vector2Int _end, DungeonRoom _partentRoom, DungeonRoom _childRoom, ConnectorOrientation _orientation, bool _isBossRoomConnector)
    {
        start = _start;
        end = _end;
        parentRoom = _partentRoom;
        childRoom = _childRoom;
        orientation = _orientation;
        isBossRoomConnector = _isBossRoomConnector;

        if (orientation == ConnectorOrientation.Vertical)
        {
            if (start.x == end.x)
            {
                isStraight = true;
            }
            else
            {
                isStraight = false;
            }
        }
        if (orientation == ConnectorOrientation.Horizontal)
        {
            if (start.y == end.y)
            {
                isStraight = true;
            }
            else
            {
                isStraight = false;
            }
        }

        SetName();

        return this;
    }
    private void SetName()
    {
        if (isStraight)
        {
            name = "Wooden Bridge";
        }
        else
        {
            name = "Platform";

        }
    }


    public void DrawConnectorTiles()
    {
        Tilemap frontTilemap = structure.tilemapLayers.frontTilemap;
        Tilemap collisionTilemap = structure.tilemapLayers.collisionTilemap;
        Tilemap platformTilemap = structure.tilemapLayers.platformTilemap;
        Tilemap bridgeTilemap = structure.tilemapLayers.bridgeTilemap;


        if (isStraight)
        {
            foreach (var tile in bridgeMain.structure.structureTiles)
            {
                bridgeTilemap.SetTile((Vector3Int)tile.position, tile.baseTile);
                frontTilemap.SetTile((Vector3Int)tile.position, tile.frontTile);
                collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
            }
        }
        if (!isStraight)
        {
            foreach (var tile in platform.structure.structureTiles)
            {
                platformTilemap.SetTile((Vector3Int)tile.position, tile.baseTile);
                frontTilemap.SetTile((Vector3Int)tile.position, tile.frontTile);
                collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
            }
            foreach (var tile in bridgeStart.structure.structureTiles)
            {
                bridgeTilemap.SetTile((Vector3Int)tile.position, tile.baseTile);
                frontTilemap.SetTile((Vector3Int)tile.position, tile.frontTile);
                collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
            }
            foreach (var tile in bridgeEnd.structure.structureTiles)
            {
                bridgeTilemap.SetTile((Vector3Int)tile.position, tile.baseTile);
                frontTilemap.SetTile((Vector3Int)tile.position, tile.frontTile);
                collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
            }
        }
        
    }

    public void GenerateAllPartStructureTiles()
    {
        if (isStraight)
        {
            bridgeMain.structure.GenerateStructureTiles(bridgeMain.bounds, bridgeMain.structure.floorPositions, false);
        }
        else
        {
            bridgeStart.structure.GenerateStructureTiles(bridgeStart.bounds, bridgeStart.structure.floorPositions, false);
            bridgeEnd.structure.GenerateStructureTiles(bridgeEnd.bounds, bridgeEnd.structure.floorPositions, false);
            platform.structure.GenerateStructureTiles(platform.bounds, platform.structure.floorPositions, true, 2);
        }
    }
}
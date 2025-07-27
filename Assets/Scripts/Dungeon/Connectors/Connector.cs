using UnityEngine;
using System;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem.Controls;

public class Connector : Structure
{
    public DungeonRoom roomA;
    public DungeonRoom roomB;
    public Vector2Int start;
    public Vector2Int end;
    public ConnectorOrientation orientation;
    public Boolean isStraight;

    public Bridge bridgeMain;
    public Bridge bridgeStart;
    public Bridge bridgeEnd;
    public Platform platform;

    public Connector(Vector2Int _start, Vector2Int _end, ConnectorOrientation _orientation)
    {
        start = _start;
        end = _end;
        orientation = _orientation;

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

        if (isStraight)
        {
            if (orientation == ConnectorOrientation.Vertical)
            {
                bridgeMain = new Bridge(ConnectorOrientation.Vertical);
            }
            else
            {
                bridgeMain = new Bridge(ConnectorOrientation.Horizontal);

            }
        }
        else
        {
            if (orientation == ConnectorOrientation.Vertical)
            {
                bridgeStart = new Bridge(ConnectorOrientation.Vertical);
                bridgeEnd = new Bridge(ConnectorOrientation.Vertical);
                platform = new Platform(ConnectorOrientation.Horizontal);
            }
            else
            {
                bridgeStart = new Bridge(ConnectorOrientation.Horizontal);
                bridgeEnd = new Bridge(ConnectorOrientation.Horizontal);
                platform = new Platform(ConnectorOrientation.Vertical);
            }
        }

        SetName();
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
        Tilemap frontTilemap = structureTilemap.tilemapLayers.frontTilemap;
        Tilemap collisionTilemap = structureTilemap.tilemapLayers.collisionTilemap;
        Tilemap platformTilemap = structureTilemap.tilemapLayers.platformTilemap;
        Tilemap bridgeTilemap = structureTilemap.tilemapLayers.bridgeTilemap;


        if (isStraight)
        {
            foreach (var tile in bridgeMain.structureTiles)
            {
                bridgeTilemap.SetTile((Vector3Int)tile.position, tile.baseTile);
                frontTilemap.SetTile((Vector3Int)tile.position, tile.frontTile);
                collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
            }
        }
        if (!isStraight)
        {
            foreach (var tile in platform.structureTiles)
            {
                platformTilemap.SetTile((Vector3Int)tile.position, tile.baseTile);
                frontTilemap.SetTile((Vector3Int)tile.position, tile.frontTile);
                collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
            }
            foreach (var tile in bridgeStart.structureTiles)
            {
                bridgeTilemap.SetTile((Vector3Int)tile.position, tile.baseTile);
                frontTilemap.SetTile((Vector3Int)tile.position, tile.frontTile);
                collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
            }
            foreach (var tile in bridgeEnd.structureTiles)
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
            bridgeMain.GenerateStructureTiles(bridgeMain.bounds, bridgeMain.floorPositions, false);
        }
        else
        {
            bridgeStart.GenerateStructureTiles(bridgeStart.bounds, bridgeStart.floorPositions, false);
            bridgeEnd.GenerateStructureTiles(bridgeEnd.bounds, bridgeEnd.floorPositions, false);
            platform.GenerateStructureTiles(platform.bounds, platform.floorPositions, true, 2);
        }
    }
}
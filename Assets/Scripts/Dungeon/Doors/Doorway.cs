using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class Doorway : Structure
{
    WallType wallType;
    public HashSet<StructureTile> closedTiles = new HashSet<StructureTile>();
    public HashSet<StructureTile> openTiles = new HashSet<StructureTile>();

    public Doorway(Vector2Int midPositon, int width, WallType wallType)
    {
        floorPositions = new HashSet<Vector2Int>();
        int sideTileCount = width / 2;

        this.wallType = wallType;
        floorPositions.Add(midPositon);
        for (int i = 1; i < sideTileCount + 1; i++)
        {
            Vector2Int doorwayTileA = new Vector2Int();
            Vector2Int doorwayTileB = new Vector2Int();

            if (wallType == WallType.WallBack || wallType == WallType.WallFront)
            {
                doorwayTileA = new Vector2Int(midPositon.x - i, midPositon.y);
                doorwayTileB = new Vector2Int(midPositon.x + i, midPositon.y);
            }
            else
            {
                doorwayTileA = new Vector2Int(midPositon.x, midPositon.y - i);
                doorwayTileB = new Vector2Int(midPositon.x, midPositon.y + i);

            }
            floorPositions.Add(doorwayTileA);
            floorPositions.Add(doorwayTileB);
        }
    }

    public void GenerateStructureTiles()
    {

        // Determine min and max based on orientation
        Vector2Int min = new Vector2Int(int.MaxValue, int.MaxValue);
        Vector2Int max = new Vector2Int(int.MinValue, int.MinValue);

        foreach (var pos in floorPositions)
        {
            if (wallType == WallType.WallFront || wallType == WallType.WallBack)
            {
                if (pos.x < min.x) min = pos;
                if (pos.x > max.x) max = pos;
            }
            else
            {
                if (pos.y < min.y) min = pos;
                if (pos.y > max.y) max = pos;
            }
        }

        foreach (var pos in floorPositions)
        {
            TileType tileType = wallType switch
            {
                WallType.WallFront => TileType.WallFront,
                WallType.WallBack => TileType.WallBack,
                WallType.WallLeft => TileType.WallLeft,
                WallType.WallRight => TileType.WallRight,
                _ => TileType.None
            };

            if (pos == min || pos == max)
                tileType = TileType.DoorEdge;

            structureTiles.Add(new StructureTile(pos, tileType));
        }
    }

    public void SetOpenDoorTiles()
    {
        foreach (var tile in structureTiles)
        {
            openTiles.Add(tile);
        }
    }

    public void AppendClosedDoorTiles(StructureTile tile)
    {
        closedTiles.Add(tile);
    }

    public void DrawOpenDoorTiles(StructureTilemap structureTilemap)
    {
        Tilemap baseTilemap = structureTilemap.tilemapLayers.baseTilemap;
        Tilemap baseDecorTilemap = structureTilemap.tilemapLayers.baseDecorationTilemap;
        Tilemap frontTilemap = structureTilemap.tilemapLayers.frontTilemap;
        Tilemap frontDecorationTilemap = structureTilemap.tilemapLayers.frontDecorationTilemap;
        Tilemap collisionTilemap = structureTilemap.tilemapLayers.collisionTilemap;

        foreach (var tile in openTiles)
        {
            baseTilemap.SetTile((Vector3Int)tile.position, tile.baseTile);
            baseDecorTilemap.SetTile((Vector3Int)tile.position, tile.baseDecorTile);
            frontTilemap.SetTile((Vector3Int)tile.position, tile.frontTile);
            frontDecorationTilemap.SetTile((Vector3Int)tile.position, tile.frontDecorTile);
            collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
        }
    }

    public void DrawClosedDoorTiles(StructureTilemap structureTilemap)
    {
        Tilemap baseTilemap = structureTilemap.tilemapLayers.baseTilemap;
        Tilemap baseDecorTilemap = structureTilemap.tilemapLayers.baseDecorationTilemap;
        Tilemap frontTilemap = structureTilemap.tilemapLayers.frontTilemap;
        Tilemap frontDecorationTilemap = structureTilemap.tilemapLayers.frontDecorationTilemap;
        Tilemap collisionTilemap = structureTilemap.tilemapLayers.collisionTilemap;

        foreach (var tile in closedTiles)
        {
            baseTilemap.SetTile((Vector3Int)tile.position, tile.baseTile);
            baseDecorTilemap.SetTile((Vector3Int)tile.position, tile.baseDecorTile);
            frontTilemap.SetTile((Vector3Int)tile.position, tile.frontTile);
            frontDecorationTilemap.SetTile((Vector3Int)tile.position, tile.frontDecorTile);
            collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
        }
    }
}
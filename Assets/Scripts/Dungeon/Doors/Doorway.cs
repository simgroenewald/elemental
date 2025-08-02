using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class Doorway: Structure
{
    public DoorType doortype;
    public Vector2Int midPosition;
    public HashSet<StructureTile> closedTiles = new HashSet<StructureTile>();
    public HashSet<StructureTile> openTiles = new HashSet<StructureTile>();

    public Doorway Initialise(Vector2Int midPositon, int width, DoorType doortype, Grid grid)
    {
        floorPositions = new HashSet<Vector2Int>();
        int sideTileCount = width / 2;

        this.doortype = doortype;
        this.midPosition = midPositon;
        floorPositions.Add(midPositon);
        for (int i = 1; i < sideTileCount + 1; i++)
        {
            Vector2Int doorwayTileA = new Vector2Int();
            Vector2Int doorwayTileB = new Vector2Int();

            if (doortype == DoorType.BackDoor || doortype == DoorType.FrontDoor)
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

        // === Convert tile position to world position ===
        Vector3 worldPosition = grid.CellToWorld((Vector3Int)midPositon) + grid.cellSize / 2f;
        transform.position = worldPosition;

        // === Adjust collider size based on orientation ===
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            if (doortype == DoorType.LeftDoor || doortype == DoorType.RightDoor)
            {
                collider.size = new Vector2(0.2f, width);  // vertical
                collider.offset = Vector2.zero;
            }
            else
            {
                collider.size = new Vector2(width, 0.2f);  // horizontal
                collider.offset = Vector2.zero;
            }
        }

        return this;
    }

    public void GenerateStructureTiles()
    {

        // Determine min and max based on orientation
        Vector2Int min = new Vector2Int(int.MaxValue, int.MaxValue);
        Vector2Int max = new Vector2Int(int.MinValue, int.MinValue);

        foreach (var pos in floorPositions)
        {
            if (doortype == DoorType.FrontDoor || doortype == DoorType.BackDoor)
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
            TileType tileType = doortype switch
            {
                DoorType.FrontDoor => TileType.WallFront,
                DoorType.BackDoor => TileType.WallBack,
                DoorType.LeftDoor => TileType.WallLeft,
                DoorType.RightDoor => TileType.WallRight,
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

    public void DrawOpenDoorTiles(TilemapLayers tilemapLayers)
    {
        Tilemap baseTilemap = tilemapLayers.baseTilemap;
        Tilemap baseDecorTilemap = tilemapLayers.baseDecorationTilemap;
        Tilemap frontTilemap = tilemapLayers.frontTilemap;
        Tilemap frontDecorationTilemap = tilemapLayers.frontDecorationTilemap;
        Tilemap collisionTilemap = tilemapLayers.collisionTilemap;

        foreach (var tile in openTiles)
        {
            baseTilemap.SetTile((Vector3Int)tile.position, tile.baseTile);
            baseDecorTilemap.SetTile((Vector3Int)tile.position, tile.baseDecorTile);
            frontTilemap.SetTile((Vector3Int)tile.position, tile.frontTile);
            frontDecorationTilemap.SetTile((Vector3Int)tile.position, tile.frontDecorTile);
            collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
        }
    }

    public void DrawClosedDoorTiles(TilemapLayers tilemapLayers)
    {
        Tilemap baseTilemap = tilemapLayers.baseTilemap;
        Tilemap baseDecorTilemap = tilemapLayers.baseDecorationTilemap;
        Tilemap frontTilemap = tilemapLayers.frontTilemap;
        Tilemap frontDecorationTilemap = tilemapLayers.frontDecorationTilemap;
        Tilemap collisionTilemap = tilemapLayers.collisionTilemap;

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
using NavMeshPlus.Components;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class Doorway: MonoBehaviour
{
    public Structure structure;
    public DoorType doortype;
    public Vector2Int midPosition;
    public HashSet<StructureTile> closedTiles = new HashSet<StructureTile>();
    public HashSet<StructureTile> openTiles = new HashSet<StructureTile>();
    public NavMeshObstacle obstacle;
    public NavMeshModifierVolume modifierVolume;
    public DungeonRoom room;

    public Doorway Initialise(Vector2Int midPositon, int width, DoorType doortype, DungeonRoom room, Grid grid)
    {
        this.room = room;
        structure.floorPositions = new HashSet<Vector2Int>();
        int sideTileCount = width / 2;

        this.doortype = doortype;
        this.midPosition = midPositon;
        structure.floorPositions.Add(midPositon);
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
            structure.floorPositions.Add(doorwayTileA);
            structure.floorPositions.Add(doorwayTileB);
        }

        // === Convert tile position to world position ===
        Vector3 worldPosition = grid.CellToWorld((Vector3Int)midPositon) + grid.cellSize / 2f;
        transform.position = worldPosition;

        // === Adjust collider size based on orientation ===
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            if (doortype == DoorType.LeftDoor)
            {
                collider.size = new Vector2(0.2f, width);  
                collider.offset = new Vector2(0.5f, 0);
            }
            else if (doortype == DoorType.RightDoor)
            {
                collider.size = new Vector2(0.2f, width);
                collider.offset = new Vector2(-0.5f, 0);
            }
            else if (doortype == DoorType.FrontDoor)
            {
                collider.size = new Vector2(width, 0.2f);
                collider.offset = new Vector2(0, 0.5f);
            }
            else if (doortype == DoorType.BackDoor)
            {
                collider.size = new Vector2(width, 0.2f);
                collider.offset = new Vector2(0, -0.5f);
            }
        }

        obstacle = GetComponent<NavMeshObstacle>();
        if (obstacle != null)
        {
            if (doortype == DoorType.LeftDoor || doortype == DoorType.RightDoor)
            {
                obstacle.size = new Vector3(0.5f, width, 2);
            }
            else
            {
                obstacle.size = new Vector3(width, 0.5f, 2);
            }
        }

        modifierVolume = GetComponent<NavMeshModifierVolume>();
        if (modifierVolume != null)
        {
            if (doortype == DoorType.LeftDoor || doortype == DoorType.RightDoor)
            {
                modifierVolume.size = new Vector3(0.5f, width, 2);
            }
            else
            {
                modifierVolume.size = new Vector3(width, 0.5f, 2);
            }
        }

        return this;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StaticEventHandler.CallRoomChangedEvent(this.room);
        }
    }

    public void GenerateStructureTiles()
    {

        // Determine min and max based on orientation
        Vector2Int min = new Vector2Int(int.MaxValue, int.MaxValue);
        Vector2Int max = new Vector2Int(int.MinValue, int.MinValue);

        foreach (var pos in structure.floorPositions)
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

        foreach (var pos in structure.floorPositions)
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

            structure.structureTiles.Add(new StructureTile(pos, tileType));
        }
    }

    public void SetOpenDoorTiles()
    {
        foreach (var tile in structure.structureTiles)
        {
            openTiles.Add(tile);
        }
    }

    public void AppendClosedDoorTiles(StructureTile tile)
    {
        closedTiles.Add(tile);
    }

    public void Open(TilemapLayers tilemapLayers)
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
        obstacle.enabled = false;
    }

    public void Close(TilemapLayers tilemapLayers)
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
        obstacle.enabled = true;
    }
}
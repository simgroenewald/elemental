using NavMeshPlus.Components;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class Doorway: MonoBehaviour
{
    public Structure structure;
    public DoorType doortype;
    public Vector2Int midPosition;
    public HashSet<StructureTile> closedTiles = new HashSet<StructureTile>();
    public HashSet<StructureTile> openTiles = new HashSet<StructureTile>();
    public NavMeshObstacle obstacle;
    public NavMeshModifierVolume modifierVolume;
    public bool isBossRoomDoorway = false;
    public DungeonRoom room;

    public Doorway Initialise(Vector2Int midPosition, int width, DoorType doortype, DungeonRoom room, Grid grid, bool isBossRoomDoorway)
    {
        this.room = room;
        structure.floorPositions = new HashSet<Vector2Int>();
        int sideTileCount = width / 2;
        this.isBossRoomDoorway = isBossRoomDoorway;

        this.doortype = doortype;
        this.midPosition = midPosition;

        for (int j = 0; j < room.GetDoorDepth(doortype); j ++)
        {
            if (doortype == DoorType.BackDoor || doortype == DoorType.FrontDoor)
            {
                midPosition = new Vector2Int(midPosition.x, midPosition.y + j);
                structure.floorPositions.Add(midPosition);
            } else
            {
                midPosition = new Vector2Int(midPosition.x + j, midPosition.y);
                structure.floorPositions.Add(midPosition);
            }

            for (int i = 1; i < sideTileCount + 1; i++)
            {
                Vector2Int doorwayTileA;
                Vector2Int doorwayTileB;

                if (doortype == DoorType.BackDoor || doortype == DoorType.FrontDoor)
                {
                    doorwayTileA = new Vector2Int(midPosition.x - i, midPosition.y);
                    doorwayTileB = new Vector2Int(midPosition.x + i, midPosition.y);
                }
                else
                {
                    doorwayTileA = new Vector2Int(midPosition.x, midPosition.y - i);
                    doorwayTileB = new Vector2Int(midPosition.x, midPosition.y + i);

                }
                structure.floorPositions.Add(doorwayTileA);
                structure.floorPositions.Add(doorwayTileB);
            }
        }

        // === Convert tile position to world position ===
        Vector3 worldPosition = grid.CellToWorld((Vector3Int)midPosition) + grid.cellSize / 2f;
        transform.position = worldPosition;

        // === Adjust collider size based on orientation ===
        BoxCollider2D entryCollider = transform.Find("EntryCollider").GetComponent<BoxCollider2D>();
        if (entryCollider != null)
        {
            if (doortype == DoorType.LeftDoor)
            {
                entryCollider.size = new Vector2(0.2f, width);
                entryCollider.offset = new Vector2(0.5f, 0);
            }
            else if (doortype == DoorType.RightDoor)
            {
                entryCollider.size = new Vector2(0.2f, width);
                entryCollider.offset = new Vector2(-0.5f, 0);
            }
            else if (doortype == DoorType.FrontDoor)
            {
                entryCollider.size = new Vector2(width, 0.2f);
                entryCollider.offset = new Vector2(0, 0.7f);
            }
            else if (doortype == DoorType.BackDoor)
            {
                entryCollider.size = new Vector2(width, 0.2f);
                entryCollider.offset = new Vector2(0, -0.5f);
            }
        }

        // === Adjust collider size based on orientation ===
        BoxCollider2D exitCollider = transform.Find("ExitCollider").GetComponent<BoxCollider2D>();
        if (exitCollider != null)
        {
            if (doortype == DoorType.LeftDoor)
            {
                exitCollider.size = new Vector2(0.2f, width);
                exitCollider.offset = new Vector2(-0.5f, 0);
            }
            else if (doortype == DoorType.RightDoor)
            {
                exitCollider.size = new Vector2(0.2f, width);
                exitCollider.offset = new Vector2(0.5f, 0);
            }
            else if (doortype == DoorType.FrontDoor)
            {
                exitCollider.size = new Vector2(width, 0.2f);
                exitCollider.offset = new Vector2(0, -0.5f);
            }
            else if (doortype == DoorType.BackDoor)
            {
                exitCollider.size = new Vector2(width, 0.2f);
                exitCollider.offset = new Vector2(0, 0.5f);
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

    public void GenerateStructureTiles()
    {

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

            if (doortype == DoorType.FrontDoor || doortype == DoorType.BackDoor)
            {
                if (!structure.floorPositions.Contains(pos + Vector2Int.left) || !structure.floorPositions.Contains(pos + Vector2Int.right))
                {
                    tileType = TileType.DoorEdge;
                }
            }
            else
            {
                if (!structure.floorPositions.Contains(pos + Vector2Int.up) || !structure.floorPositions.Contains(pos + Vector2Int.down))
                {
                    tileType = TileType.DoorEdge;
                }
            }

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
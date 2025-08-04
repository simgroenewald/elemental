using System;
using System.Collections.Generic;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UnityEngine.WSA;
using static UnityEditor.PlayerSettings;


[DisallowMultipleComponent]
public class DungeonBuilderTest : SingletonMonobehaviour<DungeonBuilderTest>
{
    [Header("Settings")]
    [SerializeField] LevelSettingSO levelSettings;
    [SerializeField] int seed;

    [Header("Generators")]
    [SerializeField] DungeonBuilder dungeonBuilder;
    [SerializeField] DungeonLayoutGenerator dungeonLayoutGenerator;
    [SerializeField] RoomGenerator roomGenerator;
    [SerializeField] ConnectorGenerator connectorGenerator;

    [Header("Mappers")]
    [SerializeField] StructureTypeToGridMapperSO structureToGridMapper;
    [SerializeField] TileTypeToTileMapperSO tileTypeToTileMapper;

    [Header("Tilemaps")]
    [SerializeField] Tilemap structureTilemap;
    [SerializeField] Tilemap tileTypeTilemap;

    List<DungeonRoom> dungeonRooms;
    List<Connector> connectors;
    GameObject dungeonParent;

    public void GenerateDungeon()
    {
        //int seed = UnityEngine.Random.Range(2000, 10000);
        //int seed = 9964;
        //int seed = 6552;
        int seed = 6776;
        Debug.Log("Seed:" + seed);
        UnityEngine.Random.InitState(seed);
        dungeonRooms = dungeonLayoutGenerator.GenerateDungeonLayout(levelSettings);
    }

    public void GenerateRooms()
    {
        dungeonBuilder.GenerateStructuredRooms(dungeonRooms);
    }

    public void GenerateDoors()
    {
        dungeonBuilder.GenerateStructuredDoors(dungeonRooms);
    }

    public void DrawRoomLayouts()
    {
        foreach (DungeonRoom dungeonRoom in dungeonRooms)
        {
            if (dungeonRoom.theme == ElementTheme.Water || dungeonRoom.theme == ElementTheme.Earth)
            {
                DrawGrid(dungeonRoom.bounds.position, dungeonRoom.bounds.size.x, dungeonRoom.bounds.size.y, dungeonRoom.tiles);
            }
            if (dungeonRoom.theme == ElementTheme.Fire || dungeonRoom.theme == ElementTheme.Air)
            {
                DrawRooms(dungeonRoom.subRooms);
            }
        }
    }

    public void GenerateConnectors()
    {
        connectors = dungeonBuilder.GenerateConnectors(dungeonRooms);
    }


    public void GenerateContainers()
    {

        if (dungeonParent != null)
        {
            Debug.Log("Containers already built");
        }
        else
        {
            dungeonParent = new GameObject("DungeonContainer");

            //dungeonBuilder.CreateDungeonContainer(dungeonParent, dungeonRooms, connectors);
        }

    }

    public void PopulateRoomTiles()
    {
        dungeonBuilder.PopulateRoomTiles(dungeonRooms, structureToGridMapper);
    }

    public void PopulateConnectorTiles()
    {

        dungeonBuilder.PopulateConnectorTiles(connectors, structureToGridMapper);
    }

    public void PopulateDoorTiles()
    {
        dungeonBuilder.PopulateOpenDoorTiles(dungeonRooms, structureToGridMapper);
    }

    // Debug Gizmos
    private void OnDrawGizmos()
    {
        if (dungeonRooms == null) return;

        foreach (var room in dungeonRooms)
        {
            Gizmos.color = room.theme switch
            {
                ElementTheme.Fire => Color.yellow,
                ElementTheme.Water => Color.cyan,
                ElementTheme.Air => Color.white,
                ElementTheme.Earth => Color.green,
                _ => Color.white
            };

            Gizmos.DrawSphere(new Vector3(room.nodeGraphPosition.x, room.nodeGraphPosition.y, 0), 0.8f);

            Gizmos.color = room.roomType switch
            {
                RoomType.Start => Color.blue,
                RoomType.Boss => Color.red,
                RoomType.MiniBoss => Color.magenta,
                _ => Color.clear
            };

            Gizmos.DrawSphere(new Vector3(room.nodeGraphPosition.x, room.nodeGraphPosition.y, 0), 0.6f);

            if (room.parent != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(new Vector3(room.nodeGraphPosition.x, room.nodeGraphPosition.y, 0),
                                new Vector3(room.parent.nodeGraphPosition.x, room.parent.nodeGraphPosition.y, 0));
            }
        }
    }

    public void DrawMap()
    {
        foreach (var room in dungeonRooms)
        {
            //DrawBoundArea(room.outerBounds, tileTypeToTileMapper.tileTypeToTileDict[TileType.Bou]);
            //DrawBoundArea(room.bounds, tileTypeToTileMapper.tileTypeToTileDict[TileType.Bou]);
            DrawTypeTiles(room.structure.structureTiles);
        }

        foreach (var connector in connectors)
        {
            if (connector.isStraight)
            {
                DrawTypeTiles(connector.bridgeMain.structure.structureTiles);
            }
            else
            {
                DrawTypeTiles(connector.platform.structure.structureTiles);
                DrawTypeTiles(connector.bridgeStart.structure.structureTiles);
                DrawTypeTiles(connector.bridgeEnd.structure.structureTiles);
            }
        }

        foreach (var room in dungeonRooms)
        {
            DrawRoomDoorways(room.doorways);
        }
    }

    public void DrawRoomTiles()
    {
        dungeonBuilder.DrawRoomTiles(dungeonRooms);
    }

    public void DrawConnectorTiles()
    {
        dungeonBuilder.DrawConnectorTiles(connectors);
    }

    public void DrawDoorTiles()
    {
        dungeonBuilder.DrawRoomDoorTiles(dungeonRooms);
    }

    private void DrawRoomDoorways(List<Doorway> doorways)
    {
        foreach (var doorway in doorways)
        {
            DrawTypeTiles(doorway.structure.structureTiles);
        }
    }

    public void DrawBoundArea(BoundsInt area, TileBase tile)
    {
        for (var x = area.position.x; x < area.position.x + area.size.x; x++)
        {
            for (var y = area.position.y; y < area.position.y + area.size.y; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                structureTilemap.SetTile(tilePosition, tile);
            }
        }
    }

    public void DrawBounds()
    {
        foreach (var room in dungeonRooms)
        {
            DrawFloorTiles(room.structure.floorPositions, tileTypeToTileMapper.tileTypeToTileDict[TileType.Floor]);
        }
    }

    private void DrawFloorTiles(HashSet<Vector2Int> floorPositions, TileBase floorTile)
    {
        throw new NotImplementedException();
    }

    public void DrawEdges(Dictionary<Direction, List<Vector2Int>> edgeLists, TileBase tile)
    {
        foreach (var edgeList in edgeLists)
        {
            foreach (var position in edgeList.Value)
            {
                Vector3Int tilePosition = new Vector3Int(position.x, position.y, 0);
                structureTilemap.SetTile(tilePosition, tile);
            }
        }
    }

    public void DrawGrid(Vector3Int position, int length, int width, int[,] tiles)
    {
        for (int y = 0; y < length; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Quad);
                tile.transform.position = new Vector3(position.x + x + 0.5f - (length / 2), position.y + y + 0.5f - (width / 2), 0);
                tile.transform.localScale = Vector3.one;

                var renderer = tile.GetComponent<Renderer>();
                renderer.sharedMaterial = new Material(Shader.Find("Unlit/Color"));
                renderer.sharedMaterial.color = tiles[y, x] == 1 ? Color.black : Color.white;

                tile.name = $"Tile_{x}_{y}";
                tile.transform.parent = transform;
            }
        }
    }

    public void DrawRooms(List<BoundsInt> roomsList)
    {
        for (int i = 0; i < roomsList.Count; i++)
        {
            BoundsInt room = roomsList[i];

            // Generate a unique color per room using HSV
            Color roomColor = Color.HSVToRGB((i * 0.15f) % 1f, 0.7f, 0.9f);

            for (int y = room.yMin; y < room.yMax; y++)
            {
                for (int x = room.xMin; x < room.xMax; x++)
                {
                    GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    tile.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0);
                    tile.transform.localScale = Vector3.one;

                    var renderer = tile.GetComponent<Renderer>();
                    renderer.sharedMaterial = new Material(Shader.Find("Unlit/Color"));
                    renderer.sharedMaterial.color = roomColor;

                    tile.name = $"Room_{i}_Tile_{x}_{y}";
                    tile.transform.parent = transform;
                }
            }
        }
    }

    public void ClearAllRooms()
    {
/*        GameObject dungeonParent = GameObject.Find("Dungeon");
        if (dungeonParent != null)
        {
            DestroyImmediate(dungeonParent);
        }
        //ClearRooms();*/
        //ClearOldTiles();
        ClearTilemap();
        dungeonRooms = null;
    }

    public void ClearRooms()
    {
        // Destroy all children (tiles) created under this GameObject
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
    private void ClearOldTiles()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    public void ClearTilemap()
    {
        structureTilemap.ClearAllTiles();
        tileTypeTilemap.ClearAllTiles();
    }

    public void DrawTypeTiles(HashSet<StructureTile> roomTiles)
    {
        Dictionary<TileType, UnityEngine.Tilemaps.Tile> tileTypeToTileDict = tileTypeToTileMapper.GetTileTypeToTileDict();
        foreach (var roomTile in roomTiles)
        {
            Vector3Int tilePosition = new Vector3Int(roomTile.position.x, roomTile.position.y, 0);
            tileTypeTilemap.SetTile(tilePosition, tileTypeToTileDict[roomTile.tileType]);
        }
    }
}
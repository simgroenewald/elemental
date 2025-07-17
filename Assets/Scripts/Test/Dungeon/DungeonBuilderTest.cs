using System;
using System.Collections.Generic;
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
    [SerializeField] DungeonLayoutGenerator dungeonLayoutGenerator;
    [SerializeField] RoomGenerator roomGenerator;
    [SerializeField] ConnectorGenerator connectorGenerator;

    [Header("Mappers")]
    [SerializeField] StructureTypeToGridMapperSO structureToGridMapper;
    [SerializeField] TileTypeToTileMapperSO tileTypeToTileMapper;

    [Header("Tilemaps")]
    [SerializeField] Tilemap structureTilemap;
    [SerializeField] Tilemap baseTilemap;
    [SerializeField] Tilemap baseDecorationTilemap;
    [SerializeField] Tilemap environmentDecorationTilemap;
    [SerializeField] Tilemap platformTilemap;
    [SerializeField] Tilemap platformDecorationTilemap;
    [SerializeField] Tilemap bridgeTilemap;
    [SerializeField] Tilemap instancesTilemap;
    [SerializeField] Tilemap frontTilemap;
    [SerializeField] Tilemap frontDecorationTilemap;
    [SerializeField] Tilemap tileTypeTilemap;
    [SerializeField] Tilemap collisionTilemap;

    List<DungeonRoom> dungeonRooms;
    List<Connector> connectors;

    /// <summary>
    /// Generate random dungeon, returns true if dungeon built, false if failed
    /// </summary>
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
        foreach (var dungeonRoom in dungeonRooms)
        {
                roomGenerator.GenerateStructuredRoom(dungeonRoom);
        }
    }

    public void PopulateRoomTiles()
    {
        structureToGridMapper.structureTypeToGridDict = structureToGridMapper.GetStructureTypeToGridDict();
        Grid grid = structureToGridMapper.structureTypeToGridDict[StructureType.WaterRoom];
        TilemapProperties properties = TilemapAnalyser.GenerateProperties(grid);
        //DungeonRoom dungeonRoom = dungeonRooms[1];
        foreach (var dungeonRoom in dungeonRooms)
        {
            if (dungeonRoom.theme == ElementTheme.Water || dungeonRoom.theme == ElementTheme.Earth)
            {
                WaveFunctionCollapse2 wfc = new WaveFunctionCollapse2(dungeonRoom.structureTiles, properties, 1);
                wfc.PopulateOutputCells();
            }
            if (dungeonRoom.theme == ElementTheme.Fire || dungeonRoom.theme == ElementTheme.Air)
            {
                WaveFunctionCollapse2 wfc = new WaveFunctionCollapse2(dungeonRoom.structureTiles, properties, 1);
                wfc.PopulateOutputCells();
            }
        }
    }

    public void GenerateCorridors()
    {
        // Determines width of whole connector including 'walls'
        connectors = connectorGenerator.GenerateConnectors(dungeonRooms, 3);
    }

    public void PopulateConnectorTiles()
    {

        structureToGridMapper.structureTypeToGridDict = structureToGridMapper.GetStructureTypeToGridDict();

        Grid verticalBridgeGrid = structureToGridMapper.structureTypeToGridDict[StructureType.VerticalWoodenBridge];
        TilemapProperties verticalBridgeProperties = TilemapAnalyser.GenerateProperties(verticalBridgeGrid);

        Grid horizontalBridgeGrid = structureToGridMapper.structureTypeToGridDict[StructureType.HorizontalWoodenBridge];
        TilemapProperties horizontalBridgeProperties = TilemapAnalyser.GenerateProperties(horizontalBridgeGrid);

        Grid platformGrid = structureToGridMapper.structureTypeToGridDict[StructureType.Platform];
        TilemapProperties platformProperties = TilemapAnalyser.GenerateProperties(platformGrid);
        foreach (var connector in connectors)
        {

            if (connector.orientation == ConnectorOrientation.Horizontal)
            {
                if (connector.isStraight)
                {
                    WaveFunctionCollapse2 wfc = new WaveFunctionCollapse2(connector.bridgeMain.structureTiles, horizontalBridgeProperties, 1);
                    wfc.PopulateOutputCells();
                }
                else
                {
                    WaveFunctionCollapse2 bridgeStartWFC = new WaveFunctionCollapse2(connector.bridgeStart.structureTiles, horizontalBridgeProperties, 1);
                    bridgeStartWFC.PopulateOutputCells();
                    WaveFunctionCollapse2 bridgeEndWFC = new WaveFunctionCollapse2(connector.bridgeEnd.structureTiles, horizontalBridgeProperties, 1);
                    bridgeEndWFC.PopulateOutputCells();
                    WaveFunctionCollapse2 platformWFC = new WaveFunctionCollapse2(connector.bridgeEnd.structureTiles, platformProperties, 1);
                    platformWFC.PopulateOutputCells();
                }
            }
            else
            {
                if (connector.isStraight)
                {
                    WaveFunctionCollapse2 wfc = new WaveFunctionCollapse2(connector.bridgeMain.structureTiles, verticalBridgeProperties, 1);
                    wfc.PopulateOutputCells();
                }
                else
                {
                    WaveFunctionCollapse2 platformWFC = new WaveFunctionCollapse2(connector.platform.structureTiles, platformProperties, 1);
                    platformWFC.PopulateOutputCells();
                    WaveFunctionCollapse2 bridgeStartWFC = new WaveFunctionCollapse2(connector.bridgeStart.structureTiles, verticalBridgeProperties, 1);
                    bridgeStartWFC.PopulateOutputCells();
                    WaveFunctionCollapse2 bridgeEndWFC = new WaveFunctionCollapse2(connector.bridgeEnd.structureTiles, verticalBridgeProperties, 1);
                    bridgeEndWFC.PopulateOutputCells();
                }
            }

        }
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
        tileTypeToTileMapper.tileTypeToTileDict = tileTypeToTileMapper.GetTileTypeToTileDict();
        foreach (var room in dungeonRooms)
        {
            //DrawBoundArea(room.outerBounds, tileTypeToTileMapper.tileTypeToTileDict[TileType.Bou]);
            //DrawBoundArea(room.bounds, tileTypeToTileMapper.tileTypeToTileDict[TileType.Bou]);
            DrawRoomTiles(tileTypeTilemap, room.structureTiles, tileTypeToTileMapper.tileTypeToTileDict);
        }

        foreach (var connector in connectors)
        {
            if (connector.isStraight)
            {
                DrawRoomTiles(tileTypeTilemap, connector.bridgeMain.structureTiles, tileTypeToTileMapper.tileTypeToTileDict);
            }
            else
            {
                DrawRoomTiles(tileTypeTilemap, connector.platform.structureTiles, tileTypeToTileMapper.tileTypeToTileDict);
                DrawRoomTiles(tileTypeTilemap, connector.bridgeStart.structureTiles, tileTypeToTileMapper.tileTypeToTileDict);
                DrawRoomTiles(tileTypeTilemap, connector.bridgeEnd.structureTiles, tileTypeToTileMapper.tileTypeToTileDict);
            }
        }

        foreach (var room in dungeonRooms)
        {
            DrawRoomDoorways(room.doorways, tileTypeToTileMapper.tileTypeToTileDict[TileType.Door]);
        }
    }

    public void DrawRoomTiles()
    {
        foreach (var room in dungeonRooms)
        {
            foreach (var tile in room.structureTiles)
            {
                baseTilemap.SetTile((Vector3Int)tile.position, tile.baseTile);
                baseDecorationTilemap.SetTile((Vector3Int)tile.position, tile.decorTile);
                collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
            }
        }
    }

    public void DrawConnectorTiles()
    {
        foreach (var connector in connectors)
        {
            if (connector.isStraight)
            {
                foreach (var tile in connector.bridgeMain.structureTiles)
                {
                    bridgeTilemap.SetTile((Vector3Int)tile.position, tile.baseTile);
                    frontTilemap.SetTile((Vector3Int)tile.position, tile.decorTile);
                    collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
                }
            }
            if (!connector.isStraight)
            {
                foreach (var tile in connector.platform.structureTiles)
                {
                    platformTilemap.SetTile((Vector3Int)tile.position, tile.baseTile);
                    platformDecorationTilemap.SetTile((Vector3Int)tile.position, tile.decorTile);
                    collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
                }
                foreach (var tile in connector.bridgeStart.structureTiles)
                {
                    bridgeTilemap.SetTile((Vector3Int)tile.position, tile.baseTile);
                    frontTilemap.SetTile((Vector3Int)tile.position, tile.decorTile);
                    collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
                }
                foreach (var tile in connector.bridgeEnd.structureTiles)
                {
                    bridgeTilemap.SetTile((Vector3Int)tile.position, tile.baseTile);
                    frontTilemap.SetTile((Vector3Int)tile.position, tile.decorTile);
                    collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
                }
            }
        }
    }

    private void DrawRoomDoorways(List<Doorway> doorways, UnityEngine.Tilemaps.Tile tile)
    {
        foreach (var doorway in doorways)
        {
            foreach (var tilePos in doorway.positions)
            {
                Vector3Int tilePosition = new Vector3Int(tilePos.x, tilePos.y, 0);
                tileTypeTilemap.SetTile(tilePosition, tile);
            }
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
            DrawFloorTiles(room.floorPositions, tileTypeToTileMapper.tileTypeToTileDict[TileType.Floor]);
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
        ClearRooms();
        ClearOldTiles();
        ClearTilemap();
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
        baseTilemap.ClearAllTiles();
        baseDecorationTilemap.ClearAllTiles();
        environmentDecorationTilemap.ClearAllTiles();
        platformTilemap.ClearAllTiles();
        platformDecorationTilemap.ClearAllTiles();
        bridgeTilemap.ClearAllTiles();
        instancesTilemap.ClearAllTiles();
        frontTilemap.ClearAllTiles();
        frontDecorationTilemap.ClearAllTiles();
        tileTypeTilemap.ClearAllTiles();
        collisionTilemap.ClearAllTiles();
    }

    public void DrawRoomTiles(Tilemap tilemap, HashSet<StructureTile> roomTiles, Dictionary<TileType, UnityEngine.Tilemaps.Tile> tileTypeToTileDict)
    {
        foreach (var roomTile in roomTiles)
        {
            Vector3Int tilePosition = new Vector3Int(roomTile.position.x, roomTile.position.y, 0);
            tilemap.SetTile(tilePosition, tileTypeToTileDict[roomTile.tileType]);
        }
    }

}
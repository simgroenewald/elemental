using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UnityEngine.WSA;
using static UnityEditor.PlayerSettings;


[DisallowMultipleComponent]
public class DungeonBuilder : SingletonMonobehaviour<DungeonBuilder>
{

    [Header("Generators")]
    [SerializeField] RoomGenerator roomGenerator;
    [SerializeField] ConnectorGenerator connectorGenerator;
    [SerializeField] DungeonLayoutGenerator dungeonLayoutGenerator;
    [SerializeField] LevelSettingSO levelSettings;
    [SerializeField]
    public StructureTypeToGridMapperSO structureToGridMapper;
    [SerializeField] Tilemap outputBaseTilemap;
    [SerializeField] Tilemap outputDecorTilemap;
    [SerializeField] Tilemap outputCollisionTilemap;
    [SerializeField] Tilemap outputBasePlatformTilemap;
    [SerializeField] Tilemap outputDecorPlatformTilemap;
    [SerializeField] Tilemap outputBaseBridgeTilemap;
    [SerializeField] Tilemap outputDecorBridgeTilemap;
    List<DungeonRoom> dungeonRooms;
    List<Connector> connectors;

    public Grid grid;
    public Tilemap tilemap;
    public TileBase floorTile;
    public TileBase outerBoundsTile;
    public TileBase roomBoundsTile;
    public TileBase corridorTile;
    public TileBase doorTile;
    public TileTypeToTileMapperSO tileTypeToTileMapper;

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
            if (dungeonRoom.theme == ElementTheme.Water || dungeonRoom.theme == ElementTheme.Earth)
            {
                /*                Debug.Log(dungeonRoom.theme);
                                Debug.Log(dungeonRoom.bounds.position.x + " " + dungeonRoom.bounds.position.y);
                                Debug.Log(dungeonRoom.bounds.size.x + " " + dungeonRoom.bounds.size.y);*/
                //roomGenerator.GenerateNaturalRoom(dungeonRoom);
                roomGenerator.GenerateStructuredRoom(dungeonRoom);
            }
            if (dungeonRoom.theme == ElementTheme.Fire || dungeonRoom.theme == ElementTheme.Air)
            {
                /*                Debug.Log(dungeonRoom.theme);
                                Debug.Log(dungeonRoom.bounds.position.x + " " + dungeonRoom.bounds.position.y);
                                Debug.Log(dungeonRoom.bounds.size.x + " " + dungeonRoom.bounds.size.y);*/
                roomGenerator.GenerateStructuredRoom(dungeonRoom);
            }
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
                /*                Debug.Log(dungeonRoom.theme);
                                Debug.Log(dungeonRoom.bounds.position.x + " " + dungeonRoom.bounds.position.y);
                                Debug.Log(dungeonRoom.bounds.size.x + " " + dungeonRoom.bounds.size.y);*/
                //roomGenerator.GenerateNaturalRoom(dungeonRoom);
                WaveFunctionCollapse2 wfc = new WaveFunctionCollapse2(dungeonRoom.structureTiles, properties, 1);
                wfc.PopulateOutputCells();
            }
            if (dungeonRoom.theme == ElementTheme.Fire || dungeonRoom.theme == ElementTheme.Air)
            {
                /*                Debug.Log(dungeonRoom.theme);
                                Debug.Log(dungeonRoom.bounds.position.x + " " + dungeonRoom.bounds.position.y);
                                Debug.Log(dungeonRoom.bounds.size.x + " " + dungeonRoom.bounds.size.y);*/
                WaveFunctionCollapse2 wfc = new WaveFunctionCollapse2(dungeonRoom.structureTiles, properties, 1);
                wfc.PopulateOutputCells();
            }
        }
    }

    public void GenerateCorridors()
    {
        // Determines width of whole connector including 'walls'
        connectors = connectorGenerator.GenerateConnectors(dungeonRooms, 3);
        /*        foreach (var corridor in corridors)
                {
                    foreach (var tile in corridor.floorPositions)
                    {
                        Debug.Log($"Generated: {tile.x}: {tile.y}");
                    }
                }*/
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

            /*            // Semi-transparent gray fill for room bounds
                        Gizmos.color = new Color(0.6f, 0.6f, 0.6f, 0.3f);
                        Gizmos.DrawCube(
                            room.outerBounds.position,
                            room.outerBounds.size
                        );

                        // Semi-transparent gray fill for room bounds
                        Gizmos.color = new Color(0.0f, 0.0f, 0.6f, 0.5f);
                        Gizmos.DrawCube(
                            room.bounds.position,
                            room.bounds.size
                        );

                        // Red wireframe outline for clarity
                        Gizmos.color = Color.red;
                        Gizmos.DrawWireCube(
                            room.outerBounds.position,
                            room.outerBounds.size
                        );*/
        }
    }

    public void DrawMap()
    {
        tileTypeToTileMapper.tileTypeToTileDict = tileTypeToTileMapper.GetTileTypeToTileDict();
        foreach (var room in dungeonRooms)
        {
            DrawBoundArea(room.outerBounds, outerBoundsTile);
            DrawBoundArea(room.bounds, roomBoundsTile);
            DrawRoomTiles(room.structureTiles, tileTypeToTileMapper.tileTypeToTileDict);
        }

        foreach (var connector in connectors)
        {
            if (connector.isStraight)
            {
                DrawRoomTiles(connector.bridgeMain.structureTiles, tileTypeToTileMapper.tileTypeToTileDict);
            }
            else
            {
                DrawRoomTiles(connector.platform.structureTiles, tileTypeToTileMapper.tileTypeToTileDict);
                DrawRoomTiles(connector.bridgeStart.structureTiles, tileTypeToTileMapper.tileTypeToTileDict);
                DrawRoomTiles(connector.bridgeEnd.structureTiles, tileTypeToTileMapper.tileTypeToTileDict);
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
                outputBaseTilemap.SetTile((Vector3Int)tile.position, tile.baseTile);
                outputDecorTilemap.SetTile((Vector3Int)tile.position, tile.decorTile);
                outputCollisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
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
                    outputBaseBridgeTilemap.SetTile((Vector3Int)tile.position, tile.baseTile);
                    outputDecorBridgeTilemap.SetTile((Vector3Int)tile.position, tile.decorTile);
                    outputCollisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
                }
            }
            if (!connector.isStraight)
            {
                foreach (var tile in connector.platform.structureTiles)
                {
                    outputBasePlatformTilemap.SetTile((Vector3Int)tile.position, tile.baseTile);
                    outputDecorPlatformTilemap.SetTile((Vector3Int)tile.position, tile.decorTile);
                    outputCollisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
                }
                foreach (var tile in connector.bridgeStart.structureTiles)
                {
                    outputBaseBridgeTilemap.SetTile((Vector3Int)tile.position, tile.baseTile);
                    outputDecorBridgeTilemap.SetTile((Vector3Int)tile.position, tile.decorTile);
                    outputCollisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
                }
                foreach (var tile in connector.bridgeEnd.structureTiles)
                {
                    outputBaseBridgeTilemap.SetTile((Vector3Int)tile.position, tile.baseTile);
                    outputDecorBridgeTilemap.SetTile((Vector3Int)tile.position, tile.decorTile);
                    outputCollisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
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
                tilemap.SetTile(tilePosition, tile);
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
                tilemap.SetTile(tilePosition, tile);
            }
        }
    }

    public void DrawBounds()
    {
        foreach (var room in dungeonRooms)
        {
            DrawFloorTiles(room.floorPositions, floorTile);
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
                tilemap.SetTile(tilePosition, tile);
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
        tilemap.ClearAllTiles();
        outputBaseTilemap.ClearAllTiles();
        outputDecorTilemap.ClearAllTiles();
        outputCollisionTilemap.ClearAllTiles();
        outputBasePlatformTilemap.ClearAllTiles();
        outputDecorPlatformTilemap.ClearAllTiles();
        outputBaseBridgeTilemap.ClearAllTiles();
        outputDecorBridgeTilemap.ClearAllTiles();
    }

    public void DrawRoomTiles(HashSet<StructureTile> roomTiles, Dictionary<TileType, UnityEngine.Tilemaps.Tile> tileTypeToTileDict)
    {
        foreach (var roomTile in roomTiles)
        {
            Vector3Int tilePosition = new Vector3Int(roomTile.position.x, roomTile.position.y, 0);
            tilemap.SetTile(tilePosition, tileTypeToTileDict[roomTile.tileType]);
        }
    }

    private void DrawTile(Vector2Int position, TileBase tile)
    {
        var worldPosition = grid.CellToWorld(new Vector3Int(position.x, position.y, 0));
        /*        worldPosition += new Vector3(0.5f, 0.5f, 0);
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Quad);
                tile.transform.position = worldPosition;
                tile.transform.localScale = Vector3.one;

                var renderer = tile.GetComponent<Renderer>();
                renderer.sharedMaterial = new Material(Shader.Find("Unlit/Color"));
                renderer.sharedMaterial.color = Color.white;

                tile.name = $"Tile_{position.x}_{position.y}";
                tile.transform.parent = transform;*/
        Vector3Int tilePosition = new Vector3Int(position.x, position.y, 0);
        tilemap.SetTile(tilePosition, tile);
    }
}
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

    [Header("Mappers")]
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

    public void GenerateDungeon(int seed, LevelSettingSO level)
    {
        UnityEngine.Random.InitState(seed);

        DungeonLayoutGenerator dungeonLayoutGenerator = new DungeonLayoutGenerator();
        StructureTypeToGridMapperSO structureToGridMapper = new StructureTypeToGridMapperSO();

        List<DungeonRoom> dungeonRooms = dungeonLayoutGenerator.GenerateDungeonLayout(level);
        GenerateStructuredRooms(dungeonRooms);
        List<Connector> connectors = GenerateConnectors(dungeonRooms);
        PopulateRoomTiles(dungeonRooms, structureToGridMapper);
        PopulateConnectorTiles(connectors, structureToGridMapper);
    }

    public void GenerateStructuredRooms(List<DungeonRoom> dungeonRooms)
    {
        RoomGenerator roomGenerator = new RoomGenerator();
        foreach (var dungeonRoom in dungeonRooms)
        {
            roomGenerator.GenerateStructuredRoom(dungeonRoom);
        }
    }

    public List<Connector> GenerateConnectors(List<DungeonRoom> dungeonRooms)
    {
        ConnectorGenerator connectorGenerator = new ConnectorGenerator();
        // Determines width of whole connector including 'walls'
        return connectorGenerator.GenerateConnectors(dungeonRooms, 3); ;
    }

    public void PopulateRoomTiles(List<DungeonRoom> dungeonRooms, StructureTypeToGridMapperSO structureToGridMapper)
    {
        structureToGridMapper.structureTypeToGridDict = structureToGridMapper.GetStructureTypeToGridDict();
        Grid grid = structureToGridMapper.structureTypeToGridDict[StructureType.WaterRoom];
        TilemapProperties properties = TilemapAnalyser.GenerateProperties(grid);

        foreach (var dungeonRoom in dungeonRooms)
        {
            WaveFunctionCollapse2 wfc = new WaveFunctionCollapse2(dungeonRoom.structureTiles, properties, 1);
            wfc.PopulateOutputCells();
        }
    }

    public void PopulateConnectorTiles(List<Connector> connectors, StructureTypeToGridMapperSO structureToGridMapper)
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

    public void DrawRoomTiles(List<DungeonRoom> dungeonRooms)
    {
        foreach (var room in dungeonRooms)
        {
            foreach (var tile in room.structureTiles)
            {
                baseTilemap.SetTile((Vector3Int)tile.position, tile.baseTile);
                frontTilemap.SetTile((Vector3Int)tile.position, tile.frontTile);
                collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
            }
        }
    }

    public void DrawConnectorTiles(List<Connector> connectors)
    {
        foreach (var connector in connectors)
        {
            if (connector.isStraight)
            {
                foreach (var tile in connector.bridgeMain.structureTiles)
                {
                    bridgeTilemap.SetTile((Vector3Int)tile.position, tile.baseTile);
                    frontTilemap.SetTile((Vector3Int)tile.position, tile.frontTile);
                    collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
                }
            }
            if (!connector.isStraight)
            {
                foreach (var tile in connector.platform.structureTiles)
                {
                    platformTilemap.SetTile((Vector3Int)tile.position, tile.baseTile);
                    frontTilemap.SetTile((Vector3Int)tile.position, tile.frontTile);
                    collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
                }
                foreach (var tile in connector.bridgeStart.structureTiles)
                {
                    bridgeTilemap.SetTile((Vector3Int)tile.position, tile.baseTile);
                    frontTilemap.SetTile((Vector3Int)tile.position, tile.frontTile);
                    collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
                }
                foreach (var tile in connector.bridgeEnd.structureTiles)
                {
                    bridgeTilemap.SetTile((Vector3Int)tile.position, tile.baseTile);
                    frontTilemap.SetTile((Vector3Int)tile.position, tile.frontTile);
                    collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
                }
            }
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

}
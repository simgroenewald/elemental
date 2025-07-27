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

    [Header("Templates")]
    [SerializeField] GameObject dungeonTemplate;
    TilemapLayers dungeonLayers;

    [Header("Scriptable Objects")]
    [SerializeField] RoomSizePresetsSO roomSizePresets;
    [SerializeField] StructureTypeToGridMapperSO structureToGridMapper;


    public Dungeon GenerateDungeon(int seed, LevelSettingSO level)
    {
        UnityEngine.Random.InitState(seed);

        DungeonLayoutGenerator dungeonLayoutGenerator = new DungeonLayoutGenerator(roomSizePresets);

        List<DungeonRoom> dungeonRooms = dungeonLayoutGenerator.GenerateDungeonLayout(level);
        GenerateStructuredRooms(dungeonRooms);
        List<Connector> connectors = GenerateConnectors(dungeonRooms);
        PopulateRoomTiles(dungeonRooms, structureToGridMapper);
        PopulateConnectorTiles(connectors, structureToGridMapper);

        GameObject dungeonParent = new GameObject("Dungeon");

        CreateDungeonContainer(dungeonParent, dungeonRooms, connectors);

        CreateDungeonCollisionLayer(dungeonParent, dungeonRooms, connectors);

        DrawDungeonTemplateTiles(dungeonRooms, connectors);

        Dungeon dungeon = new Dungeon(dungeonRooms, connectors, dungeonLayers, seed);
        return dungeon;
    }

    public void CreateDungeonContainer(GameObject parent, List<DungeonRoom> dungeonRooms, List<Connector> connectors) {

        foreach (var dungeonRoom in dungeonRooms)
        {
            dungeonRoom.CreateStructureContainer(parent);
            dungeonRoom.structureTilemap.tilemapLayers.collisionTilemap.gameObject.layer = LayerMask.NameToLayer("Enemy");
        }

        foreach (var connector in connectors)
        {
            connector.CreateStructureContainer(parent);
        }
    }

    public void CreateDungeonCollisionLayer(GameObject parent, List<DungeonRoom> dungeonRooms, List<Connector> connectors)
    {
        GameObject dungeonTemplateInstance = Instantiate(dungeonTemplate, parent.transform);
        dungeonLayers = dungeonTemplateInstance.GetComponent<TilemapLayers>();
        dungeonLayers.collisionTilemap.GetComponent<TilemapRenderer>().enabled = false;


        foreach (var dungeonRoom in dungeonRooms)
        {
            foreach (var tile in dungeonRoom.structureTiles)
            {
                dungeonLayers.collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
            }
        }

        foreach (var connector in connectors)
        {
            if (connector.isStraight)
            {
                foreach (var tile in connector.bridgeMain.structureTiles)
                {
                    dungeonLayers.collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
                }
            }
            if (!connector.isStraight)
            {
                foreach (var tile in connector.platform.structureTiles)
                {
                    dungeonLayers.collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
                }
                foreach (var tile in connector.bridgeStart.structureTiles)
                {
                    dungeonLayers.collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
                }
                foreach (var tile in connector.bridgeEnd.structureTiles)
                {
                    dungeonLayers.collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
                }
            }
        }
        dungeonLayers.collisionTilemap.gameObject.layer = LayerMask.NameToLayer("Player");
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

    public void DrawDungeonTemplateTiles(List<DungeonRoom> dungeonRooms, List<Connector> connectors)
    {
        DrawRoomTiles(dungeonRooms);
        DrawConnectorTiles(connectors);

    }

    public void DrawRoomTiles(List<DungeonRoom> dungeonRooms)
    {
        foreach (var room in dungeonRooms)
        {
            room.DrawRoomTiles();
        }
    }

    public void DrawConnectorTiles(List<Connector> connectors)
    {
        foreach (var connector in connectors)
        {
            connector.DrawConnectorTiles();
        }
    }
}
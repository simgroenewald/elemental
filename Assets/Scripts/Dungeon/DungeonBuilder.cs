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
    [SerializeField] DungeonLayoutGenerator dungeonLayoutGenerator;
    [SerializeField] RoomGenerator roomGenerator;
    [SerializeField] ConnectorGenerator connectorGenerator;

    [Header("Dungeon Parent")]
    [SerializeField] GameObject dungeonParent;

    [Header("Tilemaps")]
    [SerializeField] TilemapLayers dungeonLayers;

    [Header("Scriptable Objects")]
    [SerializeField] StructureTypeToGridMapperSO structureToGridMapper;


    public Dungeon GenerateDungeon(int seed, LevelSettingSO level)
    {
        UnityEngine.Random.InitState(seed);
        // Generated dungeon overall layout
        List<DungeonRoom> dungeonRooms = dungeonLayoutGenerator.GenerateDungeonLayout(level);

        GenerateStructuredRooms(dungeonRooms);
        List<Connector> connectors = GenerateConnectors(dungeonRooms);
        GenerateStructuredDoors(dungeonRooms);
        PopulateRoomTiles(dungeonRooms, structureToGridMapper);
        PopulateOpenDoorTiles(dungeonRooms, structureToGridMapper);
        PopulateConnectorTiles(connectors, structureToGridMapper);

        //GameObject dungeonParent = new GameObject("Dungeon");

        CreateDungeonCollisionLayer(dungeonRooms, connectors);

        DrawDungeonTemplateTiles(dungeonRooms, connectors);

        Dungeon dungeon = new Dungeon(dungeonRooms, connectors, dungeonLayers, seed);
        return dungeon;
    }

    public void CreateDungeonCollisionLayer(List<DungeonRoom> dungeonRooms, List<Connector> connectors)
    {


        foreach (var dungeonRoom in dungeonRooms)
        {
            foreach (var tile in dungeonRoom.structure.structureTiles)
            {
                dungeonLayers.collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
            }
        }

        foreach (var dungeonRoom in dungeonRooms)
        {
            foreach (var door in dungeonRoom.doorways)
            {
                foreach (var tile in door.structureTiles)
                {
                    dungeonLayers.collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
                }
            }
        }

        foreach (var connector in connectors)
        {
            if (connector.isStraight)
            {
                foreach (var tile in connector.bridgeMain.structure.structureTiles)
                {
                    dungeonLayers.collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
                }
            }
            if (!connector.isStraight)
            {
                foreach (var tile in connector.platform.structure.structureTiles)
                {
                    dungeonLayers.collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
                }
                foreach (var tile in connector.bridgeStart.structure.structureTiles)
                {
                    dungeonLayers.collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
                }
                foreach (var tile in connector.bridgeEnd.structure.structureTiles)
                {
                    dungeonLayers.collisionTilemap.SetTile((Vector3Int)tile.position, tile.collisionTile);
                }
            }
        }
        dungeonLayers.collisionTilemap.gameObject.layer = LayerMask.NameToLayer("Player");
    }


    public void GenerateStructuredRooms(List<DungeonRoom> dungeonRooms)
    {
        foreach (var dungeonRoom in dungeonRooms)
        {
            roomGenerator.GenerateStructuredRoom(dungeonRoom);
        }
    }

    public void GenerateStructuredDoors(List<DungeonRoom> dungeonRooms)
    {
        foreach (var dungeonRoom in dungeonRooms)
        {
            foreach (var door in dungeonRoom.doorways)
            {
                door.GenerateStructureTiles();
            }
        }
    }

    public List<Connector> GenerateConnectors(List<DungeonRoom> dungeonRooms)
    {
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
            WaveFunctionCollapse2 wfc = new WaveFunctionCollapse2(dungeonRoom.structure.structureTiles, properties, 10);
            wfc.PopulateOutputCells();
        }
    }

    public void PopulateOpenDoorTiles(List<DungeonRoom> dungeonRooms, StructureTypeToGridMapperSO structureToGridMapper)
    {
        structureToGridMapper.structureTypeToGridDict = structureToGridMapper.GetStructureTypeToGridDict();
        Grid grid = structureToGridMapper.structureTypeToGridDict[StructureType.WaterDoor];
        TilemapProperties properties = TilemapAnalyser.GenerateProperties(grid);

        foreach (var dungeonRoom in dungeonRooms)
        {
            foreach (var door in dungeonRoom.doorways)
            {
                WaveFunctionCollapse2 wfc = new WaveFunctionCollapse2(door.structureTiles, properties, 1);
                wfc.PopulateOutputCells();
                door.SetOpenDoorTiles();
            }
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
                    WaveFunctionCollapse2 wfc = new WaveFunctionCollapse2(connector.bridgeMain.structure.structureTiles, horizontalBridgeProperties, 1);
                    wfc.PopulateOutputCells();
                }
                else
                {
                    WaveFunctionCollapse2 bridgeStartWFC = new WaveFunctionCollapse2(connector.bridgeStart.structure.structureTiles, horizontalBridgeProperties, 1);
                    bridgeStartWFC.PopulateOutputCells();
                    WaveFunctionCollapse2 bridgeEndWFC = new WaveFunctionCollapse2(connector.bridgeEnd.structure.structureTiles, horizontalBridgeProperties, 1);
                    bridgeEndWFC.PopulateOutputCells();
                    WaveFunctionCollapse2 platformWFC = new WaveFunctionCollapse2(connector.bridgeEnd.structure.structureTiles, platformProperties, 1);
                    platformWFC.PopulateOutputCells();
                }
            }
            else
            {
                if (connector.isStraight)
                {
                    WaveFunctionCollapse2 wfc = new WaveFunctionCollapse2(connector.bridgeMain.structure.structureTiles, verticalBridgeProperties, 1);
                    wfc.PopulateOutputCells();
                }
                else
                {
                    WaveFunctionCollapse2 platformWFC = new WaveFunctionCollapse2(connector.platform.structure.structureTiles, platformProperties, 1);
                    platformWFC.PopulateOutputCells();
                    WaveFunctionCollapse2 bridgeStartWFC = new WaveFunctionCollapse2(connector.bridgeStart.structure.structureTiles, verticalBridgeProperties, 1);
                    bridgeStartWFC.PopulateOutputCells();
                    WaveFunctionCollapse2 bridgeEndWFC = new WaveFunctionCollapse2(connector.bridgeEnd.structure.structureTiles, verticalBridgeProperties, 1);
                    bridgeEndWFC.PopulateOutputCells();
                }
            }

        }
    }

    public void DrawDungeonTemplateTiles(List<DungeonRoom> dungeonRooms, List<Connector> connectors)
    {
        DrawRoomTiles(dungeonRooms);
        DrawConnectorTiles(connectors);
        DrawRoomDoorTiles(dungeonRooms);
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

    public void DrawRoomDoorTiles(List<DungeonRoom> dungeonRooms)
    {
        foreach (var room in dungeonRooms)
        {
            room.DrawOpenRoomDoorwayTiles();
        }
    }
}
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
                foreach (var tile in door.structure.structureTiles)
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

            if (dungeonRoom.theme == ElementTheme.Air)
            {
                roomGenerator.GenerateStructuredRoom(dungeonRoom, 3);
            }
            else if (dungeonRoom.theme == ElementTheme.Earth)
            {
                roomGenerator.GenerateStructuredRoom(dungeonRoom, 2);
            }
            else if (dungeonRoom.theme == ElementTheme.Fire)
            {
                roomGenerator.GenerateStructuredRoom(dungeonRoom, 2);
            }
            else
            {
                roomGenerator.GenerateStructuredRoom(dungeonRoom, 3);
            }
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

        Grid waterGrid = structureToGridMapper.structureTypeToGridDict[StructureType.WaterRoom];
        TilemapProperties waterRoomProperties = TilemapAnalyser.GenerateProperties(waterGrid);

        Grid fireGrid = structureToGridMapper.structureTypeToGridDict[StructureType.FireRoom];
        TilemapProperties fireRoomProperties = TilemapAnalyser.GenerateProperties(fireGrid);

        Grid airGrid = structureToGridMapper.structureTypeToGridDict[StructureType.AirRoom];
        TilemapProperties airRoomProperties = TilemapAnalyser.GenerateProperties(airGrid);

        Grid earthGrid = structureToGridMapper.structureTypeToGridDict[StructureType.EarthRoom];
        TilemapProperties earthRoomProperties = TilemapAnalyser.GenerateProperties(earthGrid);

        foreach (var dungeonRoom in dungeonRooms)
        {
            WaveFunctionCollapse2 wfc;
            if (dungeonRoom.theme == ElementTheme.Air)
            {
                wfc = new WaveFunctionCollapse2(dungeonRoom.structure.structureTiles, airRoomProperties, 10);
            } else if (dungeonRoom.theme == ElementTheme.Earth) 
            {
                wfc = new WaveFunctionCollapse2(dungeonRoom.structure.structureTiles, earthRoomProperties, 10);
            } else if (dungeonRoom.theme == ElementTheme.Fire)
            {
                wfc = new WaveFunctionCollapse2(dungeonRoom.structure.structureTiles, fireRoomProperties, 10);
            } else
            {
                wfc = new WaveFunctionCollapse2(dungeonRoom.structure.structureTiles, waterRoomProperties, 10);
            }

            wfc.PopulateOutputCells();
        }
    }

    public void PopulateOpenDoorTiles(List<DungeonRoom> dungeonRooms, StructureTypeToGridMapperSO structureToGridMapper)
    {
        structureToGridMapper.structureTypeToGridDict = structureToGridMapper.GetStructureTypeToGridDict();

        Grid waterDoorGrid = structureToGridMapper.structureTypeToGridDict[StructureType.WaterDoor];
        TilemapProperties waterDoorProperties = TilemapAnalyser.GenerateProperties(waterDoorGrid);

        Grid fireDoorGrid = structureToGridMapper.structureTypeToGridDict[StructureType.FireDoor];
        TilemapProperties fireDoorProperties = TilemapAnalyser.GenerateProperties(fireDoorGrid);

        Grid airDoorGrid = structureToGridMapper.structureTypeToGridDict[StructureType.AirDoor];
        TilemapProperties airDoorProperties = TilemapAnalyser.GenerateProperties(airDoorGrid);

        Grid earthDoorGrid = structureToGridMapper.structureTypeToGridDict[StructureType.EarthDoor];
        TilemapProperties earthDoorProperties = TilemapAnalyser.GenerateProperties(earthDoorGrid);

        foreach (var dungeonRoom in dungeonRooms)
        {
            foreach (var door in dungeonRoom.doorways)
            {
                WaveFunctionCollapse2 wfc;
                if (dungeonRoom.theme == ElementTheme.Air)
                {
                    wfc = new WaveFunctionCollapse2(door.structure.structureTiles, airDoorProperties, 1);
                }
                else if (dungeonRoom.theme == ElementTheme.Earth)
                {
                    wfc = new WaveFunctionCollapse2(door.structure.structureTiles, earthDoorProperties, 1);
                }
                else if (dungeonRoom.theme == ElementTheme.Fire)
                {
                    wfc = new WaveFunctionCollapse2(door.structure.structureTiles, fireDoorProperties, 1);
                }
                else
                {
                    wfc = new WaveFunctionCollapse2(door.structure.structureTiles, waterDoorProperties, 1);
                }
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
                    WaveFunctionCollapse2 platformWFC = new WaveFunctionCollapse2(connector.platform.structure.structureTiles, platformProperties, 1);
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
            StaticEventHandler.CallOpenRoomDoors(room);
        }
    }

    public void Clear()
    {
        for (int i = dungeonParent.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = dungeonParent.transform.GetChild(i);

            if (child.name != "Grid" && child.name != "Environment")
            {
                Destroy(child.gameObject);
            }
        }
        if (dungeonLayers)
        {
            dungeonLayers.collisionTilemap.ClearAllTiles();
        }
    }
}
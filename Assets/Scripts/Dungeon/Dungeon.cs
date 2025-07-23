using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Dungeon
{
    public List<DungeonRoom> dungeonRooms;
    public List<Connector> connectors;
    public DungeonLayers dungeonLayers;
    public HashSet<Vector2Int> dungeonFloorPositions;
    public BoundsInt bounds;
    public int seed;

    public Dungeon (List<DungeonRoom> dungeonRooms, List<Connector> connectors, DungeonLayers dungeonLayers, int seed)
    {
        this.dungeonRooms = dungeonRooms;
        this.connectors = connectors;
        this.dungeonLayers = dungeonLayers;
        this.seed = seed;

        this.dungeonFloorPositions = SetDungeonFloorPositions(dungeonRooms, connectors);
        this.bounds = GetDungeonBounds(dungeonRooms);
    }

    private HashSet<Vector2Int> SetDungeonFloorPositions(List<DungeonRoom> dungeonRooms, List<Connector> connectors)
    {
        HashSet<Vector2Int> allFloorPositions = new HashSet<Vector2Int>();

        foreach (var dungeonRoom in dungeonRooms)
        {
            allFloorPositions.UnionWith(dungeonRoom.floorPositions);
        }

        foreach (var connector in connectors)
        {
            if (connector.isStraight)
            {
                if (connector.bridgeMain != null)
                    allFloorPositions.UnionWith(connector.bridgeMain.floorPositions);
            }
            else
            {
                if (connector.bridgeStart != null)
                    allFloorPositions.UnionWith(connector.bridgeStart.floorPositions);
                if (connector.platform != null)
                    allFloorPositions.UnionWith(connector.platform.floorPositions);
                if (connector.bridgeEnd != null)
                    allFloorPositions.UnionWith(connector.bridgeEnd.floorPositions);
            }
        }

        return allFloorPositions;
    }

    public DungeonRoom GetStartRoom()
    {
        foreach (var dungeonRoom in dungeonRooms)
        {
            if (dungeonRoom.roomType == RoomType.Start)
            {
                return dungeonRoom;
            }
        }
        return null;
    }

    public static BoundsInt GetDungeonBounds(List<DungeonRoom> dungeonRooms)
    {
        if (dungeonRooms == null || dungeonRooms.Count == 0)
            return new BoundsInt();

        // Start with the bounds of the first room
        BoundsInt combinedBounds = dungeonRooms[0].outerBounds;

        foreach (var dungeonRoom in dungeonRooms)
        {
            BoundsInt bounds = dungeonRoom.outerBounds;

            // Expand min
            int minX = Mathf.Min(combinedBounds.xMin, bounds.xMin);
            int minY = Mathf.Min(combinedBounds.yMin, bounds.yMin);

            // Expand max
            int maxX = Mathf.Max(combinedBounds.xMax, bounds.xMax);
            int maxY = Mathf.Max(combinedBounds.yMax, bounds.yMax);

            // Recalculate combined size
            Vector3Int newMin = new Vector3Int(minX, minY, 0);
            Vector3Int newSize = new Vector3Int(maxX - minX, maxY - minY, 1);

            combinedBounds = new BoundsInt(newMin, newSize);
        }

        return combinedBounds;
    }
}
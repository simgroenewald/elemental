using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Dungeon
{
    public List<DungeonRoom> dungeonRooms;
    public List<Connector> connectors;
    public DungeonLayers dungeonLayers;
    public int seed;

    public DungeonRoom GetStartRoom()
    {
        foreach (var room in dungeonRooms)
        {
            if (room.roomType == RoomType.Start)
            {
                return room;
            }
        }
        return null;
    }
}
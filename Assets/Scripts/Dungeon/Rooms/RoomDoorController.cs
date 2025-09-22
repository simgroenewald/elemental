using System;
using UnityEngine;

public class RoomDoorController : MonoBehaviour
{
    private DungeonRoom dungeonRoom;

    private void Awake()
    {
        dungeonRoom = GetComponent<DungeonRoom>();
    }

    private void OnEnable()
    {
        StaticEventHandler.OnCloseRoomDoors += OnCloseRoomDoors;
        StaticEventHandler.OnOpenRoomDoors += OnOpenRoomDoors;
        StaticEventHandler.OnCloseRoomExitDoors += OnCloseRoomExitDoors;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnCloseRoomDoors -= OnCloseRoomDoors;
        StaticEventHandler.OnOpenRoomDoors -= OnOpenRoomDoors;
        StaticEventHandler.OnCloseRoomExitDoors -= OnCloseRoomExitDoors;
    }

    public void OnOpenRoomDoors(RoomDisplayEventArgs roomDisplayEventArgs)
    {
        if (roomDisplayEventArgs.room == dungeonRoom)
        {
            foreach (var door in dungeonRoom.doorways)
            {
                if (GameManager.Instance.state != GameState.bossRoom && door.isBossRoomDoorway)
                {
                    door.Close(dungeonRoom.structure.tilemapLayers);
                    continue;
                }
                door.Open(dungeonRoom.structure.tilemapLayers);
            }
        }
    }

    public void OnCloseRoomDoors(RoomDisplayEventArgs roomDisplayEventArgs)
    {
        if (roomDisplayEventArgs.room == dungeonRoom)
        {
            foreach (var door in dungeonRoom.doorways)
            {
                door.Close(dungeonRoom.structure.tilemapLayers);
            }
        }
    }

    public void OnCloseRoomExitDoors(RoomDisplayEventArgs roomDisplayEventArgs)
    {
        if (roomDisplayEventArgs.room == dungeonRoom)
        {
            foreach (var door in dungeonRoom.exitDoorways)
            {
                door.Close(dungeonRoom.structure.tilemapLayers);
            }
        }

    }
}

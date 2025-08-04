using System.Collections.Generic;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public class DungeonNavigationDisplayController : MonoBehaviour
{
    private List<DungeonRoom> allRooms;
    private List<Connector> allConnectors;

    public void Initialise(List<DungeonRoom> rooms, List<Connector> connectors)
    {
        allRooms = rooms;
        allConnectors = connectors;
    }

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += RoomChanged;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= RoomChanged;
    }

    public void RoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        DungeonRoom dungeonRoom = roomChangedEventArgs.room;
        // Player exited room
        if (GameManager.Instance.GetCurrentRoom() == dungeonRoom)
        {
            GameManager.Instance.SetPreviousRoom(dungeonRoom);
            GameManager.Instance.SetCurrentRoom(null);
        }
        // Player entered room
        else if (GameManager.Instance.GetCurrentRoom() == null)
        {
            Debug.Log("Player entered room");
            GameManager.Instance.SetCurrentRoom(dungeonRoom);
                        
            if (!dungeonRoom.isComplete)
            {
                NewRoomEntered(dungeonRoom);
            }
            dungeonRoom.EnterRoom();
        }
    }

    public void NewRoomEntered(DungeonRoom dungeonRoom)
    {
        GameManager.Instance.SetCurrentRoom(dungeonRoom);

        foreach (var room in allRooms)
        {
            if (room != dungeonRoom)
            {
                StaticEventHandler.CallRoomFadeOutEvent(room);
            }
        }

        foreach (var connector in allConnectors)
        {
            StaticEventHandler.CallConnectorFadeOutEvent(connector);
        }

        StaticEventHandler.CallCloseRoomDoors(dungeonRoom);
    }

    public void CompleteRoom(DungeonRoom dungeonRoom)
    {
        dungeonRoom.CompleteRoom();
        GameManager.Instance.AppendCompletedDungeonRooms(dungeonRoom);
        List<DungeonRoom> completedRooms = GameManager.Instance.GetCompletedDungeonRooms();
        foreach (var room in completedRooms)
        {
            StaticEventHandler.CallRoomFadeInEvent(room);
            StaticEventHandler.CallOpenRoomDoors(room);
            foreach (var completedRoom in completedRooms)
            {
                foreach (var childRoom in completedRoom.children)
                {
                    // New available rooms - not completed
                    if (!completedRooms.Contains(childRoom))
                    {
                        StaticEventHandler.CallRoomFadeInEvent(childRoom);
                        StaticEventHandler.CallCloseRoomExitDoors(childRoom);
                    }
                }
            }
        }

        foreach (var connector in allConnectors)
        {
            if (completedRooms.Contains(connector.parentRoom) || completedRooms.Contains(connector.childRoom))
            {
                StaticEventHandler.CallConnectorFadeInEvent(connector);
            }
        }
    }
}

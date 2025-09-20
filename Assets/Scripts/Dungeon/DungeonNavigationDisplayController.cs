using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DungeonNavigationDisplayController : MonoBehaviour
{
    private List<DungeonRoom> allRooms;
    private List<Connector> allConnectors;
    [SerializeField] private TextMeshProUGUI currentRoomUI;

    public void Initialise(List<DungeonRoom> rooms, List<Connector> connectors)
    {
        allRooms = rooms;
        allConnectors = connectors;
    }

    private void OnEnable()
    {
        StaticEventHandler.OnRoomEntered += RoomEntered;
        StaticEventHandler.OnRoomExited += RoomExited;
        StaticEventHandler.OnRoomComplete += CompleteRoom;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomEntered -= RoomEntered;
        StaticEventHandler.OnRoomExited -= RoomExited;
        StaticEventHandler.OnRoomComplete -= CompleteRoom;
    }

    public void RoomEntered(RoomChangedEventArgs roomChangedEventArgs)
    {
        DungeonRoom dungeonRoom = roomChangedEventArgs.room;
        currentRoomUI.SetText($"{dungeonRoom.roomType} {dungeonRoom.theme}");

        if (GameManager.Instance.GetCurrentRoom() == null)
        {
            Debug.Log("Player entered room");
            GameManager.Instance.SetCurrentRoom(dungeonRoom);
                        
            if (!dungeonRoom.isComplete)
            {
                NewRoomEntered(dungeonRoom);
                Debug.Log("New Room entered");
            }
            dungeonRoom.EnterRoom();
        }
    }

    public void RoomExited(RoomChangedEventArgs roomChangedEventArgs)
    {
        DungeonRoom dungeonRoom = roomChangedEventArgs.room;
        currentRoomUI.SetText("Exploring");
        // Player exited room
        if (GameManager.Instance.GetCurrentRoom() == dungeonRoom)
        {
            GameManager.Instance.SetPreviousRoom(dungeonRoom);
            GameManager.Instance.SetCurrentRoom(null);
            Debug.Log("Player Exit Room");
        }
    }

    public void NewRoomEntered(DungeonRoom dungeonRoom)
    {
        if (dungeonRoom.roomType == RoomType.Boss || dungeonRoom.roomType == RoomType.MiniBoss)
        {
            MusicManager.Instance.PlayMusic(dungeonRoom.battleMusic, 0.2f, 2f);
        }
        else
        {
            MusicManager.Instance.PlayMusic(dungeonRoom.ambientMusic, 0.2f, 2f);
        }
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

    public void CompleteRoom(RoomChangedEventArgs roomChangedEventArgs)
    {
        DungeonRoom dungeonRoom = roomChangedEventArgs.room;
        MusicManager.Instance.PlayMusic(dungeonRoom.ambientMusic, 0.2f, 2f);
        GameManager.Instance.AppendCompletedDungeonRooms(dungeonRoom);
        List<DungeonRoom> completedRooms = GameManager.Instance.GetCompletedDungeonRooms();
        foreach (var completedRoom in completedRooms)
        {
            StaticEventHandler.CallRoomFadeInEvent(completedRoom);
            StaticEventHandler.CallOpenRoomDoors(completedRoom);
            foreach (var childRoom in completedRoom.children)
            {
                // New available rooms - not completed
                if (!completedRooms.Contains(childRoom))
                {
                    StaticEventHandler.CallRoomFadeInEvent(childRoom);
                    StaticEventHandler.CallOpenRoomDoors(childRoom);
                    StaticEventHandler.CallCloseRoomExitDoors(childRoom);
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

using UnityEngine;
using System;
using UnityEditor.MemoryProfiler;

public static class StaticEventHandler
{
    // Room changed events
    public static event Action<RoomChangedEventArgs> OnRoomChanged;

    // Room Light events
    public static event Action<RoomDisplayEventArgs> OnRoomFadeIn;
    public static event Action<RoomDisplayEventArgs> OnRoomFadeOut;
    // Room Door events
    public static event Action<RoomDisplayEventArgs> OnCloseRoomDoors;
    public static event Action<RoomDisplayEventArgs> OnOpenRoomDoors;
    public static event Action<RoomDisplayEventArgs> OnCloseRoomExitDoors;
    // Connector events
    public static event Action<ConnectorDisplayEventArgs> OnConnectorFadeIn;
    public static event Action<ConnectorDisplayEventArgs> OnConnectorFadeOut;

    // Room Changed events
    internal static void CallRoomChangedEvent(DungeonRoom dungeonRoom)
    {
        OnRoomChanged?.Invoke(new RoomChangedEventArgs() { room = dungeonRoom });
    }

    // Room Light events
    internal static void CallRoomFadeInEvent(DungeonRoom dungeonRoom)
    {
        OnRoomFadeIn?.Invoke(new RoomDisplayEventArgs() { room = dungeonRoom });
    }
    internal static void CallRoomFadeOutEvent(DungeonRoom dungeonRoom)
    {
        OnRoomFadeOut?.Invoke(new RoomDisplayEventArgs() { room = dungeonRoom });
    }

    // Room door events
    internal static void CallOpenRoomDoors(DungeonRoom dungeonRoom)
    {
        OnOpenRoomDoors?.Invoke(new RoomDisplayEventArgs() { room = dungeonRoom });
    }
    internal static void CallCloseRoomDoors(DungeonRoom dungeonRoom)
    {
        OnCloseRoomDoors?.Invoke(new RoomDisplayEventArgs() { room = dungeonRoom });
    }
    internal static void CallCloseRoomExitDoors(DungeonRoom childRoom)
    {
        OnCloseRoomExitDoors?.Invoke(new RoomDisplayEventArgs() { room = childRoom });
    }

    // Connector Light events
    internal static void CallConnectorFadeInEvent(Connector connector)
    {
        OnConnectorFadeIn?.Invoke(new ConnectorDisplayEventArgs() { connector = connector });
    }

    internal static void CallConnectorFadeOutEvent(Connector connector)
    {
        OnConnectorFadeOut?.Invoke(new ConnectorDisplayEventArgs() { connector = connector });
    }
}

public class RoomChangedEventArgs : EventArgs
{
    public DungeonRoom room;
}

public class RoomDisplayEventArgs : EventArgs
{
    public DungeonRoom room;
}

public class ConnectorDisplayEventArgs : EventArgs
{
    public Connector connector;
}
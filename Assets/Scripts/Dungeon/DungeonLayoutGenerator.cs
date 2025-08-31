using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class DungeonLayoutGenerator : MonoBehaviour
{
    public float spacing = 5f;
    public List<DungeonRoom> allRooms = new();
    [SerializeField] private RoomSizePresetsSO roomSizePresets;
    [SerializeField] private DungeonRoomFactory roomFactory;
    [SerializeField] private Transform objectParent;

    public DungeonLayoutGenerator(RoomSizePresetsSO roomSizePresets)
    {
        this.roomSizePresets = roomSizePresets;
}

    public List<DungeonRoom> GenerateDungeonLayout(LevelSettingSO level)
    {
        allRooms = new List<DungeonRoom>();

        // 1. Create Start Room
        DungeonRoom start = CreateRoom(RoomType.Start, Vector2.zero);
        start.outerBounds = new BoundsInt(new Vector3Int(0, 0, 0), new Vector3Int(roomSizePresets.Large.width, roomSizePresets.Large.height, 0));
        allRooms.Add(start);

        //int targetRoomCount = GetRoomCountForLevel(level);

        // 2. Create Normal Rooms
        for (int i = 1; i < level.roomCount - 1; i++)
        {
            DungeonRoom newRoom = CreateRoom(RoomType.Normal, Vector2.zero); // position will be adjusted
            DungeonRoom parent = FindValidParent();

            if (parent != null)
            {
                newRoom.parent = parent;
                //parent.children.Add(newRoom);
                SetNewConnections(parent, newRoom);
                allRooms.Add(newRoom);
            }
            else
            {
                Debug.LogWarning("No valid parent found. Skipping room.");
            }
        }

        // 3. Place Boss Room (as a leaf)
        DungeonRoom bossRoom = CreateRoom(RoomType.Boss, Vector2.zero);
        DungeonRoom bossParent = FindValidParent();
        if (bossParent != null)
        {
            bossRoom.parent = bossParent;
            //bossParent.children.Add(bossRoom);
            SetNewConnections(bossParent, bossRoom);
            allRooms.Add(bossRoom);
        }

        // 4. Place Mini-Boss Room
        FindAndAssignMiniBossRoom();

        // 5. Assign Room Sizes
        AssignRoomSizes(level);

        // 6. Set Room Bounds
        SetRoomBounds();

        PrintRooms();

        return allRooms;
    }

    public void PrintRooms()
    {
        foreach (var room in allRooms)
        {
            room.displayInfo();
        }
    }

    DungeonRoom CreateRoom(RoomType type, Vector2 pos)
    {
        return roomFactory.CreateRoom(type, pos, objectParent);
    }

    DungeonRoom FindValidParent()
    {
        int attempts = 100;
        while (attempts-- > 0)
        {
            DungeonRoom candidate = allRooms[Random.Range(0, allRooms.Count)];
            if (candidate.CanHaveMoreChildren)
            {
                return candidate;
            }
        }
        return null;
    }

    private bool SetNewConnections(DungeonRoom parent, DungeonRoom newRoom)
    {
        List<Direction> available = GetAvailableConnections(parent.connections);
        ShuffleList(available); // Add randomness

        if (available.Count > 0)
        {
            foreach (var dir in available)
            {
                Vector2 offset = DirectionToOffset(dir);
                Vector2 candidatePosition = parent.nodeGraphPosition + offset * 5f;

                if (!IsPositionOccupied(candidatePosition))
                {
                    newRoom.nodeGraphPosition = candidatePosition;

                    parent.connections[dir] = newRoom;
                    newRoom.connections[GetOppositeDirection(dir)] = parent;

                    newRoom.parent = parent;
                    newRoom.outerBounds = GetOuterBounds(dir, parent, newRoom);
                    parent.children.Add(newRoom);
                    return true;
                }
            }
        }
        return false;
    }

    private BoundsInt GetOuterBounds(Direction dir, DungeonRoom parent, DungeonRoom room)
    {
        Vector3Int position = parent.outerBounds.position;

        if (dir == Direction.North)
        {
            position.y += (roomSizePresets.Large.height + 7);
        }
        if (dir == Direction.South)
        {
            position.y -= (roomSizePresets.Large.height + 7);
        }
        if (dir == Direction.East)
        {
            position.x += (roomSizePresets.Large.width + 7);
        }
        if (dir == Direction.West)
        {
            position.x -= (roomSizePresets.Large.width + 7);
        }

        BoundsInt outerBounds = new BoundsInt(position, new Vector3Int(roomSizePresets.Large.width, roomSizePresets.Large.height, 0));

        return outerBounds;
    }

    private List<Direction> GetAvailableConnections(Dictionary<Direction, DungeonRoom> connections)
    {
        List<Direction> available = new();

        foreach (Direction dir in System.Enum.GetValues(typeof(Direction)))
        {
            if (!connections.ContainsKey(dir))
            {
                available.Add(dir);
            }
        }

        return available;
    }

    public static Vector2 DirectionToOffset(Direction dir)
    {
        return dir switch
        {
            Direction.North => new Vector2(0, 1),
            Direction.South => new Vector2(0, -1),
            Direction.East => new Vector2(1, 0),
            Direction.West => new Vector2(-1, 0),
            _ => Vector2.zero
        };
    }

    public static Direction GetOppositeDirection(Direction dir)
    {
        return dir switch
        {
            Direction.North => Direction.South,
            Direction.South => Direction.North,
            Direction.East => Direction.West,
            Direction.West => Direction.East,
            _ => dir
        };
    }

    void FindAndAssignMiniBossRoom()
    {
        // Priority 1: Find a clean leaf room that's not Start or Boss
        foreach (var room in allRooms)
        {
            if (room.IsLeaf && room.roomType == RoomType.Normal)
            {
                room.roomType = RoomType.MiniBoss;
                room.UpdateObjectName();
                return;
            }
        }

        // Fallback: any eligible normal room
        foreach (var room in allRooms)
        {
            if (room.roomType == RoomType.Normal && room.parent.roomType != RoomType.Start)
            {
                room.roomType = RoomType.MiniBoss;
                room.UpdateObjectName();
                return;
            }
        }

        Debug.LogWarning("No suitable room found to assign as MiniBoss.");
    }

    private bool IsPositionOccupied(Vector2 position)
    {
        foreach (var room in allRooms)
        {
            if (room.nodeGraphPosition == position)
                return true;
        }
        return false;
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    // Dont remove size counts from the actual level object - find a better way to do this
    public void AssignRoomSizes(LevelSettingSO level)
    {
        List<DungeonRoom> normalRooms = new();
        List<RoomSizeSO> sizePool = new();
        level.roomSizeCountDict = level.GetRoomSizeCountDict();

        // Assign fixed sizes to special room types
        foreach (var room in allRooms)
        {
            RoomSizeSO fixedSize = GetFixedSize(room.roomType);
            if (fixedSize.width > 0)
            {
                room.bounds.size = new Vector3Int(fixedSize.width, fixedSize.height, 0);
                room.subRoomMinWidth = fixedSize.subRoomMinWidth;
                room.subRoomMinHeight = fixedSize.subRoomMinHeight;

                // Subtract from sizeCounts if that size is limited
                if (level.roomSizeCountDict.ContainsKey(fixedSize))
                {
                    level.roomSizeCountDict[fixedSize]--;
                    if (level.roomSizeCountDict[fixedSize] <= 0) level.roomSizeCountDict.Remove(fixedSize);
                }
            }
            else
            {
                normalRooms.Add(room);
            }
        }

        // Populate pool for remaining normal rooms
        foreach (var kvp in level.roomSizeCountDict)
        {
            for (int i = 0; i < kvp.Value; i++)
            {
                sizePool.Add(kvp.Key);
            }
        }

        // Shuffle to randomize assignment
        for (int i = sizePool.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (sizePool[i], sizePool[j]) = (sizePool[j], sizePool[i]);
        }

        for (int i = 0; i < normalRooms.Count && i < sizePool.Count; i++)
        {
            normalRooms[i].bounds.size = new Vector3Int(sizePool[i].width, sizePool[i].height, 0);
            normalRooms[i].subRoomMinWidth = sizePool[i].subRoomMinWidth;
            normalRooms[i].subRoomMinHeight = sizePool[i].subRoomMinHeight;
        }

        if (normalRooms.Count > sizePool.Count)
        {
            Debug.LogWarning("Not enough room sizes for all normal rooms. Some will have default size.");
        }
    }

    public RoomSizeSO GetFixedSize(RoomType type)
    {
        return type switch
        {
            RoomType.Start => roomSizePresets.Small,
            RoomType.Boss => roomSizePresets.Large,
            RoomType.MiniBoss => UnityEngine.Random.value < 0.5f ? roomSizePresets.Medium : roomSizePresets.Large,
            _ => new RoomSizeSO() // Default, for normal rooms
        };
    }

    public void SetRoomBounds()
    {
        foreach (var room in allRooms)
        {
            Vector3Int position = new Vector3Int(room.outerBounds.position.x + (room.outerBounds.size.x - room.bounds.size.x) / 2, room.outerBounds.position.y + (room.outerBounds.size.y - room.bounds.size.y) / 2, 0);

            if (room.connections.ContainsKey(Direction.East))
            {
                position.x += (room.outerBounds.size.x - room.bounds.size.x) / 2;
            }
            if (room.connections.ContainsKey(Direction.West))
            {
                position.x -= (room.outerBounds.size.x - room.bounds.size.x) / 2;
            }
            if (room.connections.ContainsKey(Direction.North))
            {
                position.y += (room.outerBounds.size.y - room.bounds.size.y) / 2;
            }
            if (room.connections.ContainsKey(Direction.South))
            {
                position.y -= (room.outerBounds.size.y - room.bounds.size.y) / 2;
            }

            room.bounds.position = position;
        }
    }
}

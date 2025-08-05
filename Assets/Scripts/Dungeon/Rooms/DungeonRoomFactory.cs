using UnityEngine;
using static UnityEditor.PlayerSettings;

public class DungeonRoomFactory : MonoBehaviour
{
    public GameObject dungeonRoomPrefab;

    public DungeonRoom CreateRoom(RoomType type, Vector2 position, Transform objectParent)
    {
        GameObject roomObject = GameObject.Instantiate(dungeonRoomPrefab, objectParent);
        DungeonRoom room = roomObject.GetComponent<DungeonRoom>();
        room.Initialise(type, position); // Or other setup logic
        return room;
    }
}

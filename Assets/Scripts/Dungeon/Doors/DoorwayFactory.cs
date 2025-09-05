using UnityEditorInternal;
using UnityEngine;

public class DoorwayFactory : MonoBehaviour
{
    public GameObject doorwayPrefab;

    public Doorway CreateDoorway(Vector2Int midPosition, int width, DoorType doorType, DungeonRoom room, bool isBossRoomDoorway)
    {
        GameObject doorwayObject = GameObject.Instantiate(doorwayPrefab, new Vector3(midPosition.x, midPosition.y, 0), Quaternion.identity, room.transform);
        Doorway doorway = doorwayObject.GetComponent<Doorway>();
        doorway.Initialise(midPosition, width, doorType, room, room.structure.tilemapLayers.grid, isBossRoomDoorway);
        return doorway;
    }
}

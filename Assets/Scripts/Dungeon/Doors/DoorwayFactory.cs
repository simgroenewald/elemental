using UnityEditorInternal;
using UnityEngine;

public class DoorwayFactory : MonoBehaviour
{
    public GameObject doorwayPrefab;

    public Doorway CreateDoorway(Vector2Int midPosition, int width, DoorType doorType, Transform objectParent, Grid grid)
    {
        GameObject doorwayObject = GameObject.Instantiate(doorwayPrefab, new Vector3(midPosition.x, midPosition.y, 0), Quaternion.identity, objectParent);
        Doorway doorway = doorwayObject.GetComponent<Doorway>();
        doorway.Initialise(midPosition, width, doorType, grid);
        return doorway;
    }
}

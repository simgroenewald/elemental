using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    public Transform cameraTarget; // Assign the CameraTarget GameObject
    public float scrollSpeed = 5f;
    public float borderThickness = 10f;
    public float edgeScrollThreshold = 10f;
    private Bounds cameraBounds;
    private bool isCameraSet = false;
    private Dungeon dungeon;

    public void Awake()
    {
        GameResources.Instance.cameraController = this;
    }

    public void SetupCamera(Vector3 startPosition, Tilemap tilemap)
    {
        //cameraBounds = new Bounds(dungeon.bounds.center, dungeon.bounds.size);
        ClampCamera(tilemap);
        cameraTarget.position = new Vector3(startPosition.x, startPosition.y, cameraTarget.position.z);
        isCameraSet = true;
    }

    public void ClampCamera(Tilemap tilemap)
    {
        BoundsInt tileBounds = tilemap.cellBounds;
        Vector3 min = tilemap.CellToWorld(tileBounds.min);
        Vector3 max = tilemap.CellToWorld(tileBounds.max);

        // Optional: Adjust bounds to center and size
        Bounds worldBounds = new Bounds();
        worldBounds.SetMinMax(min, max);

        cameraBounds = worldBounds;
    }

    void Update()
    {
        if (isCameraSet)
        {
            Vector3 direction = Vector3.zero;

            if (Input.mousePosition.x >= Screen.width - edgeScrollThreshold)
                direction.x += 1;
            if (Input.mousePosition.x <= edgeScrollThreshold)
                direction.x -= 1;
            if (Input.mousePosition.y >= Screen.height - edgeScrollThreshold)
                direction.y += 1;
            if (Input.mousePosition.y <= edgeScrollThreshold)
                direction.y -= 1;

            Vector3 newTargetPosition = cameraTarget.position + direction.normalized * scrollSpeed * Time.deltaTime;

            // Clamp to bounds
            newTargetPosition = new Vector3(
                Mathf.Clamp(newTargetPosition.x, cameraBounds.min.x, cameraBounds.max.x),
                Mathf.Clamp(newTargetPosition.y, cameraBounds.min.y, cameraBounds.max.y),
                cameraTarget.position.z
            );

            cameraTarget.position = newTargetPosition;
        }
    }
}
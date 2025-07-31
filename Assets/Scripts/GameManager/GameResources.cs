using UnityEngine;
using UnityEngine.Tilemaps;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;

    public static GameResources Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GameResources>("GameResources");
            }
            return instance;
        }
    }

    public Dungeon dungeon;
    public PlayerSO player;
    public TileBase[] enemyUnwalkableCollisionTilesArray;
    public TileBase preferredEnemyPathTile;
    public CameraController cameraController;
    public GameObject structureTemplatePrefab;
}

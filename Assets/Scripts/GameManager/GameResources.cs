using UnityEngine;
using UnityEngine.Audio;
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
    public AudioMixerGroup soundsMasterMixerGroup;
    public AudioMixerGroup musicMasterMixerGroup;
    public AudioMixerSnapshot musicOnFullSnapshot;
    public AudioMixerSnapshot musicLowSnapshot;
    public AudioMixerSnapshot musicOffSnapshot;

    // Materials - Fading rooms
    public Material dimmedMaterial;
    public Material litMaterial;
    public Shader variableLitShader;
}

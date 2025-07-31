using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapLayers : MonoBehaviour
{
    public Grid grid;

    [Header("Tilemaps")]
    public Tilemap baseTilemap;
    public Tilemap baseDecorationTilemap;
    public Tilemap environmentDecorationTilemap;
    public Tilemap platformTilemap;
    public Tilemap platformDecorationTilemap;
    public Tilemap bridgeTilemap;
    public Tilemap frontTilemap;
    public Tilemap frontDecorationTilemap;
    public Tilemap collisionTilemap;
    public Tilemap minimap;

    public Transform environment;
}
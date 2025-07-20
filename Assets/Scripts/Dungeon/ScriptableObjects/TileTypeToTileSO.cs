using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileTypeToTile", menuName = "Scriptable Objects/TileTypeToTile")]
public class TileTypeToTileSO : ScriptableObject
{
    public TileType tileType;
    public Tile tile;

}
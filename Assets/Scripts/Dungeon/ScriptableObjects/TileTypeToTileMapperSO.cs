using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileTypeToTileMapper", menuName = "Scriptable Objects/TileTypeToTileMapper")]
public class TileTypeToTileMapperSO : ScriptableObject
{
    public List<TileTypeToTileSO> tileTypeToTileList;
    public Dictionary<TileType, Tile> tileTypeToTileDict;

    public Dictionary<TileType, Tile> GetTileTypeToTileDict()
    {
        var dict = new Dictionary<TileType, Tile>();
        foreach (var entry in tileTypeToTileList)
        {
            dict[entry.tileType] = entry.tile;
        }
        return dict;
    }
}
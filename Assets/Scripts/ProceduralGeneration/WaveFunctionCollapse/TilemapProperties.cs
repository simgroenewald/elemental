using System.Collections.Generic;
using UnityEngine;

public class TilemapProperties
{
    public List<int> TileIndices { get; private set; }              // Flat tile index array (row-major)
    public int Width { get; private set; }
    public int Height { get; private set; }
    public BoundsInt mapBounds { get; private set; }
    public Dictionary<int, Cell> IndexToCell { get; private set; }  // Index -> Cell data
    public Dictionary<Vector2Int, int> PositionToIndex { get; private set; }  // Index -> Cell data

    public Dictionary<int, int> IndexToFrequency = new Dictionary<int, int>();  // Index -> Cell frequency

    public TilemapProperties(List<int> tileIndices, int width, int height, Dictionary<int, Cell> indexToCell, BoundsInt bounds, Dictionary<Vector2Int, int> positionToIndex)
    {
        TileIndices = tileIndices;
        Width = width;
        Height = height;
        IndexToCell = indexToCell;
        mapBounds = bounds;
        PositionToIndex = positionToIndex;
        SetIndexToFrequency(TileIndices);
    }

    private void SetIndexToFrequency(List<int> TileIndices)
    {
        foreach (int hash in TileIndices)
        {
            if (!IndexToFrequency.ContainsKey(hash))
                IndexToFrequency[hash] = 0;

            IndexToFrequency[hash]++;
        }
    }

    public int GetIndexFromCoords(int x, int y)
    {
        int index = (y - mapBounds.yMin) * Width + (x - mapBounds.xMin);
        return index;
    }

    public Vector2Int GetCoordsFromIndex(int index)
    {
        int y = index / Width;
        int x = index % Width;
        return new Vector2Int(x, y);
    }
}
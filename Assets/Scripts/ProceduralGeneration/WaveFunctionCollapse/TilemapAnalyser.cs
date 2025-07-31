using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class TilemapAnalyser
{
    public static TilemapProperties GenerateProperties(Grid grid)
    {

        Tilemap baseMap = grid.transform.Find("Base")?.GetComponent<Tilemap>();
        Tilemap frontMap = grid.transform.Find("Front")?.GetComponent<Tilemap>();
        Tilemap baseDecorMap = grid.transform.Find("BaseDecor")?.GetComponent<Tilemap>();
        Tilemap frontDecorMap = grid.transform.Find("FrontDecor")?.GetComponent<Tilemap>();
        Tilemap collisionMap = grid.transform.Find("Collision")?.GetComponent<Tilemap>();
        Tilemap typeMap = grid.transform.Find("TileType")?.GetComponent<Tilemap>();

        baseMap.CompressBounds();
        BoundsInt bounds;

        bounds = typeMap.cellBounds;
        
        int width = bounds.size.x;
        int height = bounds.size.y;


        List<int> tileIndices = new List<int>(width * height);
        Dictionary<int, Cell> indexToCell = new Dictionary<int, Cell>();
        Dictionary<Vector2Int, int> positionToIndex = new Dictionary<Vector2Int, int>();

        for (int y = bounds.yMin; y < bounds.yMax; y++)
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                if (x == -35 && y == 13)
                {
                    Debug.Log("Gotcha");
                }

                Vector3Int pos = new Vector3Int(x, y, 0);

                TileBase tilebase = null;
                TileBase collisionTile = null;
                TileBase baseDecorTile = null;
                TileBase frontDecorTile = null;
                TileBase frontTile = null;
                TileBase typeTile = null;
                if (baseMap)
                {
                    tilebase = baseMap.GetTile(pos);
                }

                if (collisionMap)
                {
                    collisionTile = collisionMap.GetTile(pos);
                }

                if (baseDecorMap)
                {
                    baseDecorTile = baseDecorMap.GetTile(pos);
                }

                if (frontMap)
                {
                    frontTile = frontMap.GetTile(pos);
                }

                if (frontDecorMap)
                {
                    frontDecorTile = frontDecorMap.GetTile(pos);
                }

                if (typeMap)
                {
                    typeTile = typeMap.GetTile(pos);
                }

                //Debug.Log($"Base: {tilebase}");
                //Debug.Log($"Collision: {collisionTile}");
                //Debug.Log($"Decor: {decorTile}");
                //Debug.Log($"Type: {typeTile}");

                if (tilebase == null && baseDecorTile == null && collisionTile == null && frontTile == null && typeTile == null && frontDecorTile == null)
                {
                    continue;
                }

                List<TileBase> layerTiles = new List<TileBase> { tilebase, baseDecorTile, frontTile, frontDecorTile, typeTile };
                int key = GetKey(layerTiles);
                //Debug.Log("Key: " + key);

                // Assign or retrieve index
                if (!indexToCell.ContainsKey(key))
                {
                    //Debug.Log("Adding cell");
                    indexToCell[key] = new Cell(tilebase, frontTile, baseDecorTile, frontDecorTile, collisionTile, typeTile, key, new Vector2Int(x, y));
                }

                // Store tile index in flattened array (row-major order)
                //Debug.Log("Adding key");
                tileIndices.Add(key);
                if (x == -38 && y == 0)
                {
                    Debug.Log("Gotcha");
                }
                positionToIndex[new Vector2Int(x, y)] = key;
            }
        }
        return new TilemapProperties(tileIndices, width, height, indexToCell, bounds, positionToIndex);
    }

    private static int GetKey(List<TileBase> tiles)
    {
        unchecked
        {
            //Debug.Log("Getting key");
            int hash = 17;
            foreach (var tile in tiles)
            {
                //Debug.Log("Looping: " + tile);
                hash = hash * 31 + (tile != null ? tile.name.GetHashCode() : 0);
            }
            return hash;
        }
    }

    private static void UpdateCellEdgeAndCorners(Tilemap tilemap, Cell cell, int x, int y)
    {
        // Determine if this position is on the edge or corner
        bool leftEmpty = tilemap.GetTile(new Vector3Int(x - 1, y, 0)) == null;
        bool rightEmpty = tilemap.GetTile(new Vector3Int(x + 1, y, 0)) == null;
        bool topEmpty = tilemap.GetTile(new Vector3Int(x, y + 1, 0)) == null;
        bool bottomEmpty = tilemap.GetTile(new Vector3Int(x, y - 1, 0)) == null;

        // Mark as edge
        if (topEmpty)
        {
            cell.IsEdge = true;
            cell.EdgeType = EdgeType.TopEdge;
        }
        else if (bottomEmpty)
        {
            cell.IsEdge = true;
            cell.EdgeType = EdgeType.BottomEdge;
        }
        else if (leftEmpty)
        {
            cell.IsEdge = true;
            cell.EdgeType = EdgeType.LeftEdge;
        }
        else if (rightEmpty)
        {
            cell.IsEdge = true;
            cell.EdgeType = EdgeType.RightEdge;
        }

        // Mark as corner
        if ((topEmpty && leftEmpty))
        {
            cell.IsCorner = true;
            cell.CornerType = CornerType.TopLeft;
        }
        else if ((topEmpty && rightEmpty))
        {
            cell.IsCorner = true;
            cell.CornerType = CornerType.TopRight;
        }
        else if ((bottomEmpty && leftEmpty))
        {
            cell.IsCorner = true;
            cell.CornerType = CornerType.BottomLeft;
        }
        else if ((bottomEmpty && rightEmpty))
        {
            cell.IsCorner = true;
            cell.CornerType = CornerType.BottomRight;
        }
    }
}
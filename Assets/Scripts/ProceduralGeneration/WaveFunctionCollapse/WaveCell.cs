using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum WFCDirection { Up, Down, Left, Right}

public class WaveCell : IComparable<WaveCell>, IEqualityComparer<WaveCell>
{
    public int CollapsedIndex = -1;
    public Vector2Int Position { get; private set; }
    public HashSet<int> PossibleTiles { get; set; }
    public TileType TileType { get; private set; }
    public float Entropy { get; set; }
    private float SmallEntropyNoise;
    public bool Collapsed = false;

    public WaveCell(Vector2Int position, TileType tileType, float smallEntropyNoise)
    {
        Position = position;
        TileType = tileType;
        PossibleTiles = new HashSet<int>();
        SmallEntropyNoise = smallEntropyNoise;
    }

    public void InitializePossibleTiles(Dictionary<int, Cell> indexToCell, Dictionary<int, int> hashToFrequency)
    {
        Collapsed = false;
        foreach (int tileIndex in indexToCell.Keys)
        {
            if (indexToCell[tileIndex].TileType == TileType)
            {
                PossibleTiles.Add(tileIndex);
            }
        }
        CalculateEntropy(hashToFrequency);
    }

    public void CalculateEntropy(Dictionary<int, int> hashToFrequency)
    {
        if (Collapsed)
        {
            Entropy = 0f;
        }
        else
        {

            Entropy = (PossibleTiles.Count * 0.01f) + GetCertaintyOffset() + SmallEntropyNoise;
/*            int totalFrequency = 0;
            // Calculate total frequency
            foreach (int tile in PossibleTiles)
            {
                totalFrequency += hashToFrequency[tile];
            }

            // Shannon entropy formula
            foreach (int tile in PossibleTiles)
            {
                float p = (float)hashToFrequency[tile] / totalFrequency;
                Entropy -= p * Mathf.Log(p);
            }*/
        }
    }

    public float GetCertaintyOffset()
    {
        List<TileType> walls = new List<TileType> { TileType.WallBack, TileType.WallRight, TileType.WallLeft };
        List<TileType> convexCorners = new List<TileType> { TileType.TopLeftConvex, TileType.TopRightConvex, TileType.BottomLeftConvex, TileType.BottomRightConvex };
        List<TileType> concaveCorners = new List<TileType> { TileType.TopLeftConcave, TileType.TopRightConcave, TileType.BottomLeftConcave, TileType.BottomRightConcave };
        List<TileType> floor = new List<TileType> { TileType.Floor };
        if (walls.Contains(TileType))
        {
            return 1f;
        }
        else if (convexCorners.Contains(TileType) || concaveCorners.Contains(TileType))
        {
            return 2f;
        }
        else if (floor.Contains(TileType))
        {
            return 3f;
        }
        else
        {
            return 0f;
        }
    }

    public int CompareTo(WaveCell other)
    {
        if (Entropy > other.Entropy) return 1;
        else if (Entropy < other.Entropy) return -1;
        else return 0;
    }

    public bool Equals(WaveCell cell1, WaveCell cell2)
    {
        return cell1.Position.x == cell2.Position.x && cell1.Position.y == cell2.Position.y;
    }

    public int GetHashCode(WaveCell obj)
    {
        return obj.GetHashCode();
    }

    public override int GetHashCode()
    {
        return Position.GetHashCode();
    }

    /*public int CompareTo(WaveCell other)
    {
        if (other == null) return 1;

        // Compare by entropy
        int entropyComparison = Entropy.CompareTo(other.Entropy);
        if (entropyComparison != 0) return entropyComparison;

        // Tie-breaker using position directly
        int xCompare = Position.x.CompareTo(other.Position.x);
        if (xCompare != 0) return xCompare;

        return Position.y.CompareTo(other.Position.y);
    }

    public bool Equals(WaveCell x, WaveCell y)
    {
        return x.Position == y.Position;
    }

    public int GetHashCode(WaveCell obj)
    {
        return obj.Position.GetHashCode();
    }*/

    internal static void SetTileValues(TileBase baseTile, TileBase decorTile, TileBase collisionTile, TileBase typeTile)
    {
        throw new NotImplementedException();
    }

    internal WaveCell GetNeigbouringCell(WFCDirection direction, List<WaveCell> outputCells)
    {
        Vector2Int offset = direction switch
        {
            WFCDirection.Up => new Vector2Int(0, 1),
            WFCDirection.Down => new Vector2Int(0, -1),
            WFCDirection.Left => new Vector2Int(-1, 0),
            WFCDirection.Right => new Vector2Int(1, 0),
/*            WFCDirection.DgUpLeft => new Vector2Int(-1, 1),
            WFCDirection.DgUpRight => new Vector2Int(1, 1),
            WFCDirection.DgDownLeft => new Vector2Int(-1, -1),
            WFCDirection.DgDownRight => new Vector2Int(1, -1),*/
            _ => Vector2Int.zero
        };

        Vector2Int neighborPos = Position + offset;

        foreach (var cell in outputCells)
        {
            if (cell.Position == neighborPos)
                return cell;
        }

        return null;
    }

    public static WFCDirection GetOppositeDirection(WFCDirection direction)
    {
        return direction switch
        {
            WFCDirection.Up => WFCDirection.Down,
            WFCDirection.Down => WFCDirection.Up,
            WFCDirection.Left => WFCDirection.Right,
            WFCDirection.Right => WFCDirection.Left,
/*            WFCDirection.DgUpLeft => WFCDirection.DgDownRight,
            WFCDirection.DgUpRight => WFCDirection.DgDownLeft,
            WFCDirection.DgDownLeft => WFCDirection.DgUpRight,
            WFCDirection.DgDownRight => WFCDirection.DgUpLeft,*/
            _ => direction
        };
    }
}
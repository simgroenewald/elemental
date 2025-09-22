using System.Collections.Generic;
using UnityEngine;

public class RulesGenerator
{
    public Dictionary<int, Dictionary<WFCDirection, HashSet<int>>> TileRules;

    public RulesGenerator(TilemapProperties mapProperties)
    {
        TileRules = new Dictionary<int, Dictionary<WFCDirection, HashSet<int>>> { };
        InitialiseRulesDict(mapProperties);
        GenerateRules(mapProperties);
    }

    public void InitialiseRulesDict(TilemapProperties mapProperties)
    {

        foreach (var hash in mapProperties.IndexToCell.Keys)
        {
            TileRules[hash] = new Dictionary<WFCDirection, HashSet<int>>();
            foreach (WFCDirection dir in System.Enum.GetValues(typeof(WFCDirection)))
            {
                TileRules[hash][dir] = new HashSet<int>();
            }
        }
    }
    public void GenerateRules(TilemapProperties mapProperties)
    {
        foreach (var position in mapProperties.PositionToIndex.Keys)
        {

            var hash = mapProperties.PositionToIndex[position];

            TryAddNeighbor(mapProperties, position.x, position.y + 1, hash, WFCDirection.Up);
            TryAddNeighbor(mapProperties, position.x, position.y - 1, hash, WFCDirection.Down);
            TryAddNeighbor(mapProperties, position.x - 1, position.y, hash, WFCDirection.Left);
            TryAddNeighbor(mapProperties, position.x + 1, position.y, hash, WFCDirection.Right);
/*            TryAddNeighbor(mapProperties, position.x - 1, position.y + 1, hash, WFCDirection.DgUpLeft);
            TryAddNeighbor(mapProperties, position.x + 1, position.y + 1, hash, WFCDirection.DgUpRight);
            TryAddNeighbor(mapProperties, position.x - 1, position.y - 1, hash, WFCDirection.DgDownLeft);
            TryAddNeighbor(mapProperties, position.x + 1, position.y - 1, hash, WFCDirection.DgDownRight);*/
        }
        //GenerateEmptyTileRules();
        Debug.Log("Done generating");
    }

    private void TryAddNeighbor(TilemapProperties mapProperties, int x, int y, int currentHash, WFCDirection direction)
    {
        if (x < mapProperties.mapBounds.xMin || y < mapProperties.mapBounds.yMin || x >= mapProperties.mapBounds.xMax || y >= mapProperties.mapBounds.yMax)
        {
            //AddNeighbor(currentHash, direction, 0); // 0 for missing neighbor
            return;
        }

        if (!mapProperties.PositionToIndex.TryGetValue(new Vector2Int(x, y), out var neighbourHash))
        {
            //AddNeighbor(currentHash, direction, 0);
            return;
        }

        AddNeighbor(currentHash, direction, neighbourHash);
    }

    public void AddNeighbor(int hash, WFCDirection dir, int neighborHash)
    {
        TileRules[hash][dir].Add(neighborHash);
    }

    public void GenerateEmptyTileRules()
    {
        TileRules[0] = new Dictionary<WFCDirection, HashSet<int>>();
        foreach (WFCDirection dir in System.Enum.GetValues(typeof(WFCDirection)))
        {
            TileRules[0][dir] = new HashSet<int>();
        }
        foreach (KeyValuePair<int, Dictionary<WFCDirection, HashSet<int>>> ruleSet in TileRules)
        {
            foreach (KeyValuePair<WFCDirection, HashSet<int>> ruleDict in ruleSet.Value)
            {
                if (ruleSet.Value[ruleDict.Key].Contains(0))
                {
                    WFCDirection oppDir = GetOppositeDirection(ruleDict.Key);
                    TileRules[0][oppDir].Add(ruleSet.Key);
                }

            }
        }
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
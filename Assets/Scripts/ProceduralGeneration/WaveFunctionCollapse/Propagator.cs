using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Propagator
{

    public bool hasConflict = false;
    private SortedSet<WaveCell> lowEntropySet = new SortedSet<WaveCell>();
    public HashSet<WaveCell> propagationSet = new HashSet<WaveCell>();
    RulesGenerator rulesGenerator;
    TilemapProperties tilemapProperties;
    List<WaveCell> outputCells;
    public WaveCell conflictCell;

    public Propagator(TilemapProperties tilemapProperties, RulesGenerator rulesGenerator, List<WaveCell> outputCells)
    {
        this.tilemapProperties = tilemapProperties;
        this.rulesGenerator = rulesGenerator;
        this.outputCells = outputCells;
        this.propagationSet = new HashSet<WaveCell>();
        this.conflictCell = null;
    }

    public void PopulateLowEntropySet(List<WaveCell> outputCells)
    {
        foreach (WaveCell outputCell in outputCells)
        {
            lowEntropySet.Add(outputCell);
        }
    }

    public void Propagate(WaveCell waveCell)
    {
        propagationSet.Add(waveCell);
        // Check if there are any collisions
        if (hasConflict)
        {
            // If there are then return
            return;
        }

        // If it isnt then loop through each direction
        foreach (WFCDirection direction in Enum.GetValues(typeof(WFCDirection)))
        {
            if (waveCell.Position == new Vector2Int(135, 35))
            {
                Debug.Log("Gotcha!!");
            }
            // Get the pos of the cell in that direction
            WaveCell compWaveCell = waveCell.GetNeigbouringCell(direction, outputCells);
            // Check if that cell exists
            // Check that cell has not already been updates
            // Check if its collapsed
            if (compWaveCell == null || propagationSet.Contains(compWaveCell) || compWaveCell.Collapsed)
            {
                // continue to next dir
                continue;
            }

            if (compWaveCell.Position.Equals(new Vector2Int(41, 35)))
            {
                Debug.Log("Gotcha!!");
            }

            // If it does then update its possible tiles
            bool updated = UpdatePossibleTiles(compWaveCell, waveCell.PossibleTiles, direction);


            // Check if the cell possibilities have been updated
            if (updated)
            {
                // If it has then run propogate for that cell
                lowEntropySet.Remove(compWaveCell);
                compWaveCell.CalculateEntropy(tilemapProperties.IndexToFrequency);
                lowEntropySet.Add(compWaveCell);
                Propagate(compWaveCell);
            }
            else
            {
                continue;
            }
        }

        return;
    }


    private bool UpdatePossibleTiles(WaveCell compWaveCell, HashSet<int> possibleTileHashes, WFCDirection direction)
    {
        HashSet<int> allowedTiles = new HashSet<int>();

        foreach (int hash in possibleTileHashes)
        {
            if (rulesGenerator.TileRules.TryGetValue(hash, out var rules))
            {
                if (rules.TryGetValue(direction, out var neighborDict))
                {
                    foreach (var neighbor in neighborDict)
                    {
                        allowedTiles.Add(neighbor);
                    }
                }
            }
        }

        int before = compWaveCell.PossibleTiles.Count;
        compWaveCell.PossibleTiles.IntersectWith(allowedTiles);
        int after = compWaveCell.PossibleTiles.Count;
        if (after == 0)
        {
            hasConflict = true;
            conflictCell = compWaveCell;
            LogConflictCell();
            return false;
        }

        return after < before;
    }

    public WaveCell GetLowestEntropyCell()
    {
        if (lowEntropySet.Count <= 0)
        {
            return null;
        }
        else
        {
            var lowestEntropyElement = lowEntropySet.First();
            lowEntropySet.Remove(lowestEntropyElement);
            return lowestEntropyElement;
        }
    }

    public void CollapseCell(WaveCell waveCell)
    {
        var possibleTiles = waveCell.PossibleTiles;

        if (possibleTiles.Count == 0)
        {
            return;
        }
        else
        {
            //int index = WFCUtils.SelectSolutionPatternFromFrequency(waveCell.PossibleTiles, tilemapProperties.TileIndices);
            while (waveCell.PossibleTiles.Count > 0)
            {
                int index = WFCUtils.SelectSolutionFromIndex(waveCell.PossibleTiles);

                if (waveCell.TileType.Equals(TileType.WallFront))
                {
                    Debug.Log("Gotcha!!");
                }
                bool valueAllowed = CheckForCollisions(waveCell, index);
                if (valueAllowed)
                {
                    waveCell.Collapsed = true;
                    waveCell.CollapsedIndex = index;
                    waveCell.PossibleTiles = new HashSet<int>() { index };
                    break;
                }
                else
                {
                    if (waveCell.PossibleTiles.Count == 0)
                    {
                        hasConflict = true;
                        conflictCell = waveCell;
                        LogConflictCell();
                        break;
                    }
                    else
                    {
                        waveCell.PossibleTiles.Remove(index);
                    }
                }
            }
        }
    }

    public void CollapseCellNew(WaveCell waveCell)
    {
        HashSet<int> possibleTiles = waveCell.PossibleTiles;

        if (possibleTiles.Count == 0)
        {
            return;
        }
        else
        {
            //int index = WFCUtils.SelectSolutionPatternFromFrequency(waveCell.PossibleTiles, tilemapProperties.TileIndices);

            HashSet<int> allowedTiles = GetNoCollisionTiles(waveCell);
            HashSet<int> matchedTiles = new HashSet<int>(allowedTiles.Intersect(possibleTiles));

            if (matchedTiles.Count == 0)
            {
                hasConflict = true;
                conflictCell = waveCell;
                LogConflictCell();
                return;
            }

            int index;
            if (matchedTiles.Count == 1)
            {
                index = matchedTiles.First();
            }
            else
            {
                index = WFCUtils.SelectSolutionFromIndex(matchedTiles);
            }
            waveCell.Collapsed = true;
            waveCell.CollapsedIndex = index;
            waveCell.PossibleTiles = new HashSet<int>() { index };
        }
    }

    public HashSet<int> GetNoCollisionTiles(WaveCell wavecell)
    {
        List<HashSet<int>> allowedTilesLists = new List<HashSet<int>>();

        foreach (WFCDirection direction in Enum.GetValues(typeof(WFCDirection)))
        {
            WaveCell compWaveCell = wavecell.GetNeigbouringCell(direction, outputCells);
            WFCDirection opDirection = WaveCell.GetOppositeDirection(direction);

            if (compWaveCell == null)
            {
                if (rulesGenerator.TileRules.TryGetValue(0, out var rules))
                {
                    if (rules.TryGetValue(opDirection, out var neighborDict))
                    {
                        allowedTilesLists.Add(neighborDict);
                    }
                }
            }
            else
            {
                HashSet<int> allowedTilesNeighbour = new HashSet<int>();
                foreach (int hash in compWaveCell.PossibleTiles)
                {
                    if (rulesGenerator.TileRules.TryGetValue(hash, out var rules))
                    {
                        if (rules.TryGetValue(opDirection, out var neighborDict))
                        {
                            allowedTilesNeighbour.UnionWith(neighborDict);
                        }
                    }
                }
                allowedTilesLists.Add(allowedTilesNeighbour);
            }
        }

        HashSet<int> allowedTiles = new HashSet<int>(allowedTilesLists[0]); // Start with the first set

        for (int i = 1; i < allowedTilesLists.Count; i++)
        {
            allowedTiles.IntersectWith(allowedTilesLists[i]);
        }

        return allowedTiles;
    }

    public bool CheckForCollisions(WaveCell wavecell, int selectedHash)
    {

        foreach (WFCDirection direction in Enum.GetValues(typeof(WFCDirection)))
        {
            WaveCell compWaveCell = wavecell.GetNeigbouringCell(direction, outputCells);
            WFCDirection opDirection = WaveCell.GetOppositeDirection(direction);

            if (compWaveCell != null)
            {
                List<int> allowedTiles = new List<int>();
                foreach (int hash in compWaveCell.PossibleTiles)
                {
                    if (rulesGenerator.TileRules.TryGetValue(hash, out var rules))
                    {
                        if (rules.TryGetValue(opDirection, out var neighborDict))
                        {
                            foreach (var neighbor in neighborDict)
                            {
                                allowedTiles.Add(neighbor);
                            }
                        }
                    }
                }
                if (!allowedTiles.Contains(selectedHash))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void Reset()
    {
        hasConflict = false;
        conflictCell = null;
        lowEntropySet.Clear();
        propagationSet.Clear();
    }

    public void LogConflictCell()
    {
        Debug.Log("Confict Cell");
        Debug.Log($"Cell Type: {conflictCell.TileType}");
        Debug.Log($"Position: {conflictCell.Position.x}: {conflictCell.Position.y}");
        Debug.Log($"Neighbours");

        List<Vector2Int> neighbourDiffs = new List<Vector2Int> { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0) };

        foreach (var diff in neighbourDiffs)
        {
            Vector2Int neighbourPos = conflictCell.Position + diff;
            foreach (var waveCell in outputCells)
            {
                if (waveCell.Position == neighbourPos)
                {
                    Debug.Log($"Cell Type: {waveCell.TileType}");
                    Debug.Log($"Position: {neighbourPos.x}: {neighbourPos.y}");
                    foreach (var hash in waveCell.PossibleTiles)
                    {
                        Debug.Log($"Possible Tile: {tilemapProperties.IndexToCell[hash].BaseTile.name}");
                    }
                }
            }
        }
    }

}



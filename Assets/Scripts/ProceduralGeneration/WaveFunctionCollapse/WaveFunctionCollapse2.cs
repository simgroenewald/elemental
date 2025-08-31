using System.Collections.Generic;
using UnityEngine;


public class WaveFunctionCollapse2
{
    private HashSet<StructureTile> structureTiles;
    private TilemapProperties tilemapProperties;
    private List<WaveCell> outputCells;
    private RulesGenerator rulesGenerator;
    private int maxIterations = 10;
    private Propagator propagator;
    private bool solved = false;

    public WaveFunctionCollapse2(HashSet<StructureTile> roomTiles, TilemapProperties mapProperties, int maxIterations)
    {
        this.structureTiles = roomTiles;
        this.tilemapProperties = mapProperties;
        this.outputCells = new List<WaveCell>();
        this.maxIterations = maxIterations;
        this.rulesGenerator = new RulesGenerator(mapProperties);
        InitialiseOutputCells();
        this.propagator = new Propagator(mapProperties, rulesGenerator, outputCells);
        this.propagator.PopulateLowEntropySet(outputCells);
    }

    private void InitialiseOutputCells() {
        outputCells.Clear();
        float entropyNoiseIncrement = 0.000001f;
        float entropyNoise = 0.000001f;
        foreach (var tile in structureTiles)
        {
            WaveCell wavecell = new WaveCell(tile.position, tile.tileType, entropyNoise);
            wavecell.InitializePossibleTiles(tilemapProperties.IndexToCell, tilemapProperties.IndexToFrequency);
            outputCells.Add(wavecell);
            entropyNoise = entropyNoise + entropyNoiseIncrement;
        }
    }

    public void PopulateOutputCells()
    {
        int iteration = 0;
        while (iteration < this.maxIterations && !solved)
        {
            while (!propagator.hasConflict && !solved)
            {
                WaveCell waveCell = propagator.GetLowestEntropyCell();
                if (waveCell == null)
                {
                    solved = true;
                    break;
                }
                if (waveCell.Position.Equals(new Vector2Int(100, 107)))
                {
                    Debug.Log("Gotcha");
                }
                propagator.CollapseCellNew(waveCell);
                if (propagator.hasConflict)
                {
                    break;
                }
                propagator.Propagate(waveCell);
                propagator.propagationSet.Clear();
            }
/*            if (propagator.hasConflict && iteration < this.maxIterations - 1)
            {
                Reset();
            }*/
/*            else
            {
                Debug.Log("Solved on: " + iteration);
                //this.outputGrid.PrintResultsToConsole();
                break;
            }*/
            iteration += 1;
        }
        if (iteration >= this.maxIterations)
        {
            Debug.Log("Couldn't solve the tilemap");
        }
        
        PopulateStructureTiles();
    }

    public void Reset()
    {
        InitialiseOutputCells();
        this.propagator.Reset();
        this.propagator.PopulateLowEntropySet(outputCells);
    }

    public void PopulateStructureTiles()
    {
        int index = 0;
        foreach (var tile in structureTiles)
        {
            WaveCell waveCell = outputCells[index];

            if (waveCell.CollapsedIndex == -1)
            {
                Debug.LogError($"Wavecell collapsed index not set: {waveCell.Position.x}: {waveCell.Position.y}");
                index++;
                continue;
            }

            Cell cell = tilemapProperties.IndexToCell[waveCell.CollapsedIndex];
            tile.baseTile = cell.BaseTile;
            tile.frontTile = cell.FrontTile;
            tile.baseDecorTile = cell.BaseDecorTile;
            tile.frontDecorTile = cell.FrontDecorTile;
            tile.collisionTile = cell.CollisionTile;

            index++;
        }
    }

}

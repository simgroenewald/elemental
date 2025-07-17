using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.UIElements;

public class CellularAutomata
{
    [Header("Grid Settings")]
    Vector3Int position;
    private int gridWidth = 20;
    private int gridHeight = 20;
    [Range(1, 100)]
    private int noiseDensity = 65;

    public int[,] tileGrid;

    public CellularAutomata(int _gridWidth = 50, int _gridHeight = 50)
    {
        gridWidth = _gridWidth;
        gridHeight = _gridHeight;
    }

    public void GenerateNoiseGrid()
    {
        tileGrid = new int[gridHeight, gridWidth];
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                int randomValue = Random.Range(1, 101);
                tileGrid[y, x] = (randomValue <= noiseDensity) ? 1 : 0;
            }
        }
    }

    public void IterateCellularAutomaton(int iterations, int wallThreshold)
    {
        for (int i = 0; i < iterations; i++)
        {
            ApplyCellularAutomaton(wallThreshold);
        }
    }

    private void ApplyCellularAutomaton(int wallThreshold)
    {
       int[,] newGrid = new int[gridHeight, gridWidth];

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                (int wallCount, int floorCount) = GetNeighbourCounts(x, y);

                // Example rule (you can customize this):
                // If there are more walls than floors around, this cell becomes a wall.
                if (wallCount > wallThreshold)
                    newGrid[y, x] = 1;
                else
                    newGrid[y, x] = 0;
            }
        }

        tileGrid = newGrid;
    }

    private (int wallCount, int floorCount) GetNeighbourCounts(int x, int y)
    {
        int wallCount = 0;
        int floorCount = 0;

        for (int offsetY = -1; offsetY <= 1; offsetY++)
        {
            for (int offsetX = -1; offsetX <= 1; offsetX++)
            {
                // Skip the current tile
                if (offsetX == 0 && offsetY == 0)
                    continue;

                int nx = x + offsetX;
                int ny = y + offsetY;

                bool isOutOfBounds = nx < 0 || ny < 0 || nx >= gridWidth || ny >= gridHeight;

                if (isOutOfBounds || tileGrid[ny, nx] == 1)
                    wallCount++;
                else
                    floorCount++;
            }
        }

        return (wallCount, floorCount);
    }

    public void CleanEdges()
    {
        int[,] newGrid = (int[,])tileGrid.Clone();

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (tileGrid[y, x] != 0) // Skip if not floor
                    continue;

                bool hasWallAbove = IsWallAt(x, y + 1);
                bool hasWallBelow = IsWallAt(x, y - 1);
                bool hasWallLeft = IsWallAt(x - 1, y);
                bool hasWallRight = IsWallAt(x + 1, y);

                bool verticalWallSandwich = hasWallAbove && hasWallBelow;
                bool horizontalWallSandwich = hasWallLeft && hasWallRight;

                if (verticalWallSandwich || horizontalWallSandwich)
                {
                    newGrid[y, x] = 1; // Convert to wall
                }
            }
        }

        tileGrid = newGrid;
    }

    private bool IsWallAt(int x, int y)
    {
        if (x < 0 || y < 0 || x >= gridWidth || y >= gridHeight)
            return true; // Treat out-of-bounds as wall
        return tileGrid[y, x] == 1;
    }

}
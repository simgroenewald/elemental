using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WFCUtils
{
    public static WaveCell GetRandomCell(List<WaveCell> outputCells)
    {
        int randomIndex = Random.Range(0, outputCells.Count);
        return outputCells[randomIndex];
    }

    public static int SelectSolutionPatternFromFrequency(HashSet<int> possibleTiles, List<int> hashList)
    {
        // Count frequencies
        Dictionary<int, int> frequencyMap = new Dictionary<int, int>();
        foreach (int hash in hashList)
        {
            if (possibleTiles.Contains(hash))
            {
                if (!frequencyMap.ContainsKey(hash))
                    frequencyMap[hash] = 0;

                frequencyMap[hash]++;
            }
        }

        // Total weight
        int totalWeight = 0;
        foreach (int count in frequencyMap.Values)
            totalWeight += count;

        // Random selection
        float randomValue = Random.Range(0, totalWeight);
        int cumulative = 0;
        foreach (var kvp in frequencyMap)
        {
            cumulative += kvp.Value;
            if (randomValue < cumulative)
                return kvp.Key;
        }

        Debug.LogError("No solution selected from frequency");

        return possibleTiles.First(); // Fallback (should not happen)
    }

    public static int SelectSolutionFromIndex(HashSet<int> possibleTiles)
    {

        return possibleTiles.ElementAt(Random.Range(0, possibleTiles.Count));
    }
}

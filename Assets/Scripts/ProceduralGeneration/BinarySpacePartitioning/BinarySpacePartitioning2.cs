using System.Collections.Generic;
using UnityEngine;

public static class BinarySpacePartitioning2
{
    public static List<HashSet<BoundsInt>> Execute(BoundsInt area, int minWidth, int minHeight, float mergeChance = 0.4f)
    {
        var partitions = new List<BoundsInt>();
        Queue<BoundsInt> queue = new Queue<BoundsInt>();
        queue.Enqueue(area);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            // Add early termination randomness
            if ((current.size.x < 2 * minWidth && current.size.y < 2 * minHeight) || Random.value < 0.2f)
            {
                partitions.Add(current);
                continue;
            }

            // Randomly decide to split horizontally or vertically
            bool splitHorizontally = Random.value > 0.5f;

            if (splitHorizontally && current.size.x >= 2 * minWidth + 5)
            {
                int padding = Random.Range(2, 6); // 2-5 tile spacing
                int splitX = Random.Range(minWidth, current.size.x - minWidth);

                var left = new BoundsInt(current.min, new Vector3Int(splitX - padding / 2, current.size.y, 1));
                var right = new BoundsInt(new Vector3Int(current.xMin + splitX + padding / 2, current.yMin, 0),
                                          new Vector3Int(current.size.x - splitX - padding / 2, current.size.y, 1));

                queue.Enqueue(left);
                queue.Enqueue(right);
            }
            else if (!splitHorizontally && current.size.y >= 2 * minHeight + 5)
            {
                int padding = Random.Range(2, 6); // 2-5 tile spacing
                int splitY = Random.Range(minHeight, current.size.y - minHeight);

                var bottom = new BoundsInt(current.min, new Vector3Int(current.size.x, splitY - padding / 2, 1));
                var top = new BoundsInt(new Vector3Int(current.xMin, current.yMin + splitY + padding / 2, 0),
                                        new Vector3Int(current.size.x, current.size.y - splitY - padding / 2, 1));

                queue.Enqueue(bottom);
                queue.Enqueue(top);
            }
            else
            {
                partitions.Add(current);
            }
        }

        // Post-process and return merged room groups
        List<HashSet<BoundsInt>> roomGroups = ShiftAndMerge(partitions, area, minWidth, minHeight);
        return roomGroups;
    }

    private static List<HashSet<BoundsInt>> ShiftAndMerge(List<BoundsInt> partitions, BoundsInt area, int minWidth, int minHeight)
    {
        // Shift, trim, and optionally merge on overlap
        List<BoundsInt> shiftedPartitions = new List<BoundsInt>();

        foreach (var partition in partitions)
        {
            // Random shift (50/50 per direction)
            int dx = 0;
            int dy = 0;
            if (Random.value < 0.5f) dx -= (int)(minWidth / 2.5);
            if (Random.value < 0.5f) dx += (int)(minWidth / 2.5);
            if (Random.value < 0.5f) dy -= (int)(minHeight / 2.5);
            if (Random.value < 0.5f) dy += (int)(minHeight / 2.5);

            BoundsInt shifted = new BoundsInt(partition.min + new Vector3Int(dx, dy, 0), partition.size);

            // Trim shifted room to fit within bounds
            shifted = TrimToBounds(shifted, area);

            if (shifted.size.x > 6 && shifted.size.y > 6)
            {
                shiftedPartitions.Add(shifted);
            }
        }

        // If none of the shifted partitions were big enough use the original partitions
        if (shiftedPartitions.Count == 0 )
        {
            foreach (var partition in partitions)
            {
                BoundsInt newPartition = TrimToBounds(partition, area);
                shiftedPartitions.Add(newPartition);
            }
        }

        List<HashSet<BoundsInt>> groupedPartitions = GroupPartitions(shiftedPartitions);
        return groupedPartitions;
    }

    private static List<HashSet<BoundsInt>> GroupPartitions(List<BoundsInt> shiftedPartitions)
    {
        List<HashSet<BoundsInt>> groupedPartitions = new List<HashSet<BoundsInt>>();
        foreach (var partition in shiftedPartitions)
        {
            // Check for overlap to merge
            foreach (var compPartition in shiftedPartitions)
            {
                if (Overlaps(partition, compPartition))
                {
                    int partitionGroupIndex = GetGroup(partition, compPartition, groupedPartitions);
                    if (partitionGroupIndex == -1)
                    {
                        HashSet<BoundsInt> partitionGroup = new HashSet<BoundsInt>();
                        groupedPartitions.Add(partitionGroup);
                        partitionGroupIndex = groupedPartitions.Count - 1;
                    }

                    groupedPartitions[partitionGroupIndex].Add(partition);
                    groupedPartitions[partitionGroupIndex].Add(compPartition);
                    break;
                }
            }
        }
        return groupedPartitions;
    }

    private static int GetGroup(BoundsInt part, BoundsInt shifted, List<HashSet<BoundsInt>> partGroups)
    {
        int index = 0;
        foreach (var group in partGroups)
        {
            foreach (var gPart in group)
            {
                if (part == gPart || shifted == gPart)
                {
                    return index;
                }
            }
            index++;
        }
        return -1;
    }


    private static bool Overlaps(BoundsInt partBounds, BoundsInt compPartBounds)
    {
        //if (partBounds.position == compPartBounds.position) return false;
        return partBounds.xMin < compPartBounds.xMax && partBounds.xMax > compPartBounds.xMin &&
               partBounds.yMin < compPartBounds.yMax && partBounds.yMax > compPartBounds.yMin;
    }

    private static BoundsInt TrimToBounds(BoundsInt room, BoundsInt bounds)
    {
        int newXMin = Mathf.Max(room.xMin, bounds.xMin + 1);
        int newYMin = Mathf.Max(room.yMin, bounds.yMin + 1);
        int newXMax = Mathf.Min(room.xMax, bounds.xMax - 1);
        int newYMax = Mathf.Min(room.yMax, bounds.yMax - 1);

        var newMin = new Vector3Int(newXMin, newYMin, 0);
        var newSize = new Vector3Int(newXMax - newXMin, newYMax - newYMin, 1);

        return new BoundsInt(newMin, newSize);
    }
}

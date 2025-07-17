using System.Collections.Generic;
using UnityEngine;

public static class BSPCorridorGenerator
{
    public static HashSet<Vector2Int> GenerateCorridors(List<HashSet<Vector2Int>> areas)
    {
        List<Vector2Int> areaCenters = new List<Vector2Int>();
        foreach (HashSet<Vector2Int> area in areas)
        {
            areaCenters.Add(GetCenter(area));
        }

        HashSet<Vector2Int> corridors = ConnectRooms(areaCenters);

        return corridors;
    }

    private static HashSet<Vector2Int> ConnectRooms(List<Vector2Int> areaCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = areaCenters[UnityEngine.Random.Range(0, areaCenters.Count)];

        areaCenters.Remove(currentRoomCenter);

        while(areaCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPoint(currentRoomCenter, areaCenters);
            areaCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }
        return corridors;
    }

    private static HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);

        int width = UnityEngine.Random.Range(3, 6);
        int sideTileCount = width / 2;

        while (position.y != destination.y)
        {
            if (destination.y > position.y)
            {
                position += Vector2Int.up;
                for (int i = 1; i < sideTileCount + 1; i++)
                {
                    Vector2Int tileLeft = new Vector2Int(position.x - i, position.y);
                    Vector2Int tileRight = new Vector2Int(position.x + i, position.y);
                    corridor.Add(tileLeft);
                    corridor.Add(tileRight);
                }
            }
            else if (destination.y < position.y)
            {
                position += Vector2Int.down;
                for (int i = 1; i < sideTileCount + 1; i++)
                {
                    Vector2Int tileLeft = new Vector2Int(position.x - i, position.y);
                    Vector2Int tileRight = new Vector2Int(position.x + i, position.y);
                    corridor.Add(tileLeft);
                    corridor.Add(tileRight);
                }
            }
            corridor.Add(position);
        }
        while (position.x != destination.x) 
        {
            if (destination.x > position.x)
            {
                position += Vector2Int.right;
                for (int i = 1; i < sideTileCount + 1; i++)
                {
                    Vector2Int tileDown = new Vector2Int(position.x, position.y - i);
                    Vector2Int tileUp = new Vector2Int(position.x + i, position.y + i);
                    corridor.Add(tileDown);
                    corridor.Add(tileUp);
                }
            }
            else if (destination.x < position.x)
            {
                position += Vector2Int.left;
                for (int i = 1; i < sideTileCount + 1; i++)
                {
                    Vector2Int tileDown = new Vector2Int(position.x, position.y - i);
                    Vector2Int tileUp = new Vector2Int(position.x + i, position.y + i);
                    corridor.Add(tileDown);
                    corridor.Add(tileUp);
                }
            }
            corridor.Add(position);
        }
        return corridor;
    }

    private static Vector2Int FindClosestPoint(Vector2Int currentAreaCenter, List<Vector2Int> areaCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var position in areaCenters)
        {
            float currentDistance = Vector2.Distance(currentAreaCenter, position);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }
        return closest;
    }

    private static Vector2Int GetCenter(HashSet<Vector2Int> area)
    {
        if (area == null || area.Count == 0)
            return Vector2Int.zero;

        int sumX = 0;
        int sumY = 0;

        foreach (var pos in area)
        {
            sumX += pos.x;
            sumY += pos.y;
        }

        return new Vector2Int(sumX / area.Count, sumY / area.Count);
    }
}

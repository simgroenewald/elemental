using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
    public class Platform : Structure
    {
        public ConnectorOrientation orientation;
        public BoundsInt bounds;

        public Platform(ConnectorOrientation _orientation)
        {
            orientation = _orientation;
        }

        public void GenerateBounds()
        {
            if (floorPositions == null || floorPositions.Count == 0)
            {
                bounds = new BoundsInt();
                return;
            }

            int minX = floorPositions.Min(pos => pos.x);
            int maxX = floorPositions.Max(pos => pos.x);
            int minY = floorPositions.Min(pos => pos.y);
            int maxY = floorPositions.Max(pos => pos.y);

            Vector3Int position = new Vector3Int(minX - 1, minY - 1, 0);
            Vector3Int size = new Vector3Int((maxX - minX) + 3, (maxY - minY) + 3, 1);

            bounds = new BoundsInt(position, size);
        }
    }


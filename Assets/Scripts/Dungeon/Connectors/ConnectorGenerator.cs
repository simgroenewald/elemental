using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

    public class ConnectorGenerator : MonoBehaviour
    {
        public List<Connector> GenerateConnectors(List<DungeonRoom> rooms, int connectorWidth)
        {
            List<Connector> connectors = new List<Connector>();

            foreach (var parent in rooms)
            {
                Debug.Log($"Child count: {parent.children.Count} ");
                foreach (var child in parent.children)
                {
                    Debug.Log($"Connecting {parent.roomType} {parent.theme} to {child.roomType} {child.theme}");
                    // Get direction from parent to child
                    Direction direction = GetDirectionToChild(parent, child);
                    ConnectorOrientation orientation = GetCorridorOrientaion(direction);

                    // Get best edge tile pair (closest across given direction)
                    (Vector2Int start, Vector2Int end) = GetClosestEdgeTiles(parent, child, direction, connectorWidth, orientation);
                    Debug.Log($"Connecting {parent.roomType} {parent.theme} to {child.roomType} {child.theme} in direction: {direction} - corridor orientation {orientation}");
                    Connector connector = new Connector(start, end, orientation);

                    Debug.Log($"Start ({start.x}, {start.y}) - end ({end.x}, {end.y})");

                    // Generate L-shaped corridor path
                    GenerateWideCorridorPath(start, end, connector, connectorWidth);

                    SetAllBounds(connector);

                    connector.GenerateAllPartStructureTiles();

                    connectors.Add(connector);
                }
            }
            Debug.Log($"Corridor Count: {connectors.Count}");
            return connectors;
        }

        private Direction GetDirectionToChild(DungeonRoom parent, DungeonRoom child)
        {
            foreach (var kvp in parent.connections)
            {
                if (kvp.Value == child)
                    return kvp.Key;
            }

            throw new Exception("Direction to child not found.");
        }


        private ConnectorOrientation GetCorridorOrientaion(Direction direction)
        {
            if (direction == Direction.North || direction == Direction.South)
            {
                return ConnectorOrientation.Vertical;
            }
            else if (direction == Direction.East || direction == Direction.West)
            {
                return ConnectorOrientation.Horizontal;
            }
            else
            {
                Debug.Log($"Connector orientation could nor be found - invalid direction {direction}. Returning deafaut Vertical");
                return ConnectorOrientation.Vertical;
            }
        }

        private (Vector2Int, Vector2Int) GetClosestEdgeTiles(DungeonRoom parent, DungeonRoom child, Direction direction, int corridorWidth, ConnectorOrientation orientation)
        {
            var parentEdge = parent.edgeLists[direction];
            var opposite = GetOppositeDirection(direction);
            var childEdge = child.edgeLists[opposite];

            Vector2Int bestStart = Vector2Int.zero;
            Vector2Int bestEnd = Vector2Int.zero;
            float minDistance = float.MaxValue;

            foreach (var p in parentEdge)
            {
                if (!HasCorridorWidth(p, direction, parentEdge, corridorWidth + 2)) continue;

                foreach (var c in childEdge)
                {
                    if (!HasCorridorWidth(c, opposite, childEdge, corridorWidth + 2)) continue;

                    float dist = Vector2Int.Distance(p, c);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        bestStart = p;
                        bestEnd = c;
                    }
                }
            }

            // Allow wider doorways so that bridge ropes dont overlap + 2 (1 tile for each side)
            parent.AddDoorway(bestStart, corridorWidth + 2, orientation);
            child.AddDoorway(bestEnd, corridorWidth + 2, orientation);

            return (bestStart, bestEnd);
        }

        private bool HasCorridorWidth(Vector2Int pos, Direction dir, List<Vector2Int> edge, int width)
        {
            if (width % 2 == 0)
            {
                Debug.LogError($"Corridor width {width} cannot be an even value");
            }

            Boolean hasWidth = true;
            int sideTileCount = width / 2;
            Vector2Int perpA, perpB;

            // Check 3-tile corridor: one tile in center, one above, one below (if vertical), or left/right (if horizontal)
            if (dir == Direction.North || dir == Direction.South)
            {
                for (int i = 1; i < sideTileCount + 1; i++)
                {
                    perpA = new Vector2Int(pos.x - i, pos.y);
                    perpB = new Vector2Int(pos.x + i, pos.y);
                    hasWidth = edge.Contains(perpA) && edge.Contains(perpB);
                }
            }
            else // East or West
            {
                for (int i = 1; i < sideTileCount + 1; i++)
                {
                    perpA = new Vector2Int(pos.x, pos.y - i);
                    perpB = new Vector2Int(pos.x, pos.y + i);
                    hasWidth = edge.Contains(perpA) && edge.Contains(perpB);
                }
            }

            return hasWidth;
        }

        private Direction GetOppositeDirection(Direction dir)
        {
            return dir switch
            {
                Direction.North => Direction.South,
                Direction.South => Direction.North,
                Direction.East => Direction.West,
                Direction.West => Direction.East,
                _ => throw new Exception("Invalid direction")
            };
        }

        private void GenerateWideCorridorPath(Vector2Int start, Vector2Int end, Connector connector, int width)
        {
            int xDiff = Mathf.Abs(start.x - end.x);
            int yDiff = Mathf.Abs(start.y - end.y);
            int offset = width / 2 - 1;

            Vector2Int minXVector = start.x < end.x ? start + Vector2Int.right : end + Vector2Int.right;
            Vector2Int minYVector = start.y < end.y ? start + Vector2Int.up : end + Vector2Int.up;
            Vector2Int maxXVector = start.x > end.x ? start + Vector2Int.left : end + Vector2Int.left;
            Vector2Int maxYVector = start.y > end.y ? start + Vector2Int.down : end + Vector2Int.down;

            if (connector.orientation == ConnectorOrientation.Vertical)
            {
                if (xDiff == 0)
                {
                    // Go straight up
                    TraverseVertically(minYVector.y, maxYVector.y, minYVector.x, connector, offset, false, false, false);

                }
                else
                {
                    int firstCount = UnityEngine.Random.Range(4, yDiff - 6);
                    // Go up
                    TraverseVertically(minYVector.y - 1, minYVector.y + firstCount - 1, minYVector.x, connector, offset, true, true, false);
                    if (minYVector.x < maxYVector.x)
                    {
                        // Go right
                        TraverseHorizontally(minYVector.x - offset, maxYVector.x + offset, minYVector.y + firstCount, connector, offset, true, false, false);
                    }
                    else
                    {
                        // Go left
                        TraverseHorizontally(maxYVector.x - offset, minYVector.x + offset, minYVector.y + firstCount, connector, offset, true, false, false);
                    }
                    // Go up
                    TraverseVertically(minYVector.y + firstCount + 1, maxYVector.y + 1, maxYVector.x, connector, offset, true, false, true);

                }
            }
            else
            {
                if (yDiff == 0)
                {
                    TraverseHorizontally(minXVector.x, maxXVector.x, minXVector.y, connector, offset, false, false, false);
                }
                else
                {
                    int firstCount = UnityEngine.Random.Range(6, xDiff - 6); ;
                    TraverseHorizontally(minXVector.x, minXVector.x + firstCount, minXVector.y, connector, offset, true, true, false);
                    if (minXVector.y < maxXVector.y)
                    {
                        // Go right
                        TraverseVertically(minXVector.y - offset, maxXVector.y + offset, minXVector.x + firstCount, connector, offset, true, false, false);
                    }
                    else
                    {
                        // Go left
                        TraverseVertically(maxXVector.y - offset, minXVector.y + offset, minXVector.x + firstCount, connector, offset, true, false, false);
                    }
                    // Go up
                    TraverseHorizontally(minXVector.x + firstCount, maxXVector.x, maxXVector.y, connector, offset, true, false, true);

                }

            }

            /*        // First leg - horizontal
                    Vector2Int current = start;
                    while (current.x != end.x)
                    {
                        current.x += current.x < end.x ? 1 : -1;
                        path.AddRange(GetWideTiles(current, true, width));
                    }

                    // Second leg - vertical
                    while (current.y != end.y)
                    {
                        current.y += current.y < end.y ? 1 : -1;
                        path.AddRange(GetWideTiles(current, false, width));
                    }*/
        }

        private void TraverseVertically(int startY, int endY, int xPos, Connector corridor, int offset, Boolean platform, Boolean connectorStart, Boolean connectorEnd)
        {
            int yPos = startY;
            while (yPos != endY + 1)
            {
                Vector2Int position = new Vector2Int(xPos, yPos);
                for (var i = 1; i < offset + 1; i++)
                {
                    Vector2Int position1 = new Vector2Int(xPos + i, yPos);
                    AppendSections(startY, endY, yPos, corridor, offset, platform, connectorStart, connectorEnd, position1);
                    Vector2Int position2 = new Vector2Int(xPos - i, yPos);
                    AppendSections(startY, endY, yPos, corridor, offset, platform, connectorStart, connectorEnd, position2);
                }
                AppendSections(startY, endY, yPos, corridor, offset, platform, connectorStart, connectorEnd, position);
                yPos += 1;
            }
        }

        private void TraverseHorizontally(int startX, int endX, int yPos, Connector connector, int offset, Boolean platform, Boolean connectorStart, Boolean connectorEnd)
        {
            int xPos = startX;
            while (xPos != endX + 1)
            {
                Vector2Int position = new Vector2Int(xPos, yPos);
                for (var i = 1; i < offset + 1; i++)
                {
                    Vector2Int position1 = new Vector2Int(xPos, yPos + i);
                    AppendSections(startX, endX, xPos, connector, offset, platform, connectorStart, connectorEnd, position1);
                    Vector2Int position2 = new Vector2Int(xPos, yPos - i);
                    AppendSections(startX, endX, xPos, connector, offset, platform, connectorStart, connectorEnd, position2);
                }
                AppendSections(startX, endX, xPos, connector, offset, platform, connectorStart, connectorEnd, position);
                xPos += 1;
            }
        }

        private void AppendSections(int startPos, int endPos, int currentPos, Connector connector, int offset, Boolean platform, Boolean connectorStart, Boolean connectorEnd, Vector2Int pos)
        {
            if (connectorStart)
            {
                if (currentPos < endPos - offset)
                {
                    connector.bridgeStart.floorPositions.Add(pos);
                }
                else if (currentPos > endPos - offset)
                {
                    connector.platform.floorPositions.Add(pos);
                }

            }
            else if (connectorEnd)
            {
                if (currentPos > startPos + offset)
                {
                    connector.bridgeEnd.floorPositions.Add(pos);
                }
                else if (currentPos < startPos - offset)
                {
                    connector.platform.floorPositions.Add(pos);
                }
            }
            else
            {
                if (platform)
                {
                    connector.platform.floorPositions.Add(pos);
                }
                else
                {
                    connector.bridgeMain.floorPositions.Add(pos);
                }

            }
        }

        private void SetAllBounds(Connector connector)
        {
            if (connector.isStraight)
            {
                connector.bridgeMain.GenerateBounds();
            }
            else
            {
                connector.bridgeStart.GenerateBounds();
                connector.bridgeEnd.GenerateBounds();
                connector.platform.GenerateBounds();
            }
        }

        private IEnumerable<Vector2Int> GetWideTiles(Vector2Int center, bool horizontal, int width)
        {
            int half = width / 2;
            for (int i = -half; i <= half; i++)
            {
                yield return horizontal
                    ? new Vector2Int(center.x, center.y + i)
                    : new Vector2Int(center.x + i, center.y);
            }
        }
    }

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public static class AStar
{
    public static Stack<Vector3> BuildPath(Dungeon dungeon, HashSet<Vector2Int> validPositions, BoundsInt bounds, Vector3Int startGridPosition, Vector3Int endGridPosition, LayerMask obstacleMask)
    {

        // Create open list and closed hashset
        List<Node> openNodeList = new List<Node>();
        HashSet<Node> closedNodeHashSet = new HashSet<Node>();

        // Create gridnodes for path finding
        GridNodes gridNodes = new GridNodes(bounds);

        Node startNode = gridNodes.GetGridNode(startGridPosition.x, startGridPosition.y);
        Node targetNode = gridNodes.GetGridNode(endGridPosition.x, endGridPosition.y);

        Node endPathNode = FindShortestPath(startNode, targetNode, gridNodes, openNodeList, closedNodeHashSet, validPositions);

        if (endPathNode != null)
        {
            Stack<Vector3> pathStack =  CreatePathStack(endPathNode, dungeon);
            if (obstacleMask == 0)
            {
                return pathStack;
            }
            return SimplifyPath(pathStack, obstacleMask);
        }

        return null;
    }

    public static Stack<Vector3> SimplifyPath(Stack<Vector3> pathStack, LayerMask obstacleMask)
    {
        if (pathStack.Count < 3) return new Stack<Vector3>(pathStack); // Not enough to simplify

        List<Vector3> path = pathStack.Reverse().ToList(); // Start to end
        List<Vector3> simplified = new List<Vector3> { path[0] };

        int lastIndex = 0;

        for (int i = 1; i < path.Count; i++)
        {
            // If there's an obstacle in a direct line
            if (Physics2D.Linecast(path[lastIndex], path[i], obstacleMask))
            {
                // Add the last point before the obstacle
                simplified.Add(path[i - 1]);
                lastIndex = i - 1;
            }
            else if (i == path.Count - 1)
            {
                // If final point is reachable directly, add current
                simplified.Add(path[i]);
            }
        }

        return new Stack<Vector3>(simplified); // Already in correct order
    }

    private static Node FindShortestPath(Node startNode, Node targetNode, GridNodes gridNodes, List<Node> openNodeList, HashSet<Node> closedNodeHashSet, HashSet<Vector2Int> validPositions)
    {
        // Add start node to open list
        openNodeList.Add(startNode);

        // Loop through open node list until empty
        while (openNodeList.Count > 0)
        {
            // Sort List
            openNodeList.Sort();

            // current node = the node in the open list with the lowest fCost
            Node currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);

            // if the current node = target node then finish
            if (currentNode == targetNode)
            {
                return currentNode;
            }

            // add current node to the closed list
            closedNodeHashSet.Add(currentNode);

            // evaluate fcost for each neighbour of the current node
            EvaluateCurrentNodeNeighbours(currentNode, targetNode, gridNodes, openNodeList, closedNodeHashSet, validPositions);
        }

        return null;

    }

    private static Stack<Vector3> CreatePathStack(Node targetNode, Dungeon dungeon)
    {
        Stack<Vector3> movementPathStack = new Stack<Vector3>();

        Node nextNode = targetNode;

        // Get mid point of cell
        Vector3 cellMidPoint = dungeon.dungeonLayers.grid.cellSize * 0.5f;
        cellMidPoint.z = 0f;

        while (nextNode != null)
        {
            // Convert grid position to world position
            Vector3 worldPosition = dungeon.dungeonLayers.grid.CellToWorld(new Vector3Int(nextNode.gridPosition.x, nextNode.gridPosition.y, 0));

            // Set the world position to the middle of the grid cell
            worldPosition += cellMidPoint;

            movementPathStack.Push(worldPosition);

            nextNode = nextNode.parentNode;
        }

        return movementPathStack;
    }

    private static void EvaluateCurrentNodeNeighbours(Node currentNode, Node targetNode, GridNodes gridNodes, List<Node> openNodeList, HashSet<Node> closedNodeHashSet, HashSet<Vector2Int> validPositions)
    {
        Vector2Int currentNodeGridPosition = currentNode.gridPosition;

        Node validNeighbourNode;

        // Loop through all directions
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                validNeighbourNode = GetValidNodeNeighbour(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j, gridNodes, closedNodeHashSet, validPositions);

                if (validNeighbourNode != null)
                {
                    // Calculate new gcost for neighbour
                    int newCostToNeighbour;

                    newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode);

                    bool isValidNeighbourNodeInOpenList = openNodeList.Contains(validNeighbourNode);

                    if (newCostToNeighbour < validNeighbourNode.gCost || !isValidNeighbourNodeInOpenList)
                    {
                        validNeighbourNode.gCost = newCostToNeighbour;
                        validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);
                        validNeighbourNode.parentNode = currentNode;

                        if (!isValidNeighbourNodeInOpenList)
                        {
                            openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }
    }

    private static int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);  // 10 used instead of 1, and 14 is a pythagoras approximation SQRT(10*10 + 10*10) - to avoid using floats
        return 14 * dstX + 10 * (dstY - dstX);
    }

    private static Node GetValidNodeNeighbour(int neighbourNodeXPosition, int neighbourNodeYPosition, GridNodes gridNodes, HashSet<Node> closedNodeHashSet, HashSet<Vector2Int> validPositions)
    {

        bool validNode = validPositions.Contains(new Vector2Int(neighbourNodeXPosition, neighbourNodeYPosition));
        
        if (!validNode)
        {
            return null;
        }

        // Get neighbour node
        Node neighbourNode = gridNodes.GetGridNode(neighbourNodeXPosition, neighbourNodeYPosition);

        // if neighbour is in the closed list then skip
        if (closedNodeHashSet.Contains(neighbourNode))
        {
            return null;
        }
        else
        {
            return neighbourNode;
        }

    }
}
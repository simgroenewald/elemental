using UnityEngine;

public class GridNodes
{
    private int width;
    private int height;
    private Vector3Int offset; // used to translate world to local indices

    private Node[,] gridNode;

    public GridNodes(BoundsInt bounds)
    {
        width = bounds.size.x;
        height = bounds.size.y;
        offset = bounds.min;

        gridNode = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int worldPos = new Vector2Int(x + offset.x, y + offset.y);
                gridNode[x, y] = new Node(worldPos);
            }
        }
    }

    public Node GetGridNode(int xPosition, int yPosition)
    {
        int x = xPosition - offset.x;
        int y = yPosition - offset.y;

        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return gridNode[x, y];
        }
        else
        {
            Debug.LogWarning("Requested grid node is out of bounds");
            return null;
        }
    }

}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarTest : MonoBehaviour
{
    public Dungeon dungeon;
    public Grid grid;
    public Tilemap frontTilemap;
    public Tilemap pathTilemap;
    private Vector3Int startGridPosition;
    private Vector3Int endGridPosition;
    public TileBase startPathTile;
    public TileBase finishPathTile;

    private Vector3Int noValue = new Vector3Int(9999, 9999, 9999);
    private Stack<Vector3> pathStack;

    private void Start()
    {
        startPathTile = GameResources.Instance.preferredEnemyPathTile;
        finishPathTile = GameResources.Instance.enemyUnwalkableCollisionTilesArray[0];

    }

    private void SetUpPathTilemap()
    {
        dungeon = GameResources.Instance.dungeon;
        grid = dungeon.dungeonLayers.grid;
        pathTilemap = dungeon.dungeonLayers.frontDecorationTilemap;
        pathTilemap.ClearAllTiles();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SetUpPathTilemap();
        }

        if (dungeon == null || startPathTile == null || finishPathTile == null || grid == null || pathTilemap == null) return;

        if (Input.GetKeyDown(KeyCode.I))
        {
            ClearPath();
            SetStartPosition();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            ClearPath();
            SetEndPosition();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            DisplayPath();
        }
    }

    private void SetStartPosition()
    {
        if (startGridPosition == noValue)
        {
            startGridPosition = grid.WorldToCell(HelperUtilities.GetMouseWorldPosition());

            if (!IsPositionWithinBounds(startGridPosition))
            {
                startGridPosition = noValue;
                return;
            }

            pathTilemap.SetTile(startGridPosition, startPathTile);
        }
        else
        {
            pathTilemap.SetTile(startGridPosition, null);
            startGridPosition = noValue;
        }
    }

    private void SetEndPosition()
    {
        if (endGridPosition == noValue)
        {
            endGridPosition = grid.WorldToCell(HelperUtilities.GetMouseWorldPosition());

            if (!IsPositionWithinBounds(endGridPosition))
            {
                endGridPosition = noValue;
                return;
            }

            pathTilemap.SetTile(endGridPosition, finishPathTile);
        }
        else
        {
            pathTilemap.SetTile(endGridPosition, null);
            endGridPosition = noValue;
        }
    }

    private bool IsPositionWithinBounds(Vector3Int position)
    {
        // If  position is beyond grid then return false
        if (position.x < dungeon.bounds.xMin || position.x > dungeon.bounds.xMax
            || position.y < dungeon.bounds.yMin || position.y > dungeon.bounds.yMax)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void ClearPath()
    {
        // Clear Path
        if (pathStack == null) return;

        foreach (Vector3 worldPosition in pathStack)
        {
            pathTilemap.SetTile(grid.WorldToCell(worldPosition), null);
        }

        pathStack = null;

        //Clear Start and Finish Squares
        endGridPosition = noValue;
        startGridPosition = noValue;
    }

    private void DisplayPath()
    {
        if (startGridPosition == noValue || endGridPosition == noValue) return;

        pathStack = AStar.BuildPath(dungeon, dungeon.dungeonFloorPositions, dungeon.bounds, startGridPosition, endGridPosition, 0, 0.1f);

        if (pathStack == null) return;

        foreach (Vector3 worldPosition in pathStack)
        {
            pathTilemap.SetTile(grid.WorldToCell(worldPosition), startPathTile);
        }
    }

}
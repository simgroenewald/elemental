using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class Structure: MonoBehaviour
{
    public String name;

    public HashSet<Vector2Int> floorPositions;
    public HashSet<Vector2Int> backWallPositions;
    public HashSet<Vector2Int> frontWallPositions;
    public HashSet<Vector2Int> leftWallPositions;
    public HashSet<Vector2Int> rightWallPositions;
    public HashSet<StructureTile> structureTiles;

    public TilemapLayers tilemapLayers;

    public Structure()
    {
        floorPositions = new HashSet<Vector2Int>();
        backWallPositions = new HashSet<Vector2Int>();
        frontWallPositions = new HashSet<Vector2Int>();
        leftWallPositions = new HashSet<Vector2Int>();
        rightWallPositions = new HashSet<Vector2Int>();
        structureTiles = new HashSet<StructureTile>();
    }

    public void GenerateStructureTilesOld(BoundsInt bounds, HashSet<Vector2Int> positions, Boolean extendFrontWalls, int extentionLength = 0)
    {
        floorPositions = positions;

        for (var x = bounds.position.x; x < bounds.position.x + bounds.size.x; x++)
        {
            for (var y = bounds.position.y; y < bounds.position.y + bounds.size.y; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                Vector2Int search = new Vector2Int(121, 26);

                if (pos == search)
                {
                    Debug.Log("Gotcha");
                }

                if (floorPositions.Contains(pos))
                {
                    structureTiles.Add(new StructureTile(pos, TileType.Floor));
                    continue;
                }

                Vector2Int up = pos + Vector2Int.up;
                Vector2Int down = pos + Vector2Int.down;
                Vector2Int left = pos + Vector2Int.left;
                Vector2Int right = pos + Vector2Int.right;

                Vector2Int dgUpLeft = pos + Vector2Int.up + Vector2Int.left;
                Vector2Int dgUpRight = pos + Vector2Int.up + Vector2Int.right;
                Vector2Int dgDownLeft = pos + Vector2Int.down + Vector2Int.left;
                Vector2Int dgDownRight = pos + Vector2Int.down + Vector2Int.right;

                bool containsUp = floorPositions.Contains(up);
                bool containsDown = floorPositions.Contains(down);
                bool containsLeft = floorPositions.Contains(left);
                bool containsRight = floorPositions.Contains(right);

                bool containsDgUpLeft = floorPositions.Contains(dgUpLeft);
                bool containsDgUpRight = floorPositions.Contains(dgUpRight);
                bool containsDgDownLeft = floorPositions.Contains(dgDownLeft);
                bool containsDgDownRight = floorPositions.Contains(dgDownRight);

                // Concave corners
                if (containsUp && containsLeft && containsDgUpLeft)
                {
                    structureTiles.Add(new StructureTile(pos, TileType.BottomRightConcave));
                    /*                    backWallPositions.Add(pos);
                                        leftWallPositions.Add(pos);*/
                }
                else if (containsUp && containsRight && containsDgUpRight)
                {
                    structureTiles.Add(new StructureTile(pos, TileType.BottomLeftConcave));
                    /*                    backWallPositions.Add(pos);
                                        rightWallPositions.Add(pos);*/
                }
                else if (containsDown && containsLeft && containsDgDownLeft)
                {
                    structureTiles.Add(new StructureTile(pos, TileType.TopRightConcave));
                    /*                    frontWallPositions.Add(pos);
                                        leftWallPositions.Add(pos);*/
                }
                else if (containsDown && containsRight && containsDgDownRight)
                {
                    structureTiles.Add(new StructureTile(pos, TileType.TopLeftConcave));
                    /*                    frontWallPositions.Add(pos);
                                        rightWallPositions.Add(pos);*/
                }

                // Convex Corners
                else if (containsDgUpLeft && !containsUp && !containsDown && !containsLeft && !containsRight && !containsDgUpRight && !containsDgDownLeft && !containsDgDownRight)
                {
                    structureTiles.Add(new StructureTile(pos, TileType.TopLeftConvex));
                    frontWallPositions.Add(pos);
                    rightWallPositions.Add(pos);
                }
                else if (containsDgUpRight && !containsUp && !containsDown && !containsLeft && !containsRight && !containsDgUpLeft && !containsDgDownLeft && !containsDgDownRight)
                {
                    structureTiles.Add(new StructureTile(pos, TileType.TopRightConvex));
                    frontWallPositions.Add(pos);
                    leftWallPositions.Add(pos);
                }
                else if (containsDgDownLeft && !containsUp && !containsDown && !containsLeft && !containsRight && !containsDgUpLeft && !containsDgUpRight && !containsDgDownRight)
                {
                    structureTiles.Add(new StructureTile(pos, TileType.BottomLeftConvex));
                    backWallPositions.Add(pos);
                    rightWallPositions.Add(pos);
                }
                else if (containsDgDownRight && !containsUp && !containsDown && !containsLeft && !containsRight && !containsDgUpLeft && !containsDgUpRight && !containsDgDownLeft)
                {
                    structureTiles.Add(new StructureTile(pos, TileType.BottomRightConvex));
                    backWallPositions.Add(pos);
                    leftWallPositions.Add(pos);
                }

                // Walls
                else if (containsDown)
                {
                    structureTiles.Add(new StructureTile(pos, TileType.WallBack));
                    backWallPositions.Add(pos);
                }
                else if (containsUp)
                {
                    structureTiles.Add(new StructureTile(pos, TileType.WallFront));
                    frontWallPositions.Add(pos);
                }
                else if (containsLeft)
                {
                    structureTiles.Add(new StructureTile(pos, TileType.WallRight));
                    leftWallPositions.Add(pos);
                }
                else if (containsRight)
                {
                    structureTiles.Add(new StructureTile(pos, TileType.WallLeft));
                    rightWallPositions.Add(pos);
                }

            }
        }
        if (extendFrontWalls)
        {
            ExtendFrontWalls(extentionLength);
        }
    }

    public void GenerateStructureTiles(BoundsInt bounds, HashSet<Vector2Int> positions, Boolean extendFrontWalls, int extentionLength = 0)
    {
        floorPositions = positions;

        for (var x = bounds.position.x; x <= bounds.position.x + bounds.size.x; x++)
        {
            for (var y = bounds.position.y; y <= bounds.position.y + bounds.size.y; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                Vector2Int search = new Vector2Int(-9, 92);

                if (pos == search)
                {
                    Debug.Log("Gotcha");
                }

                TileType tileType = GetTileType(pos, floorPositions);
                if (tileType == TileType.None)
                {
                    continue;
                }

                if (tileType == TileType.WallLeft)
                {
                    List<TileType> typesToCheck = new List<TileType> { TileType.BottomRightConcave, TileType.WallFront, TileType.TopRightConvex };
                    tileType = GetSideWallTileType(extentionLength, pos, tileType, Vector2Int.left, TileType.BottomLeftConcave, typesToCheck);
                    /*                    Vector2Int up = pos + Vector2Int.up;
                                        Vector2Int up2 = pos + Vector2Int.up + Vector2Int.up;
                                        Vector2Int dgUpLeft = pos + Vector2Int.up + Vector2Int.left;
                                        Vector2Int dgUpLeft2 = pos + Vector2Int.up + Vector2Int.up + Vector2Int.left;

                                        TileType upTileType = GetTileType(up, floorPositions);
                                        TileType upTileType2 = GetTileType(up2, floorPositions);
                                        TileType dgUpLeftTileType = GetTileType(dgUpLeft, floorPositions);
                                        TileType dgUpLeftTileType2 = GetTileType(dgUpLeft2, floorPositions);

                                        if (upTileType == TileType.BottomLeftConcave && (dgUpLeftTileType == TileType.BottomRightConcave || dgUpLeftTileType == TileType.WallFront || dgUpLeftTileType == TileType.TopRightConvex) ||
                                            upTileType2 == TileType.BottomLeftConcave && (dgUpLeftTileType2 == TileType.BottomRightConcave || dgUpLeftTileType2 == TileType.WallFront || dgUpLeftTileType2 == TileType.TopRightConvex))
                                        {
                                            tileType = TileType.BottomLeftConcave;
                                        }*/

                }

                if (tileType == TileType.WallRight)
                {
                    List <TileType> typesToCheck = new List<TileType> { TileType.BottomLeftConcave, TileType.WallFront, TileType.TopLeftConvex };
                    tileType = GetSideWallTileType(extentionLength, pos, tileType, Vector2Int.right, TileType.BottomRightConcave, typesToCheck);
/*                    Vector2Int up = pos + Vector2Int.up;
                    Vector2Int up2 = pos + Vector2Int.up + Vector2Int.up;
                    Vector2Int dgUpRight = pos + Vector2Int.up + Vector2Int.right;
                    Vector2Int dgUpRight2 = pos + Vector2Int.up + Vector2Int.up + Vector2Int.right;

                    TileType upTileType = GetTileType(up, floorPositions);
                    TileType upTileType2 = GetTileType(up2, floorPositions);
                    TileType dgUpRightTileType = GetTileType(dgUpRight, floorPositions);
                    TileType dgUpRightTileType2 = GetTileType(dgUpRight2, floorPositions);

                    if (upTileType == TileType.BottomRightConcave && (dgUpRightTileType == TileType.BottomLeftConcave || dgUpRightTileType == TileType.WallFront || dgUpRightTileType == TileType.TopLeftConvex) ||
                        upTileType2 == TileType.BottomRightConcave && (dgUpRightTileType2 == TileType.BottomLeftConcave || dgUpRightTileType2 == TileType.WallFront || dgUpRightTileType2 == TileType.TopLeftConvex))
                    {
                        tileType = TileType.BottomRightConcave;
                    }*/
                }

                structureTiles.Add(new StructureTile(pos, tileType));

                // Convex Corners
                if (tileType == TileType.TopLeftConvex)
                {
                    frontWallPositions.Add(pos);
                    rightWallPositions.Add(pos);
                }
                else if (tileType == TileType.TopRightConvex)
                {
                    frontWallPositions.Add(pos);
                    leftWallPositions.Add(pos);
                }
                else if (tileType == TileType.BottomLeftConvex)
                {
                    backWallPositions.Add(pos);
                    rightWallPositions.Add(pos);
                }
                else if (tileType == TileType.BottomRightConvex)
                {
                    backWallPositions.Add(pos);
                    leftWallPositions.Add(pos);
                }

                // Walls
                else if (tileType == TileType.WallBack)
                {
                    backWallPositions.Add(pos);
                }
                else if (tileType == TileType.WallFront)
                {
                    frontWallPositions.Add(pos);
                }
                else if (tileType == TileType.WallRight)
                {
                    leftWallPositions.Add(pos);
                }
                else if (tileType == TileType.WallLeft)
                {
                    rightWallPositions.Add(pos);
                }

            }
        }
        if (extendFrontWalls)
        {
            ExtendFrontWalls(extentionLength);
        }
    }

    // Determines whether a right or left wall needs to be have a concave corner backgrounds piece due to neighbours
    private TileType GetSideWallTileType(int extentionLength, Vector2Int pos, TileType tileType, Vector2Int sideOffset, TileType upTileType, List<TileType> dgTypesToCheck)
    {
        for (int i = 1; i < extentionLength; i++)
        {
            Vector2Int upOffset = pos + new Vector2Int(0, i);
            Vector2Int dgUpLeftOffset = pos + new Vector2Int(0, i) + sideOffset;

            TileType upOffsetTileType = GetTileType(upOffset, floorPositions);
            TileType dgUpLeftOffsetTileType = GetTileType(dgUpLeftOffset, floorPositions);

            if (upOffsetTileType == upTileType && dgTypesToCheck.Contains(dgUpLeftOffsetTileType))
            {
                tileType = upTileType;
                break;
            }
        }

        return tileType;
    }

    private TileType GetTileType(Vector2Int pos, HashSet<Vector2Int> floorPositions)
    {
        TileType tileType = new TileType();
        if (floorPositions.Contains(pos))
        {
            tileType = TileType.Floor;
            return tileType;
        }

        Vector2Int up = pos + Vector2Int.up;
        Vector2Int down = pos + Vector2Int.down;
        Vector2Int left = pos + Vector2Int.left;
        Vector2Int right = pos + Vector2Int.right;

        Vector2Int dgUpLeft = pos + Vector2Int.up + Vector2Int.left;
        Vector2Int dgUpRight = pos + Vector2Int.up + Vector2Int.right;
        Vector2Int dgDownLeft = pos + Vector2Int.down + Vector2Int.left;
        Vector2Int dgDownRight = pos + Vector2Int.down + Vector2Int.right;

        bool containsUp = floorPositions.Contains(up);
        bool containsDown = floorPositions.Contains(down);
        bool containsLeft = floorPositions.Contains(left);
        bool containsRight = floorPositions.Contains(right);

        bool containsDgUpLeft = floorPositions.Contains(dgUpLeft);
        bool containsDgUpRight = floorPositions.Contains(dgUpRight);
        bool containsDgDownLeft = floorPositions.Contains(dgDownLeft);
        bool containsDgDownRight = floorPositions.Contains(dgDownRight);

        // Concave corners
        if (containsUp && containsLeft && containsDgUpLeft)
        {
            tileType = TileType.BottomRightConcave;
        }
        else if (containsUp && containsRight && containsDgUpRight)
        {
            tileType = TileType.BottomLeftConcave;
        }
        else if (containsDown && containsLeft && containsDgDownLeft)
        {
            tileType = TileType.TopRightConcave;
        }
        else if (containsDown && containsRight && containsDgDownRight)
        {
            tileType = TileType.TopLeftConcave;
        }

        // Convex Corners
        else if (containsDgUpLeft && !containsUp && !containsDown && !containsLeft && !containsRight && !containsDgUpRight && !containsDgDownLeft && !containsDgDownRight)
        {
            tileType = TileType.TopLeftConvex;
        }
        else if (containsDgUpRight && !containsUp && !containsDown && !containsLeft && !containsRight && !containsDgUpLeft && !containsDgDownLeft && !containsDgDownRight)
        {
            tileType = TileType.TopRightConvex;
        }
        else if (containsDgDownLeft && !containsUp && !containsDown && !containsLeft && !containsRight && !containsDgUpLeft && !containsDgUpRight && !containsDgDownRight)
        {
            tileType = TileType.BottomLeftConvex;
        }
        else if (containsDgDownRight && !containsUp && !containsDown && !containsLeft && !containsRight && !containsDgUpLeft && !containsDgUpRight && !containsDgDownLeft)
        {
            tileType = TileType.BottomRightConvex;
        }

        // Walls
        else if (containsDown)
        {
            tileType = TileType.WallBack;
        }
        else if (containsUp)
        {
            tileType = TileType.WallFront;
        }
        else if (containsLeft)
        {
            tileType = TileType.WallRight;
        }
        else if (containsRight)
        {
            tileType = TileType.WallLeft;
        }

        return tileType;
    }

    private void ExtendFrontWalls(int extentionLength)
    {
        if (extentionLength == 0)
        {
            Debug.LogError("No extentionLength set for extention wall.");
            return;
        }
        foreach (var pos in frontWallPositions)
        {
            for (int i = 1; i < extentionLength; i++)
            {
                Vector2Int newWallPos = new Vector2Int(pos.x, pos.y - i);
                if (!backWallPositions.Contains(newWallPos))
                {
                    if (leftWallPositions.Contains(pos))
                    {
                        structureTiles.Add(new StructureTile(newWallPos, TileType.TopRightConvex));
                    }
                    else if (rightWallPositions.Contains(pos))
                    {
                        structureTiles.Add(new StructureTile(newWallPos, TileType.TopLeftConvex));
                    }
                    else
                    {
                        structureTiles.Add(new StructureTile(newWallPos, TileType.WallFront));
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }

    public StructureTile RemoveTileAtPosition(Vector2Int targetPos)
    {
        StructureTile removedTile = null;

        foreach (var tile in this.structureTiles)
        {
            if (tile.position == targetPos)
            {
                removedTile = tile;
                break;
            }
        }

        if (removedTile != null)
        {
            this.structureTiles.Remove(removedTile);
            return removedTile;
        }

        return removedTile;
    }
}
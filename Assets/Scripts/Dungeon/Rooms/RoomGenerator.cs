using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

    public class RoomGenerator : MonoBehaviour
    {

        [SerializeField] private int width;
        [SerializeField] private int length;
        [SerializeField] private Vector2Int position;

        [SerializeField] private int minRoomWidth = 4;
        [SerializeField] private int minRoomHeight = 4;
        [SerializeField] private int dungeonWidth = 20;
        [SerializeField] private int dungeonHeight = 20;

        [SerializeField]
        [Range(0, 10)]
        private int offset = 1;

        public Tilemap tilemap;              // Assign in inspector
        public List<BoundsInt> roomsList;    // Assign or populate in code

        public void GenerateNaturalRoom(DungeonRoom dungeonRoom)
        {
            //Debug.Log($"Generating natural room at {position} with size {width}x{length}");
            int[,] tiles = GenerateCellularAutomataLayout(dungeonRoom.bounds.size.x, dungeonRoom.bounds.size.y);
            HashSet<Vector2Int> floorPositions = GenerateCellularAutomataFloor(tiles, dungeonRoom.bounds);
            dungeonRoom.floorPositions = floorPositions;
            GetEdgePositions(dungeonRoom);
            GetNaturalEdgePositions(dungeonRoom);
        }
        public void GenerateStructuredRoom(DungeonRoom dungeonRoom)
        {
            //Debug.Log($"Generating structured room at {position} with size {width}x{length}");
            List<HashSet<BoundsInt>> roomGroupsList = GenerateBSPLayout(dungeonRoom.bounds, dungeonRoom.subRoomMinWidth, dungeonRoom.subRoomMinHeight);
            List<HashSet<Vector2Int>> roomAreas = GenerateBSPRoomAreas(roomGroupsList);
            HashSet<Vector2Int> corridors = BSPCorridorGenerator.GenerateCorridors(roomAreas);
            //HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
            HashSet<Vector2Int> floorPositions = GenerateBSPFloor(roomAreas, corridors);
            CleanUpRoomTiles(dungeonRoom.bounds, floorPositions);
            dungeonRoom.GenerateStructureTiles(dungeonRoom.bounds, floorPositions, true, 3);
            dungeonRoom.roomAreas = roomAreas;
            GetNaturalEdgePositions(dungeonRoom);
        }

        private List<HashSet<Vector2Int>> GenerateBSPRoomAreas(List<HashSet<BoundsInt>> roomGroupsList)
        {
            List<HashSet<Vector2Int>> roomAreas = new List<HashSet<Vector2Int>>();
            foreach (var group in roomGroupsList)
            {
                HashSet<Vector2Int> area = new HashSet<Vector2Int>();
                foreach (var part in group)
                {
                    for (int col = offset; col < part.size.x - offset; col++)
                    {
                        for (int row = offset; row < part.size.y - offset; row++)
                        {
                            Vector2Int position = (Vector2Int)part.min + new Vector2Int(col, row);
                            area.Add(position);
                        }
                    }
                    roomAreas.Add(area);
                }
            }
            return roomAreas;
        }

        private static void CleanUpRoomTiles(BoundsInt roomBounds, HashSet<Vector2Int> floorPositions)
        {

            for (var x = roomBounds.position.x; x < roomBounds.position.x + roomBounds.size.x; x++)
            {
                for (var y = roomBounds.position.y; y < roomBounds.position.y + roomBounds.size.y; y++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    Vector2Int up = pos + Vector2Int.up;
                    Vector2Int down = pos + Vector2Int.down;
                    Vector2Int left = pos + Vector2Int.left;
                    Vector2Int right = pos + Vector2Int.right;

                    if (floorPositions.Contains(up) && floorPositions.Contains(down))
                    {
                        floorPositions.Add(pos);
                    }

                    if (floorPositions.Contains(left) && floorPositions.Contains(right))
                    {
                        floorPositions.Add(pos);
                    }
                }
            }
            /*        foreach(var tile in roomTiles)
                    {
                        Vector2Int up = tile.position + Vector2Int.up;
                        Vector2Int down = tile.position + Vector2Int.down;
                        Vector2Int left = tile.position + Vector2Int.left;
                        Vector2Int right = tile.position + Vector2Int.right;

                        if (floorPositions.Contains(up) && floorPositions.Contains(down))
                        {
                            tile.tileType = TileType.Floor;
                        }

                        if (floorPositions.Contains(left) && floorPositions.Contains(right))
                        {
                            tile.tileType = TileType.Floor;
                        }
                    }*/
        }

        private int[,] GenerateCellularAutomataLayout(int width, int length)
        {
            //Debug.Log($"Generating Floor");
            CellularAutomata cellularAutomata = new CellularAutomata(width, length);
            cellularAutomata.GenerateNoiseGrid();
            cellularAutomata.IterateCellularAutomaton(1, 3);
            cellularAutomata.IterateCellularAutomaton(4, 6);
            cellularAutomata.IterateCellularAutomaton(4, 6);
            cellularAutomata.CleanEdges();
            cellularAutomata.CleanEdges();
            int[,] tiles = cellularAutomata.tileGrid;

            return tiles;
        }

        public HashSet<Vector2Int> GenerateCellularAutomataFloor(int[,] tiles, BoundsInt bounds)
        {
            HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
            for (int y = 0; y < bounds.size.y; y++)
            {
                for (int x = 0; x < bounds.size.x; x++)
                {
                    if (tiles[y, x] == 0)
                    {
                        Vector2Int position = new Vector2Int(x + bounds.position.x, y + bounds.position.y);
                        floor.Add(position);
                    }
                }
            }
            return floor;
        }

        public List<HashSet<BoundsInt>> GenerateBSPLayout(BoundsInt bounds, int subRoomMinWidth, int subRoomMinHeight)
        {
            Vector3Int startPos = new Vector3Int(bounds.position.x, bounds.position.y, 0);
            List<HashSet<BoundsInt>> roomsList = BinarySpacePartitioning2.Execute(new BoundsInt(startPos, new Vector3Int(bounds.size.y, bounds.size.x, 0)), subRoomMinWidth, subRoomMinHeight);
            return roomsList;
        }

        public HashSet<Vector2Int> GenerateBSPFloor(List<HashSet<Vector2Int>> roomAreas, HashSet<Vector2Int> corridors)
        {
            HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
            foreach (var area in roomAreas)
            {
                floor.UnionWith(area);
            }

            floor.UnionWith(corridors);

            return floor;
        }

        public void GetEdgePositions(DungeonRoom dungeonRoom)
        {

            dungeonRoom.edgeLists[Direction.North] = new List<Vector2Int>();
            dungeonRoom.edgeLists[Direction.South] = new List<Vector2Int>();
            dungeonRoom.edgeLists[Direction.East] = new List<Vector2Int>();
            dungeonRoom.edgeLists[Direction.West] = new List<Vector2Int>();

            int maxY = dungeonRoom.floorPositions.Max(pos => pos.y);
            int minY = dungeonRoom.floorPositions.Min(pos => pos.y);
            int maxX = dungeonRoom.floorPositions.Max(pos => pos.x);
            int minX = dungeonRoom.floorPositions.Min(pos => pos.x);

            foreach (Vector2Int pos in dungeonRoom.floorPositions)
            {
                Vector2Int up = pos + Vector2Int.up;
                Vector2Int down = pos + Vector2Int.down;
                Vector2Int left = pos + Vector2Int.left;
                Vector2Int right = pos + Vector2Int.right;

                if (pos.y == maxY && pos.x < maxX - 2 && pos.x > minX + 2)
                {
                    dungeonRoom.edgeLists[Direction.North].Add(up);
                }

                if (pos.y == minY && pos.x < maxX - 2 && pos.x > minX + 2)
                {
                    dungeonRoom.edgeLists[Direction.South].Add(down);
                }

                if (pos.x == maxX && pos.y < maxY - 2 && pos.y > minY + 2)
                {
                    dungeonRoom.edgeLists[Direction.East].Add(right);
                }

                if (pos.x == minX && pos.y < maxY - 2 && pos.y > minY + 2)
                {
                    dungeonRoom.edgeLists[Direction.West].Add(left);
                }
            }
        }

        /*    public void GetEdgePositions(DungeonRoom dungeonRoom)
            {
                int maxY = dungeonRoom.roomTiles.Max(tile => tile.position.y);
                int minY = dungeonRoom.roomTiles.Min(tile => tile.position.y);
                int maxX = dungeonRoom.roomTiles.Max(tile => tile.position.x);
                int minX = dungeonRoom.roomTiles.Min(tile => tile.position.x);

                dungeonRoom.edgeLists[Direction.North] = dungeonRoom.roomTiles
                    .Where(tile => tile.position.y == maxY)
                    .Select(tile => tile.position)
                    .ToList();

                dungeonRoom.edgeLists[Direction.South] = dungeonRoom.roomTiles
                    .Where(tile => tile.position.y == minY)
                    .Select(tile => tile.position)
                    .ToList();

                dungeonRoom.edgeLists[Direction.West] = dungeonRoom.roomTiles
                    .Where(tile => tile.position.x == minX)
                    .Select(tile => tile.position)
                    .ToList();

                dungeonRoom.edgeLists[Direction.East] = dungeonRoom.roomTiles
                    .Where(tile => tile.position.x == maxX)
                    .Select(tile => tile.position)
                    .ToList();
            }*/

        public void GetNaturalEdgePositions(DungeonRoom dungeonRoom)
        {
            dungeonRoom.edgeLists[Direction.North] = new List<Vector2Int>();
            dungeonRoom.edgeLists[Direction.South] = new List<Vector2Int>();
            dungeonRoom.edgeLists[Direction.East] = new List<Vector2Int>();
            dungeonRoom.edgeLists[Direction.West] = new List<Vector2Int>();


            for (var y = dungeonRoom.bounds.position.y + 1; y < dungeonRoom.bounds.position.y + dungeonRoom.bounds.size.y - 1; y++)
            {
                for (var x = dungeonRoom.bounds.position.x + 1; x < dungeonRoom.bounds.position.x + dungeonRoom.bounds.size.x - 1; x++)
                {
                    Vector2Int tile = new Vector2Int(x, y);
                    if (dungeonRoom.floorPositions.Contains(tile))
                    {
                        Vector2Int up = tile + Vector2Int.up;
                        Vector2Int down = tile + Vector2Int.down;
                        Vector2Int left = tile + Vector2Int.left;
                        Vector2Int right = tile + Vector2Int.right;

                        if (!dungeonRoom.floorPositions.Contains(up) && dungeonRoom.floorPositions.Contains(down))
                        {
                            dungeonRoom.edgeLists[Direction.North].Add(up);
                        }
                        if (!dungeonRoom.floorPositions.Contains(down) && dungeonRoom.floorPositions.Contains(up))
                        {
                            dungeonRoom.edgeLists[Direction.South].Add(down);
                        }
                        if (!dungeonRoom.floorPositions.Contains(left) && dungeonRoom.floorPositions.Contains(right))
                        {
                            dungeonRoom.edgeLists[Direction.West].Add(left);
                        }
                        if (!dungeonRoom.floorPositions.Contains(right) && dungeonRoom.floorPositions.Contains(left))
                        {
                            dungeonRoom.edgeLists[Direction.East].Add(right);
                        }
                    }
                }
            }
        }

        /*    public int[,] GetGrid(int areaHeight, int areaWidth, List<BoundsInt> roomsList)
            {
                int[,] grid = new int[areaHeight, areaWidth];
                foreach (var room in roomsList)
                {
                    for (int x = 0; x < room.size.x; x++)
                    {
                        for (int y = 0; y < room.size.y; y++)
                        {
                            Vector3Int pos = new Vector3Int(room.xMin + x, room.yMin + y, 0);

                            grid[x, y] = 1;
                        }
                    }
                }
                return grid;
            }*/

        // Code to move to rooms

    }

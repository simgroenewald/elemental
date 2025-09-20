using UnityEngine;
using UnityEngine.Tilemaps;

public class Cell
{
    public int TileIndex { get; private set; }
    public TileBase BaseTile { get; private set; }
    public TileBase FrontTile { get; set; }
    public TileBase BaseDecorTile { get; set; }
    public TileBase FrontDecorTile { get; set; }
    public TileBase CollisionTile { get; set; }
    public TileBase TypeTile { get; set; }


    public TileType TileType { get; set; }

    //public Dictionary<WFCDirection, HashSet<int>> PossibleNeighbours { get; private set; }

    public bool IsEdge { get; set; }
    public EdgeType EdgeType { get; set; }

    public bool IsCorner { get; set; }
    public CornerType CornerType { get; set; }

    public Vector2Int Position { get; set; }

    public Cell(TileBase tileBase, TileBase frontTile, TileBase baseDecorTile, TileBase frontDecorTile, TileBase collisionTile, TileBase typeTile, int tileIndex, Vector2Int position)
    {
        BaseTile = tileBase;
        FrontTile = frontTile;
        BaseDecorTile = baseDecorTile;
        FrontDecorTile = frontDecorTile;
        TypeTile = typeTile;
        CollisionTile = collisionTile;
        TileIndex = tileIndex;
        IsEdge = false;
        IsCorner = false;
        Position = position;

        if (TypeTile is TileBase)
        {
            TileType = TileTypeMapper.GetTileType(TypeTile.name);
        }

        /*        PossibleNeighbours = new Dictionary<WFCDirection, HashSet<int>>();
                foreach (WFCDirection dir in System.Enum.GetValues(typeof(WFCDirection)))
                {
                    PossibleNeighbours[dir] = new HashSet<int>();
                }*/
        Position = position;
    }

    /*    public void AddNeighbor(WFCDirection dir, int neighborHash)
        {
            if (!PossibleNeighbours.ContainsKey(dir))
                PossibleNeighbours[dir] = new HashSet<int>();

            PossibleNeighbours[dir].Add(neighborHash);
        }*/
}
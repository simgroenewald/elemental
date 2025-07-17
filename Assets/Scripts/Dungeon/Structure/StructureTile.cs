using UnityEngine;
using UnityEngine.Tilemaps;


    public class StructureTile
    {
        public Vector2Int position;
        public TileType tileType;
        public TileBase baseTile;
        public TileBase decorTile;
        public TileBase collisionTile;

        public StructureTile(Vector2Int position, TileType tileType)
        {
            this.position = position;
            this.tileType = tileType;
        }
    }

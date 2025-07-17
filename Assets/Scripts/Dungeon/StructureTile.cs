using UnityEngine;
using UnityEngine.Tilemaps;

namespace Elemental
{
    public class StructureTile
    {
        public Vector2Int tilePosition;
        public TileType structureTileType;
        public TileBase baseTile;
        public TileBase decorTile;
        public TileBase collisionTile;

        public StructureTile(Vector2Int tilePosition, TileType structureTileType)
        {
            this.tilePosition = tilePosition;
            this.structureTileType = structureTileType;
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

    public class Doorway
    {
        public HashSet<Vector2Int> positions;

        public Doorway(Vector2Int midPositon, int width, ConnectorOrientation orientation)
        {
            positions = new HashSet<Vector2Int>();
            int sideTileCount = width / 2;

            positions.Add(midPositon);
            for (int i = 1; i < sideTileCount + 1; i++)
            {
                Vector2Int doorwayTileA = new Vector2Int();
                Vector2Int doorwayTileB = new Vector2Int();

                if (orientation == ConnectorOrientation.Vertical)
                {
                    doorwayTileA = new Vector2Int(midPositon.x - i, midPositon.y);
                    doorwayTileB = new Vector2Int(midPositon.x + i, midPositon.y);
                }
                else
                {
                    doorwayTileA = new Vector2Int(midPositon.x, midPositon.y - i);
                    doorwayTileB = new Vector2Int(midPositon.x, midPositon.y + i);

                }
                positions.Add(doorwayTileA);
                positions.Add(doorwayTileB);
            }
        }
    }
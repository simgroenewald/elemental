using UnityEngine;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine.Tilemaps;

namespace UnityEditor
{
    [CustomGridBrush(false, false, false, "NameDisplayBrush")]
    [CreateAssetMenu(fileName = "NameDisplayBrush", menuName = "Brushes/NameDisplayBrush")]
    public class NameDisplayBrush : GridBrush
    {
        // No painting, erasing, or filling logic needed
    }

    [CustomEditor(typeof(NameDisplayBrush))]
    public class TooltipBrushEditor : GridBrushEditor
    {
        private NameDisplayBrush nameDisplayBrush => target as NameDisplayBrush;

        public override void OnPaintSceneGUI(GridLayout grid, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
        {
            base.OnPaintSceneGUI(grid, brushTarget, position, tool, executing);

            if (brushTarget == null) return;

            Tilemap tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap == null) return;

            Vector3Int cellPos = new Vector3Int(position.x, position.y, 0);
            TileBase tile = tilemap.GetTile(cellPos);

            if (tile is Tile tileWithSprite && tileWithSprite.sprite != null)
            {
                Vector3 worldPos = grid.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0f);

                GUIStyle labelStyle = new GUIStyle
                {
                    normal = { textColor = Color.yellow },
                    fontStyle = FontStyle.Bold
                };

                Handles.Label(worldPos, $"Sprite: {tileWithSprite.sprite.name}", labelStyle);
            }
        }
    }
}
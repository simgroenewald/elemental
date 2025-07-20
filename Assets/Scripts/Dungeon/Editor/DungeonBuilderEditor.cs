using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DungeonBuilderTest))]
public class DungeonBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DungeonBuilderTest dungeonBuilder = (DungeonBuilderTest)target;

        if (GUILayout.Button("Generate Layout"))
        {
            dungeonBuilder.GenerateDungeon();

        }

        if (GUILayout.Button("Generate Rooms"))
        {
            dungeonBuilder.GenerateRooms();

        }


        if (GUILayout.Button("Generate Corridors"))
        {
            dungeonBuilder.GenerateCorridors();

        }

        if (GUILayout.Button("Draw Rooms"))
        {
            dungeonBuilder.DrawMap();

        }

        if (GUILayout.Button("Populate Room Tiles"))
        {
            dungeonBuilder.PopulateRoomTiles();

        }

        if (GUILayout.Button("Draw Room Tiles"))
        {
            dungeonBuilder.DrawRoomTiles();

        }

        if (GUILayout.Button("Populate Connector Tiles"))
        {
            dungeonBuilder.PopulateConnectorTiles();

        }

        if (GUILayout.Button("Draw Connector Tiles"))
        {
            dungeonBuilder.DrawConnectorTiles();

        }

        if (GUILayout.Button("Clear All Rooms"))
        {
            dungeonBuilder.ClearAllRooms();

        }

    }
}
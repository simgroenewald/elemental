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

        if (GUILayout.Button("Generate Connectors"))
        {
            dungeonBuilder.GenerateConnectors();

        }

        if (GUILayout.Button("Generate Doors"))
        {
            dungeonBuilder.GenerateDoors();

        }

        if (GUILayout.Button("Draw Map Tile Types"))
        {
            dungeonBuilder.DrawMap();

        }

        if (GUILayout.Button("Generate Containers"))
        {
            dungeonBuilder.GenerateContainers();
        }

        if (GUILayout.Button("Populate Room Tiles"))
        {
            dungeonBuilder.PopulateRoomTiles();

        }

        if (GUILayout.Button("Populate Door Tiles"))
        {
            dungeonBuilder.PopulateDoorTiles();

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

        if (GUILayout.Button("Draw Door Tiles"))
        {
            dungeonBuilder.DrawDoorTiles();

        }

        if (GUILayout.Button("Clear All Rooms"))
        {
            dungeonBuilder.ClearAllRooms();

        }

    }
}
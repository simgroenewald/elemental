using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public class SimpleEnemyInitializer : MonoBehaviour
{
    [Header("Enemy Initialization")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int enemyCount = 3;
    [SerializeField] private Transform enemyParent;

    public void InitializeEnemiesInStartRoom(DungeonRoom startRoom, Grid grid)
    {
        if (enemyPrefab == null)
        {
            // Try to find the PorcumanBlue prefab automatically
            enemyPrefab = Resources.Load<GameObject>("Prefabs/Enemies/Porcuman/Blue/PorcumanBlue");
            
            if (enemyPrefab == null)
            {
                Debug.LogError("Enemy prefab not found!");
                return;
            }
        }

        // Create parent object for enemies if not assigned
        if (enemyParent == null)
        {
            GameObject parentObj = GameObject.Find("Enemies");
            if (parentObj == null)
            {
                parentObj = new GameObject("Enemies");
            }
            enemyParent = parentObj.transform;
        }

        // Spawn enemies at random valid positions
        for (int i = 0; i < enemyCount; i++)
        {

            Vector2Int spawnPosition2D = GetEnemyStartPosition(startRoom);

            // Convert tile position to world position
            Vector3 worldPosition = grid.CellToWorld((Vector3Int)spawnPosition2D);
            // Center the position in the tile
            worldPosition += grid.cellSize * 0.16f;

            // Instantiate enemy
            GameObject enemyGO = Instantiate(enemyPrefab, worldPosition, Quaternion.identity, enemyParent);
            enemyGO.name = $"Enemy_{i + 1}";
            
            // Ensure proper 2D rotation (no X or Y rotation)
            enemyGO.transform.rotation = Quaternion.identity;

            Debug.Log($"Spawned enemy at floor position: {spawnPosition2D}");
        }

        Debug.Log($"Successfully spawned {enemyCount} enemies in start room");
    }

    public Vector2Int GetEnemyStartPosition(DungeonRoom room)
    {
        return room.structure.floorPositions.ElementAt(UnityEngine.Random.Range(0, room.structure.floorPositions.Count));
    }
}
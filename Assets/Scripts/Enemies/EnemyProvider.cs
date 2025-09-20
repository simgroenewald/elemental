using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProvider : MonoBehaviour
{
    [SerializeField] private EnemyRoomTypeMapperSO mapper;
    [SerializeField] List<EnemySpawnCount> enemySpawnCounts;

    public void Awake()
    {
        mapper.PopulateEnemiesDicts();
    }

    public GameObject GetEnemyPrefab(ElementTheme theme)
    {
        List<GameObject> enemies = mapper.themeToEnemiesDict[theme];
        GameObject enemy = enemies[UnityEngine.Random.Range(0, enemies.Count)];
        return enemy;
    }
    public GameObject GetMiniBossPrefab(ElementTheme theme)
    {
        List<GameObject> miniBosses = mapper.themeToMiniBossesDict[theme];
        GameObject miniBoss = miniBosses[UnityEngine.Random.Range(0, miniBosses.Count)];
        return miniBoss;
    }
    public GameObject GetBossPrefab(ElementTheme theme)
    {
        List<GameObject> bosses = mapper.themeToBossesDict[theme];
        GameObject boss = bosses[UnityEngine.Random.Range(0, bosses.Count)];
        return boss;
    }

    public List<Enemy> SpawnEnemies(DungeonRoom room, Transform transform)
    {
        SimpleEnemyInitialiser enemyInitialiser = GetComponent<SimpleEnemyInitialiser>();
        Transform roomTransform = GetComponent<Transform>();

        int enemyCount = GetCount(room.roomSize);

        List<Enemy> enemies = new List<Enemy>();

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemyPrefab = GetEnemyPrefab(room.theme);
            Enemy enemy = SpawnEnemy(room, roomTransform, i, enemyPrefab);
            enemies.Add(enemy);
        }

        if (room.roomType == RoomType.MiniBoss)
        {
            GameObject miniBossPrefab = GetMiniBossPrefab(room.theme);
            Enemy miniBoss = SpawnEnemy(room, roomTransform, 0, miniBossPrefab);
            enemies.Add(miniBoss);
        }

        if (room.roomType == RoomType.Boss)
        {
            GameObject bossPrefab = GetBossPrefab(room.theme);
            Enemy boss = SpawnEnemy(room, roomTransform, 0, bossPrefab);
            enemies.Add(boss);
        }

        return enemies;
    }

    private static Enemy SpawnEnemy(DungeonRoom room, Transform roomTransform, int i, GameObject enemyPrefab)
    {
        Vector2Int spawnPosition2D = room.GetValidSpawnPosition();

        // Convert tile position to world position
        Vector3 worldPosition = room.structure.tilemapLayers.grid.CellToWorld((Vector3Int)spawnPosition2D);
        // Center the position in the tile
        worldPosition += room.structure.tilemapLayers.grid.cellSize * 0.16f;

        // Instantiate enemy
        GameObject enemyGO = Instantiate(enemyPrefab, worldPosition, Quaternion.identity, roomTransform);

        enemyGO.name = $"Enemy_{i + 1}";

        // Ensure proper 2D rotation (no X or Y rotation)
        enemyGO.transform.rotation = Quaternion.identity;

        Enemy enemy = enemyGO.GetComponent<Enemy>();
        enemy.SetRoom(room);
        return enemy;
    }

    private int GetCount(RoomSizeSO roomSize)
    {
        foreach (var enemySpawnCount in enemySpawnCounts)
        {
            if (roomSize == enemySpawnCount.roomSizeSO)
            {
                return enemySpawnCount.count;
            }
        }
        return 0;
    }

}


[Serializable]
public class EnemySpawnCount
{
    public RoomSizeSO roomSizeSO;
    public int count;
}

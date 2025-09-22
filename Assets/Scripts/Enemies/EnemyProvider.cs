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
        Transform roomTransform = GetComponent<Transform>();

        int enemyCount = GetEnemyCount(room.structure.floorPositions.Count);

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

        Material dimmedMaterial = new Material(GameResources.Instance.dimmedMaterial);
        SpriteRenderer spriteRenderer = enemyGO.GetComponent<SpriteRenderer>();
        spriteRenderer.material = dimmedMaterial;
        
        enemy.SetRoom(room);
        return enemy;
    }

    private int GetCount(DungeonRoom room)
    {
        int enemyCount = room.structure.floorPositions.Count / 200;
        if (enemyCount <= 0) {
            return 1;
        }
        return enemyCount;
    }

    public static int GetEnemyCount(
    int tileCount,
    float density = 0.02f,     // overall scale
    float exponent = 0.80f,    // <1.0 => diminishing returns
    int minEnemies = 4,        // ensure small rooms still feel occupied
    int maxEnemies = 12        // cap huge rooms
)
    {
        // Optional: ignore the first few tiles as "free space"
        const int freeBuffer = 30;
        float effectiveTiles = Mathf.Max(0, tileCount - freeBuffer);

        // Sublinear growth
        float raw = density * Mathf.Pow(effectiveTiles, exponent);

        // Baseline body count
        raw += minEnemies - 0.5f; // shift so minEnemies is common but not guaranteed

        // Probabilistic rounding (stochastic)
        int flo = Mathf.FloorToInt(raw);
        float frac = raw - flo;
        int result = flo + (UnityEngine.Random.value < frac ? 1 : 0);

        // Clamp
        return Mathf.Clamp(result, minEnemies, maxEnemies);
    }

}


[Serializable]
public class EnemySpawnCount
{
    public RoomSizeSO roomSizeSO;
    public int count;
}

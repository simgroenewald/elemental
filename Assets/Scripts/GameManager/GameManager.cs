using NavMeshPlus.Components;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{
    [SerializeField] List<LevelSettingSO> levels;
    [SerializeField] DungeonBuilder dungeonBuilder;
    [SerializeField] NavMeshSurface playerNavMeshSurface;
    [SerializeField] NavMeshSurface enemyNavMeshSurface;
    [SerializeField] int currentLevelIndex = 0;
    private Dungeon dungeon;
    private DungeonRoom previousDungeonRoom;
    private DungeonRoom currentDungeonRoom;
    private CharacterDetailSO characterDetail;
    private Player player;
    private CameraController cameraController;

    public GameState state;

    protected override void Awake()
    {
        base.Awake();

        GameResources gameResources = GameResources.Instance;

        PlayerSO playerSO = gameResources.player;
        characterDetail = playerSO.characterDetails;

        cameraController = gameResources.cameraController;

        InstantiatePlayer();
    }

    // TODO: Maybe refactor so charachter doest get the player which then uses character to instantiate - loopy
    private void InstantiatePlayer()
    {
        GameObject playerGameObject = Instantiate(characterDetail.playerPrefab);

        player = playerGameObject.GetComponent<Player>();

        player.Initialise(characterDetail);
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = GameState.start;

        if (Input.GetKeyDown(KeyCode.R))
        {
            state = GameState.start;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleGameState();
    }

    private void HandleGameState()
    {
        switch(state)
        {
            case GameState.start:
                StartGameLevel();
                break;

        }
    }

    private void StartGameLevel()
    {
        dungeon = dungeonBuilder.GenerateDungeon(6776, levels[currentLevelIndex]);
        GameResources.Instance.dungeon = dungeon;
        DungeonRoom startRoom = dungeon.GetStartRoom();
        currentDungeonRoom = startRoom;
        previousDungeonRoom = startRoom;
        player.SetPlayerStartPosition(startRoom, startRoom.structureTilemap.tilemapLayers.grid);
        playerNavMeshSurface.BuildNavMesh();
        enemyNavMeshSurface.BuildNavMesh();
        //PlacePlayerOnNavMesh(player.transform);
        cameraController.SetupCamera(player.transform.position, startRoom.structureTilemap.tilemapLayers.baseTilemap);

        state = GameState.playing;
    }

    internal Player GetPlayer()
    {
        return player;
    }

    public void PlacePlayerOnNavMesh(Transform player)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(player.position, out hit, 1f, NavMesh.AllAreas))
        {
            player.position = hit.position; // Snap to nearest navmesh point
            Debug.Log("Player snapped to NavMesh at: " + hit.position);
        }
        else
        {
            Debug.LogError("Could not find NavMesh position for player!");
        }
    }


}

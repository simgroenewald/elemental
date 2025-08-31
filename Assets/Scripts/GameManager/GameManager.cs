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
    [SerializeField] DungeonNavigationDisplayController dungeonNavigationDisplay;
    [SerializeField] int currentLevelIndex = 0;
    private Dungeon dungeon;
    private DungeonRoom previousDungeonRoom;
    private DungeonRoom currentDungeonRoom;
    private List<DungeonRoom> completeDungeonRooms;
    private PlayerDetailsSO playerDetails;
    public Player player;
    private CameraController cameraController;

    public GameState state;

    private void OnEnable()
    {
        // Set dimmed material to off
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 0f);
    }

    private void OnDisable()
    {
        // Set dimmed material to fully visible
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 1f);
    }

    protected override void Awake()
    {

        base.Awake();

        completeDungeonRooms = new List<DungeonRoom>();

        GameResources gameResources = GameResources.Instance;

        PlayerSO playerSO = gameResources.player;
        playerDetails = playerSO.playerDetails;

        cameraController = gameResources.cameraController;

        InstantiatePlayer();
    }

    // TODO: Maybe refactor so charachter doest get the player which then uses character to instantiate - loopy
    private void InstantiatePlayer()
    {
        GameObject playerGameObject = Instantiate(playerDetails.characterPrefab);

        player = playerGameObject.GetComponent<Player>();

        player.Initialise(playerDetails);
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
        switch (state)
        {
            case GameState.start:
                StartGameLevel();
                break;

        }
    }

    private void StartGameLevel()
    {
        dungeon = dungeonBuilder.GenerateDungeon(6776, levels[currentLevelIndex]);
        dungeonNavigationDisplay.Initialise(dungeon.dungeonRooms, dungeon.connectors);
        GameResources.Instance.dungeon = dungeon;

        DungeonRoom startRoom = dungeon.GetStartRoom();
        dungeonNavigationDisplay.CompleteRoom(startRoom);
        StaticEventHandler.CallRoomChangedEvent(startRoom);


        player.SetPlayerStartPosition(startRoom, startRoom.structure.tilemapLayers.grid);
        
        // Initialize enemies BEFORE NavMesh baking so they're included as obstacles
        foreach (var room in dungeon.dungeonRooms)
        {
            if (room.roomType != RoomType.Start)
            {
                room.SpawnRoomEnemies(3);
                room.SpawnRoomItems(2);
            }
        }

        // Now bake NavMesh with all agents present
        playerNavMeshSurface.BuildNavMesh();
        enemyNavMeshSurface.BuildNavMesh();

        cameraController.SetupCamera(player.transform.position, dungeon.dungeonLayers.collisionTilemap);

        state = GameState.playing;
    }

    internal Player GetPlayer()
    {
        return player;
    }

    public DungeonRoom GetCurrentRoom()
    {
        return currentDungeonRoom;
    }

    public void SetCurrentRoom(DungeonRoom room)
    {
        currentDungeonRoom = room;
    }

    public DungeonRoom GetPreviousRoom()
    {
        return currentDungeonRoom;
    }

    public void SetPreviousRoom(DungeonRoom room)
    {
        currentDungeonRoom = room;
    }

    public List<DungeonRoom> GetCompletedDungeonRooms( )
    {
        return completeDungeonRooms;
    }

    public void AppendCompletedDungeonRooms(DungeonRoom room)
    {
        completeDungeonRooms.Add(room);
    }
}

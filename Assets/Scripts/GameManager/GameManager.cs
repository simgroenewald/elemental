using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{
    [SerializeField] List<LevelSettingSO> levels;
    [SerializeField] DungeonBuilder dungeonBuilder;
    [SerializeField] int currentLevelIndex = 0;
    private Dungeon dungeon;
    private DungeonRoom previousDungeonRoom;
    private DungeonRoom currentDungeonRoom;
    private CharacterDetailSO characterDetail;
    private Player player;

    public GameState state;

    protected override void Awake()
    {
        base.Awake();

        GameResources gameResources = GameResources.Instance;

        PlayerSO playerSO = gameResources.player;

        characterDetail = playerSO.characterDetails;

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
        DungeonRoom startRoom = dungeon.GetStartRoom();
        currentDungeonRoom = startRoom;
        previousDungeonRoom = startRoom;
        player.SetPlayerStartPosition(startRoom, dungeon.dungeonLayers.grid);
        state = GameState.playing;
    }


}

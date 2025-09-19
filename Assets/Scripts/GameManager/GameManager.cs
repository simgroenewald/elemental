using NavMeshPlus.Components;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{

    [Header("Main Game Components")]
    private PlayerDetailsSO playerDetails;
    public Player player;
    private CameraController cameraController;
    private Dungeon dungeon;

    [Header("Levels")]
    [SerializeField] List<LevelSettingSO> levels;
    public int currentLevelIndex = 0;

    [Header("Dungeon Builders")]
    [SerializeField] DungeonBuilder dungeonBuilder;
    [SerializeField] NavMeshSurface playerNavMeshSurface;
    [SerializeField] NavMeshSurface enemyNavMeshSurface;
    [SerializeField] DungeonNavigationDisplayController dungeonNavigationDisplay;

    [Header("States")]
    public GameState state;
    public GameState previousState;
    public DungeonRoom currentDungeonRoom;
    private DungeonRoom previousDungeonRoom;
    private List<DungeonRoom> completeDungeonRooms;

    [Header("UI Components")]
    public BackpackUI backpackUI;
    public ItemDetailsUI itemDetailsUI;
    public AbilitySelectorUI abilitySelectorUI;
    public AbilityUnlockedUI abilityUnlockedUI;
    public GameObject pauseMenuUI;
    [SerializeField] private CanvasGroup fadeScreen;
    [SerializeField] private TextMeshProUGUI screenMessage;

    [Header("Abaility Bools")]
    private bool firstTimeLoad = false;
    public bool newAbilityUnlocked = false;

    [Header("State Sounds")]
    [SerializeField] SoundEffectSO bossRoomUnlockedSound;
    [SerializeField] SoundEffectSO levelCompleteSound;

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

        firstTimeLoad = true;
        newAbilityUnlocked = false;

        fadeScreen.alpha = 1f;
        screenMessage.SetText("");

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
        previousState = GameState.none;
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
            case GameState.restart:
                RestartGame();
                break;
            case GameState.start:
                if (previousState != GameState.start)
                {
                    StartCoroutine(StartGameLevel());
                }
                break;
            case GameState.playing:
                HandlePlaying();
                break;
            case GameState.bossRoom:
                if (previousState != GameState.bossRoom)
                {
                    StartCoroutine(StartBossFight());
                }
                break;
            case GameState.paused:
                PauseGame();
                break;
            case GameState.levelPassed:
                StartCoroutine(CompleteLevel());
                break;
            case GameState.won:
                if (previousState != GameState.won)
                {
                    StartCoroutine(GameWon());
                }
                break;
            case GameState.lost:
                if (previousState != GameState.lost)
                {
                    StopAllCoroutines();
                    StartCoroutine(GameLost());
                }
                break;

        }
    }

    private IEnumerator GameWon()
    {

        previousState = GameState.won;
        yield return StartCoroutine(Fade(0f, 1f, 2f, Color.black));

        player.DisablePlayer();
        dungeonBuilder.Clear();
        playerNavMeshSurface.RemoveData();

        yield return StartCoroutine(DisplayMessageRoutine("WELL DONE! \n\n You have completed the dungeon!", 3f));
        yield return StartCoroutine(DisplayMessageRoutine("You scored 1000", 4f));
        yield return StartCoroutine(DisplayMessageRoutine("Press enter to restart the game", 0f));

        while (!Input.GetKeyDown(KeyCode.Return))
        {
            state = GameState.restart;
            yield break;
        }
    }


    private IEnumerator GameLost()
    {
        previousState = GameState.lost;

        yield return StartCoroutine(Fade(0f, 1f, 1f, Color.black));

        player.DisablePlayer();
        dungeonBuilder.Clear();
        playerNavMeshSurface.RemoveData();

        yield return StartCoroutine(DisplayMessageRoutine("You were defeated", 3f));
        yield return StartCoroutine(DisplayMessageRoutine("Press enter to restart the game", 0f));

        while (!Input.GetKeyDown(KeyCode.Return))
        {
            state = GameState.restart;
            yield break;
        }

    }

    private IEnumerator CompleteLevel()
    {
        if (currentLevelIndex == levels.Count - 1)
        {
            state = GameState.won;
            yield break;
        }

        previousState = GameState.levelPassed;
        state = GameState.playing;
        SoundEffectManager.Instance.PlaySoundEffect(levelCompleteSound);
        yield return StartCoroutine(Fade(0f, 1f, 1f, new Color(0f, 0f, 0f, 0.4f)));
        yield return StartCoroutine(DisplayMessageRoutine("WELL DONE! \n\n You have survived this dungeon", 5f));
        yield return StartCoroutine(DisplayMessageRoutine("Collect any remaining items then \n\npress enter to continue to the next dungeon", 5f));
        yield return StartCoroutine(Fade(1f, 0f, 1f, new Color(0f, 0f, 0f, 0.4f)));

        while (!Input.GetKeyDown(KeyCode.Return))
        {
            yield return null;
        }

        yield return null;

        currentLevelIndex++;
        state = GameState.start;
    }

    private void RestartGame()
    {
        firstTimeLoad = true;
        newAbilityUnlocked = false;
        player.backpackController.backpack.ResetBackpack();
        player.stats.ResetStats();
        currentLevelIndex = 0;
        state = GameState.start;
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HidePauseGameMenu();
        }
    }

    private void HandlePlaying()
    {
        Time.timeScale = 1f;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DisplayPauseGameMenu();
        }
    }

    public void DisplayPauseGameMenu()
    {
        if (state != GameState.paused)
        {
            pauseMenuUI.SetActive(true);
            state = GameState.paused;
        }
    }

    public void HidePauseGameMenu()
    {
        pauseMenuUI.SetActive(false);
        state = GameState.playing;
    }

    private IEnumerator StartBossFight()
    {
        previousState = GameState.bossRoom;
        state = GameState.playing;
        SoundEffectManager.Instance.PlaySoundEffect(bossRoomUnlockedSound);
        yield return StartCoroutine(Fade(0f, 1f, 1f, new Color(0f, 0f, 0f, 0.4f)));
        yield return StartCoroutine(DisplayMessageRoutine("The boss room has been unlocked.", 5f));
        yield return StartCoroutine(DisplayMessageRoutine("Find and defeat the boss\n\nto unlock your next ability", 5f));
        yield return StartCoroutine(Fade(1f, 0f, 1f, new Color(0f, 0f, 0f, 0.4f)));

        while (!Input.GetKeyDown(KeyCode.Return))
        {
            yield return null;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DisplayPauseGameMenu();
        }

        state = GameState.playing;
    }

    private IEnumerator StartGameLevel()
    {
        previousState = GameState.start;

        if (!firstTimeLoad)
        {
            DisplayDungeonLevelText();
            StartCoroutine(Fade(0f, 1f, 2f, Color.black));
            yield return new WaitForSeconds(2f);
        } else
        {
            DisplayDungeonLevelText();
            yield return new WaitForSeconds(2f);
        }

        player.DisablePlayer();
        dungeonBuilder.Clear();
        playerNavMeshSurface.RemoveData();

        dungeon = dungeonBuilder.GenerateDungeon(6776, levels[currentLevelIndex]);
        //dungeon = dungeonBuilder.GenerateDungeon(8543, levels[currentLevelIndex]);

        completeDungeonRooms = new List<DungeonRoom>();
        dungeonNavigationDisplay.Initialise(dungeon.dungeonRooms, dungeon.connectors);
        GameResources.Instance.dungeon = dungeon;

        DungeonRoom startRoom = dungeon.GetStartRoom();
        startRoom.CompleteRoom();
        StaticEventHandler.CallRoomEnteredEvent(startRoom);

        player.SetPlayerStartPosition(startRoom, startRoom.structure.tilemapLayers.grid);

        // Initialize enemies BEFORE NavMesh baking so they're included as obstacles
        foreach (var room in dungeon.dungeonRooms)
        {
            if (room.roomType != RoomType.Start)
            {
                room.SpawnRoomEnemies(1);
                //room.SpawnRoomItems(2);
            }
        }

        // Now bake NavMesh with all agents present
        playerNavMeshSurface.BuildNavMesh();
        player.EnablePlayer();

        cameraController.SetupCamera(player.transform.position, dungeon.dungeonLayers.collisionTilemap);

        StartCoroutine(Fade(1f, 0f, 2f, Color.black));
        screenMessage.SetText("");
        firstTimeLoad = false;
        newAbilityUnlocked = false;

        state = GameState.playing;
    }

    public void SetStateCompleteLevel()
    {
        // Check that all enemies are defeted and all rooms are complete
        if (completeDungeonRooms.Count == dungeon.dungeonRooms.Count && newAbilityUnlocked && state != GameState.levelPassed)
        {
            state = GameState.levelPassed;
        }
    }

    public void SetStateGameOver()
    {
        if (state != GameState.lost)
        {
            state = GameState.lost;
        }
    }

    private void DisplayDungeonLevelText()
    {
        string messageText = "LOADING LEVEL " + (currentLevelIndex + 1).ToString();
        screenMessage.SetText(messageText);
    }

    private IEnumerator DisplayMessageRoutine(string text, float displaySeconds)
    {
        screenMessage.SetText(text);

        if (displaySeconds > 0f)
        {
            float timer = displaySeconds;

            while (timer > 0f && !Input.GetKeyDown(KeyCode.Return))
            {
                timer -= Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            while (!Input.GetKeyDown(KeyCode.Return))
            {
                yield return null;
            }
        }

        yield return null;

        screenMessage.SetText("");
    }

    public IEnumerator Fade(float startFadeAlpha, float targetFadeAlpha, float fadeSeconds, Color backgroundColour)
    {
        Image image = fadeScreen.GetComponent<Image>();
        image.color = backgroundColour;

        float time = 0;

        while (time < fadeSeconds)
        {
            time += Time.deltaTime;
            fadeScreen.alpha = Mathf.Lerp(startFadeAlpha, targetFadeAlpha, time / fadeSeconds);
            yield return null;
        }
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
        if (previousState == GameState.bossRoom)
        {
            SetStateCompleteLevel();
        }
        if (completeDungeonRooms.Count == dungeon.dungeonRooms.Count - 1)
        {
            state = GameState.bossRoom;
        }
    }
}

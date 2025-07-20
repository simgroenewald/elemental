using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{
    [SerializeField] List<LevelSettingSO> levels;
    [SerializeField] int currentLevelIndex = 0;
    public GameState state;

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
                //DungeonBuilder.GenerateDungeon(1667, levels[currentLevelIndex]);
                state = GameState.playing;
                break;

        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
[RequireComponent(typeof(ItemCollectionSystem))]
[RequireComponent(typeof(BackpackController))]
[RequireComponent(typeof(ItemSelectorController))]
[RequireComponent(typeof(AbilityController))]
public class Player : Character
{

    public PlayerDetailsSO playerDetails;
    [HideInInspector] public ItemCollectionSystem itemCollectionSystem;
    [HideInInspector] public BackpackController backpackController;
    [HideInInspector] public ItemSelectorController itemSelectorController;
    [HideInInspector] public AbilityController abilityController;
    [HideInInspector] public bool isMovementDisabled;

    protected override void Awake()
    {
        base.Awake();
        itemCollectionSystem = GetComponent<ItemCollectionSystem>();
        backpackController = GetComponent<BackpackController>();
        itemSelectorController = GetComponent<ItemSelectorController>();
        abilityController = GetComponent<AbilityController>();
    }

    private void Start()
    {
        Initialise(playerDetails);
    }

    public void Initialise(PlayerDetailsSO playerDetails)
    {
        this.playerDetails = playerDetails;

        stats.Initialise(playerDetails.statsSO);

        base.Initialise(playerDetails);
    }

    public void SetPlayerStartPosition(DungeonRoom room, Grid grid)
    {
        Vector2Int randomPos = room.structure.floorPositions.ElementAt(UnityEngine.Random.Range(0, room.structure.floorPositions.Count));
        base.SetCharacterPosition(randomPos, grid);
    }

    public void DisablePlayer()
    {
        isMovementDisabled = true;
        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.velocity = Vector3.zero;
            agent.enabled = false;
        }
        Rigidbody2D rigidBody = gameObject.GetComponent<Rigidbody2D>();
        rigidBody.linearVelocity = Vector3.zero;
    }

    public void EnablePlayer()
    {
        isMovementDisabled = false;
        agent.enabled = true;
        characterState.Reset();
        animateCharacter.ResetAnimation();
        health.SetHealth();
        mana.SetMana();
    }

}

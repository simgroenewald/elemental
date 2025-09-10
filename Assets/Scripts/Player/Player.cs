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
public class Player : Character
{

    public PlayerDetailsSO playerDetails;
    [HideInInspector] public ItemCollectionSystem itemCollectionSystem;
    [HideInInspector] public BackpackController backpackController;
    [HideInInspector] public ItemSelectorController itemSelectorController;

    protected override void Awake()
    {
        base.Awake();
        itemCollectionSystem = GetComponent<ItemCollectionSystem>();
        backpackController = GetComponent<BackpackController>();
        itemSelectorController = GetComponent<ItemSelectorController>();
    }

    private void Start()
    {
        Initialise(playerDetails);
    }

    public void Initialise(PlayerDetailsSO playerDetails)
    {
        this.playerDetails = playerDetails;

        base.Initialise(playerDetails);

    }

    public void SetPlayerStartPosition(DungeonRoom room, Grid grid)
    {
        Vector2Int randomPos = room.structure.floorPositions.ElementAt(UnityEngine.Random.Range(0, room.structure.floorPositions.Count));
        base.SetCharacterPosition(randomPos, grid);
    }

}

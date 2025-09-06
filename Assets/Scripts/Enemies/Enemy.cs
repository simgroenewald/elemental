using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
public class Enemy : Character
{
    public EnemyDetailsSO enemyDetails;
    public Stats enemyStats;
    public DungeonRoom room;
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        Initialise();
    }

    protected virtual void Initialise()
    {
        base.Initialise(enemyDetails);
    }

    public void SetRoom(DungeonRoom room)
    {
        this.room = room;
    }
}

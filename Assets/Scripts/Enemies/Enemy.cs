using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
public class Enemy : Character
{
    public EnemyDetailsSO enemyDetails;
    public DungeonRoom room;
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        Initialise(enemyDetails);
    }

    protected virtual void Initialise(EnemyDetailsSO enemyDetails)
    {
        this.enemyDetails = enemyDetails;

        base.Initialise(enemyDetails);

    }

    public void SetRoom(DungeonRoom room)
    {
        this.room = room;
    }
}

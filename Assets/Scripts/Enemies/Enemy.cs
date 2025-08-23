using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
public class Enemy : Character
{
    public EnemyDetailsSO enemyDetails;
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
}

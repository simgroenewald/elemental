using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class MeleeAbility : MonoBehaviour
{
    private AbilityEvents abilityEvents;

    private void Awake()
    {
        abilityEvents = GetComponent<AbilityEvents>();
    }

    private void MeleeAttack()
    {
        //float distanceFromPlayer = Vector2.Distance(player.transform.position, enemy.transform.position);
/*        if (distanceFromPlayer < attackRange && isAttacking)
        {
            player.healthEvents.RaiseMelee
        }*/
    }

    private void EndAttack()
    {
        abilityEvents.RaiseMeleeEndAttackEvent();
    }
}

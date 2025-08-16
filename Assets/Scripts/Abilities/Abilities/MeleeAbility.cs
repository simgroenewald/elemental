using System;
using UnityEngine;

public class MeleeAbility : MonoBehaviour
{
    private AbilityEvents abilityEvents;

    private void Awake()
    {
        abilityEvents = GetComponent<AbilityEvents>();
    }

    private void MeleeAttack()
    {
        // Play an attack animation

        // Detect enemies in range of the attack

        // Damage enemy
    }

    private void EndAttack()
    {
        abilityEvents.RaiseMeleeEndAttackEvent();
    }
}

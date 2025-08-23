using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class MeleeAbility : MonoBehaviour
{
    private Character character;
    private AbilityEvents abilityEvents;
    private CharacterCombat characterCombat;

    private void Awake()
    {
        character = GetComponent<Character>();
        abilityEvents = GetComponent<AbilityEvents>();
        characterCombat = GetComponent<CharacterCombat>();
    }

    private void MeleeAttack()
    {
        float distanceFromTarget = Vector2.Distance(characterCombat.currentTarget.transform.position, transform.position);
        if (distanceFromTarget <= characterCombat.attackRange)
        {
            characterCombat.currentTarget.healthEvents.RaiseReduceHealthEvent(characterCombat.currentAbility.abilityDetails.physicalDamage);
        }
    }

    private void EndAttack()
    {
        abilityEvents.RaiseMeleeEndAttackEvent();
    }
}

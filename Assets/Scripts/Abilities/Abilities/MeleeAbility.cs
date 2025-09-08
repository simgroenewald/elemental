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
        if (distanceFromTarget <= characterCombat.currentAbility.abilityDetails._range)
        {
            float damage;
            if (characterCombat.currentAbility.abilityDetails.isCritical)
            {
                damage = characterCombat.currentAbility.EvaluateDamageDealingStats(characterCombat.currentAbility.abilityDetails._damage);
            }
            else
            {
                damage = characterCombat.currentAbility.abilityDetails._damage;
            }
            damage = characterCombat.currentTarget.stats.EvaluateDamageTakingStats(damage);
            characterCombat.currentTarget.healthEvents.RaiseReduceHealthEvent(damage);
        }
    }

    private void EndAttack()
    {
        abilityEvents.RaiseMeleeEndAttackEvent();
    }
}

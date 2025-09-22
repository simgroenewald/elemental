using UnityEngine;

public class MeleeAbility : MonoBehaviour
{
    private Character character;
    private AbilityEvents abilityEvents;
    private CharacterCombat characterCombat;
    private ActiveAbility activeAbility;

    private void Awake()
    {
        character = GetComponent<Character>();
        abilityEvents = GetComponent<AbilityEvents>();
        characterCombat = GetComponent<CharacterCombat>();
        activeAbility = GetComponent<ActiveAbility>();
    }

    private void MeleeAttack()
    {
        if (!characterCombat.currentTarget || characterCombat.currentTarget.characterState.isDead || characterCombat.currentTarget.characterState.isDying)
        {
            EndAttack();
            return;
        }
        float distanceFromTarget = Vector2.Distance(characterCombat.currentTarget.transform.position, transform.position);
        SoundEffectManager.Instance.PlaySoundEffect(activeAbility.currentAbility.abilityDetails.abilityAttackSound);
        if (distanceFromTarget <= activeAbility.currentAbility.abilityDetails._range)
        {
            DealDamage(characterCombat.currentTarget);
        }

    }

    private void MeleeMultiAttack()
    {
        if (activeAbility.currentAbility.abilityDetails.isMultiTarget)
        {
            SoundEffectManager.Instance.PlaySoundEffect(activeAbility.currentAbility.abilityDetails.abilityAttackSound);
            foreach (var characterTarget in characterCombat.currentTargets)
            {
                if (characterTarget == null && characterTarget.characterState.isDead || characterTarget.characterState.isDying)
                {
                    continue;
                }
                DealDamage(characterTarget);
            }
        }
    }

    private void DealDamage(Character target)
    {
        float damage;
        if (activeAbility.currentAbility.abilityDetails.isHurt)
        {
            target.characterState.SetToHurt();
            target.movementEvents.RaiseHurt();
        }
        if (activeAbility.currentAbility.abilityDetails.isCritical)
        {
            damage = activeAbility.currentAbility.EvaluateDamageDealingStats(character, activeAbility.currentAbility.abilityDetails._damage);
        }
        else
        {
            damage = activeAbility.currentAbility.abilityDetails._damage;
        }
        if (character is Player)
        {
            Score.Instance.IncreaseDamageDealt((int)damage);
        }
        damage = target.stats.EvaluateDamageTakingStats(damage);
        if (target is Player)
        {
            Score.Instance.IncreaseDamageTaken((int)damage);
        }
        target.healthEvents.RaiseReduceHealthEvent(damage);
        if (character.stats.GetStat(StatType.HealthSteal) > 0)
        {
            float health = character.stats.GetStat(StatType.HealthSteal) * damage / 100;
            character.healthEvents.RaiseIncreaseHealthEvent(health);
        }
    }

    private void EndAttack()
    {
        abilityEvents.RaiseMeleeEndAttackEvent();
    }
}

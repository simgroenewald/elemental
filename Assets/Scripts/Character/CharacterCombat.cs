using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Character))]
[RequireComponent(typeof(CharacterState))]
[RequireComponent(typeof(CharacterMovement))]
public class CharacterCombat : MonoBehaviour
{
    private Character character;
    private CharacterState characterState;
    private CharacterMovement characterMovement;
    public ActiveAbility activeAbility;

    public Character currentTarget;
    public List<Character> currentTargets;

    // Used for enemy auto attack if attacked by player
    public bool attackTriggered = false;
    public bool normalAdvance = false;

    private void Awake()
    {
        // Load components
        character = GetComponent<Character>();
        characterState = GetComponent<CharacterState>();
        characterMovement = GetComponent<CharacterMovement>();
        activeAbility = GetComponent<ActiveAbility>();
        currentTargets = new List<Character>();
    }

    private void Start()
    {
        SetPlayerAttackAnimationSpeed();
    }

    public void SetPlayerAttackAnimationSpeed()
    {
        character.animator.SetFloat("attackSpeed", character.stats.GetStat(StatType.AttackSpeed));
    }

    public bool SetMovementSpeedForAttack(Character targetCharacter)
    {
        if (activeAbility.currentAbility.abilityDetails.slowDownBuffer != 0 && targetCharacter != null)
        {
            float dist = Vector3.Distance(transform.position, targetCharacter.transform.position);
            // Slow down at buffer distance
            if (dist < activeAbility.currentAbility.abilityDetails._range + activeAbility.currentAbility.abilityDetails.slowDownBuffer)
            {
                character.agent.speed = 1f;
            }
            else
            {
                character.agent.speed = characterMovement.movementSpeed;
            }
            return true;
        }
        return false;
    }

    public void AttemptAttack(Character characterTarget, bool isAutoAttack, float lineOfSight)
    {
        currentTarget = characterTarget;
        float distanceFromTarget = Vector2.Distance(characterTarget.transform.position, character.transform.position);
        // If player is in range and target is selected
        if (distanceFromTarget < activeAbility.currentAbility.abilityDetails._range && characterTarget != null && !characterState.isAttacking)
        {
            if (character.activeAbility.currentAbility.abilityDetails.isMultiTarget)
            {
                AttackEnemies(currentTarget);
            } else
            {
                AttackEnemy(characterTarget);
            }
        }
        // If player is not in range and target is selected
        else if (characterTarget != null && !characterState.isAttacking)
        {
            if (isAutoAttack && distanceFromTarget < lineOfSight || !isAutoAttack && distanceFromTarget >= activeAbility.currentAbility.abilityDetails._range)
            {
                characterMovement.MoveToPosition(characterTarget.transform.position);
            }
        }
    }

    public void TriggerAbility()
    {
        if (character.activeAbility.currentAbility.abilityDetails.isMultiTarget)
        {
            AttackEnemies();
        }
    }

    private void GetAllReachableTargets()
    {
        List<Enemy> allRoomEnemies = GameManager.Instance.currentDungeonRoom.enemies;
        foreach (var enemy in allRoomEnemies)
        {
            if (!enemy.characterState.isDying && !enemy.characterState.isDead)
            {
                float distanceFromTarget = Vector2.Distance(enemy.transform.position, character.transform.position);
                // If player is in range and target is selected
                if (distanceFromTarget < activeAbility.currentAbility.abilityDetails._range)
                {
                    currentTargets.Add(enemy);
                }
            }
        }
    }

    public void AttackEnemies(Character mainTarget = null)
    {
        GetAllReachableTargets();
        characterMovement.StopMovement();
        characterState.SetToAttacking();
        character.movementEvents.RaiseAttack();
        if (mainTarget != null)
        {
            Vector3 enemyPosition = mainTarget.target.position;

            Vector3 playerDirection = (enemyPosition - character.transform.position);
            float playerAngle = HelperUtilities.GetAngleFromVector(playerDirection);
            TargetDirection aimDirection = HelperUtilities.GetTargetDirection(playerAngle);

            characterMovement.UpdateCharacterDirection(aimDirection);
        }
        if(activeAbility.currentAbility.abilityDetails.isRanged)
        {
            if (activeAbility.currentAbility.abilityDetails.isCastAtTarget)
            {
                if (!activeAbility.currentAbility.AttemptToUseSuccessful(character)) return;
                MultiCastAtTargetAttack(currentTargets);
            }
        }
        else
        {
            if (!activeAbility.currentAbility.AttemptToUseSuccessful(character)) return;
            MeleeAttack();
        }

    }

    public void AttackEnemy(Character characterTarget)
    {
        characterMovement.StopMovement();
        characterState.SetToAttacking();

        character.movementEvents.RaiseAttack();
        Vector3 enemyPosition = characterTarget.target.position;

        Vector3 playerDirection = (enemyPosition - character.transform.position);
        float playerAngle = HelperUtilities.GetAngleFromVector(playerDirection);
        TargetDirection aimDirection = HelperUtilities.GetTargetDirection(playerAngle);

        characterMovement.UpdateCharacterDirection(aimDirection);

        if (!activeAbility.currentAbility.AttemptToUseSuccessful(character)) return;

        if (activeAbility.currentAbility.abilityDetails.isRanged)
        {
            if (activeAbility.currentAbility.abilityDetails.isCastAtTarget)
            {
                CastAtTargetAttack(characterTarget);
            }
            else if (activeAbility.currentAbility.abilityDetails.isEnemyTargetable)
            {
                RangedTargetAttack(aimDirection, enemyPosition, playerAngle, characterTarget);
            }
            else
            {
                RangedAttack(aimDirection, enemyPosition, playerAngle, characterTarget);
            }
        }
        else
        {
            MeleeAttack();
        }
    }


    private void RangedTargetAttack(TargetDirection aimDirection, Vector3 enemyPosition, float playerAngle, Character characterTarget)
    {
        Vector3 castPosition = character.activeAbility.GetCastPosition(aimDirection);
        // Ensure we're working in 2D by setting Z to 0 for both positions
        Vector3 castPosition2D = new Vector3(castPosition.x, castPosition.y, 0f);
        Vector3 worldPosition2D = new Vector3(enemyPosition.x, enemyPosition.y, 0f);

        Vector3 castPointDirection = (worldPosition2D - castPosition2D);
        float castPointAngle = HelperUtilities.GetAngleFromVector(castPointDirection);

        character.abilitySetupEvent.RaiseSingleMovementAbilitySetupEvent(true, aimDirection, playerAngle, castPointAngle, castPointDirection, character, characterTarget);

        character.abilityEvents.RaiseCastAbilityEvent();
    }

    private void CastAtTargetAttack(Character characterTarget)
    {
        character.abilitySetupEvent.RaiseSingleStaticAbilitySetupEvent(character, characterTarget);
        character.abilityEvents.RaiseCastAbilityEvent();
    }

    private void MultiCastAtTargetAttack(List<Character> currentTargets)
    {

        character.abilitySetupEvent.RaiseMultiAbilitySetupEvent(character, currentTargets);
        character.abilityEvents.RaiseCastAbilityEvent();
    }

    private void MeleeAttack()
    {
        character.abilityEvents.RaiseMeleeAttackEvent();
    }

    private void RangedAttack(TargetDirection aimDirection, Vector3 enemyPosition, float playerAngle, Character characterTarget)
    {
        Vector3 castPosition = character.activeAbility.GetCastPosition(aimDirection);
        // Ensure we're working in 2D by setting Z to 0 for both positions
        Vector3 castPosition2D = new Vector3(castPosition.x, castPosition.y, 0f);
        Vector3 worldPosition2D = new Vector3(enemyPosition.x, enemyPosition.y, 0f);

        Vector3 castPointDirection = (worldPosition2D - castPosition2D);
        float castPointAngle = HelperUtilities.GetAngleFromVector(castPointDirection);

        character.abilitySetupEvent.RaiseSingleMovementAbilitySetupEvent(true, aimDirection, playerAngle, castPointAngle, castPointDirection, character, null);
        character.abilityEvents.RaiseCastAbilityEvent();
    }

    // World pos cast target
/*    private void CastAbility()
    {
        if (Input.GetMouseButton(0))
        {
            //if (player.activeAbility.isCooledDown()) {
            StopMovement();
            SetToAttacking();
            player.movementEvents.RaiseAttack();
            Vector3 worldPosition = GetClickPosition();

            Vector3 playerDirection = (worldPosition - player.transform.position);
            float playerAngle = HelperUtilities.GetAngleFromVector(playerDirection);
            TargetDirection aimDirection = HelperUtilities.GetTargetDirection(playerAngle);

            Vector3 castPosition = player.activeAbility.GetCastPosition(aimDirection);
            // Ensure we're working in 2D by setting Z to 0 for both positions
            Vector3 castPosition2D = new Vector3(castPosition.x, castPosition.y, 0f);
            Vector3 worldPosition2D = new Vector3(worldPosition.x, worldPosition.y, 0f);

            Vector3 castPointDirection = (worldPosition2D - castPosition2D);
            float castPointAngle = HelperUtilities.GetAngleFromVector(castPointDirection);

            UpdatePlayerDirection(aimDirection);

            if (currentAbility.abilityDetails.isEnemyTargetable)
            {
                player.abilitySetupEvent.RaiseAbilitySetupEvent(true, aimDirection, playerAngle, castPointAngle, castPointDirection, targetEnemy.enemy);
            }
            else
            {
                player.abilitySetupEvent.RaiseAbilitySetupEvent(true, aimDirection, playerAngle, castPointAngle, castPointDirection, null);
            }
            player.abilityEvents.RaiseCastAbilityEvent();
        }
    }*/

}
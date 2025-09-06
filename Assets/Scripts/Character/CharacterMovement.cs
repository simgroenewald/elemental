using GLTFast.Schema;
using UnityEngine;

[RequireComponent(typeof(Character))]
[RequireComponent(typeof(CharacterState))]
[RequireComponent(typeof(CharacterCombat))]
public class CharacterMovement : MonoBehaviour
{
    private Character character;
    private CharacterState characterState;
    private CharacterCombat characterCombat;

    private Vector3 lastPosition;
    public float movementSpeed;

    private void Awake()
    {
        // Load components
        character = GetComponent<Character>();
        characterState = GetComponent<CharacterState>();
        characterCombat = GetComponent<CharacterCombat>();

        character.agent.updateRotation = false;
        character.agent.updateUpAxis = false;
    }

    private void Start()
    {
        character.abilityEvents.OnAbilityCasted += HandleIdleState;
        character.abilityEvents.OnMeleeEndAttack += HandleIdleState;

        movementSpeed = character.characterDetails.stats.GetStat(StatType.MovementSpeed);
        SetAgentSpeed(movementSpeed);
        SetPlayerMovmentAnimationSpeed();
    }

    private void OnDisable()
    {
        character.abilityEvents.OnAbilityCasted -= HandleIdleState;
        character.abilityEvents.OnMeleeEndAttack -= HandleIdleState;
    }

    private void SetPlayerMovmentAnimationSpeed()
    {
        float animMovementSpeed = movementSpeed / Settings.baseSpeedForPlayerAnimations;
        character.animator.SetFloat("movementSpeed", animMovementSpeed);
    }

    public void UpdateSpeed(Character characterTarget)
    {
        bool speedUpdatedForAttack = false;

        // If the ability requires slow down time
        if (characterTarget != null) {
            speedUpdatedForAttack = characterCombat.SetMovementSpeedForAttack(characterTarget);
        }

        if (character.agent.speed != movementSpeed && !speedUpdatedForAttack)
        {
            character.agent.speed = movementSpeed;
        }
    }

    public void SetAgentSpeed(float movementSpeed)
    {
        character.agent.speed = movementSpeed;
    }

    public void MoveToPosition(Vector2 position)
    {
        character.agent.SetDestination(position);

        if (!characterState.isMoving)
        {
            characterState.SetToMoving();
            character.movementEvents.RaiseMoveByPosition(movementSpeed);
            StartTracking();
        }
    }


    public void StopMovement()
    {
        character.agent.ResetPath();
        StopTracking();
    }


    public void UpdateCharacterDirection(TargetDirection direction)
    {
        if (direction == TargetDirection.Left)
        {
            character.movementEvents.RaiseFaceLeft();
            characterState.posTargetRight = false;
            characterState.posTargetLeft = true;
        }
        else
        {
            character.movementEvents.RaiseFaceRight();
            characterState.posTargetRight = true;
            characterState.posTargetLeft = false;
        }
    }

    private void StartTracking()
    {
        lastPosition = character.transform.position;
    }

    private void StopTracking()
    {
        HandleIdleState();
    }

    private void HandleIdleState()
    {
        characterState.SetToIdle();
        character.movementEvents.RaiseIdle();
    }

    public void UpdateCharacterMovement()
    {
        {
            if (!characterState.isMoving || character.agent.pathPending) return;

            Vector3 currentPosition = character.transform.position;
            Vector3 delta = currentPosition - lastPosition;

            if (currentPosition.x >= lastPosition.x && !characterState.posTargetRight)
            {
                character.movementEvents.RaiseFaceRight();
                characterState.posTargetRight = true;
                characterState.posTargetLeft = false;
            }
            else if (currentPosition.x < lastPosition.x && !characterState.posTargetLeft)
            {
                character.movementEvents.RaiseFaceLeft();
                characterState.posTargetRight = false;
                characterState.posTargetLeft = true;
            }

            lastPosition = currentPosition;

            // Stop animation early if somehow movement stops unexpectedly
            if (character.agent.remainingDistance <= character.agent.stoppingDistance && character.agent.velocity.sqrMagnitude < 0.01f)
            {
                StopTracking();
            }
        }
    }

}
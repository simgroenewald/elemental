using UnityEngine;

[RequireComponent(typeof(Character))]
[RequireComponent(typeof(CharacterState))]
[RequireComponent(typeof(CharacterMovement))]
public class CharacterCombat : MonoBehaviour
{
    private Character character;
    private CharacterState characterState;
    private CharacterMovement characterMovement;

    private int currentAbilityIndex = 1;
    public Ability currentAbility;
    private float slowDownBuffer;
    public float attackRange;
    public Character currentTarget;

    // Used for enemy auto attack if attacked by player
    public bool attackTriggered = false;
    public bool normalAdvance = false;

    private void Awake()
    {
        // Load components
        character = GetComponent<Character>();
        characterState = GetComponent<CharacterState>();
        characterMovement = GetComponent<CharacterMovement>();
    }

    private void Start()
    {
        SetCharacterStartAbility();
        SetPlayerAttackAnimationSpeed();
    }

    private void SetPlayerAttackAnimationSpeed()
    {
        character.animator.SetFloat("attackSpeed", character.stats.GetStat(StatType.AttackSpeed));
    }

    public bool SetMovementSpeedForAttack(Character targetCharacter)
    {
        if (slowDownBuffer != 0 && targetCharacter != null)
        {
            float dist = Vector3.Distance(transform.position, targetCharacter.transform.position);
            // Slow down at buffer distance
            if (dist < attackRange + slowDownBuffer)
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

    private void SetCharacterStartAbility()
    {
        int index = 1;
        foreach (Ability ability in character.abilityList)
        {
            if (ability.abilityDetails == character.characterDetails.startingAbility)
            {
                SetAbilityByIndex(character, index);
                break;
            }
            index++;
        }
    }

    private void SetAbilityByIndex(Character character, int abilityIndex)
    {
        if (abilityIndex - 1 < character.abilityList.Count)
        {
            currentAbilityIndex = abilityIndex;
            character.setActiveAbilityEvent.CallSetActiveAbilityEvent(character.abilityList[abilityIndex - 1]);
        }
        currentAbility = character.activeAbility.GetCurrentAbility();
        slowDownBuffer = currentAbility.abilityDetails.slowDownBuffer;
        attackRange = currentAbility.abilityDetails.range;
    }

    public void AttemptAttack(Character characterTarget, bool isAutoAttack, float lineOfSight)
    {
        currentTarget = characterTarget;
        float distanceFromTarget = Vector2.Distance(characterTarget.transform.position, character.transform.position);
        // If player is in range and target is selected
        if (distanceFromTarget < attackRange && characterTarget != null && !characterState.isAttacking)
        {
            AttackEnemy(characterTarget);
        }
        // If player is not in range and target is selected
        else if (characterTarget != null && !characterState.isAttacking)
        {
            if (isAutoAttack && distanceFromTarget < lineOfSight || !isAutoAttack && distanceFromTarget >= attackRange)
            {
                characterMovement.MoveToPosition(characterTarget.transform.position);
            }
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
        if (currentAbility.abilityDetails.isRanged)
        {
            RangedAttack(aimDirection, enemyPosition, playerAngle, characterTarget);
        }
        else
        {
            character.abilityEvents.RaiseMeleeAttackEvent();
        }
    }

    private void RangedAttack(TargetDirection aimDirection, Vector3 enemyPosition, float playerAngle, Character characterTarget)
    {
        Vector3 castPosition = character.activeAbility.GetCastPosition(aimDirection);
        // Ensure we're working in 2D by setting Z to 0 for both positions
        Vector3 castPosition2D = new Vector3(castPosition.x, castPosition.y, 0f);
        Vector3 worldPosition2D = new Vector3(enemyPosition.x, enemyPosition.y, 0f);

        Vector3 castPointDirection = (worldPosition2D - castPosition2D);
        float castPointAngle = HelperUtilities.GetAngleFromVector(castPointDirection);


        if (currentAbility.abilityDetails.isEnemyTargetable)
        {
            character.abilitySetupEvent.RaiseAbilitySetupEvent(true, aimDirection, playerAngle, castPointAngle, castPointDirection, characterTarget);
        }
        else
        {
            character.abilitySetupEvent.RaiseAbilitySetupEvent(true, aimDirection, playerAngle, castPointAngle, castPointDirection, null);
        }
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
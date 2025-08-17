using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Enemy))]
public class EnemyAutoController : MonoBehaviour
{
    private Enemy enemy;
    private Player player;
    private NavMeshAgent agent;
    public float lineOfSight;
    public float attackRange;
    private float moveSpeed;
    private float slowDownBuffer;
    private Ability currentAbility;

    private Vector3 lastPosition;
    public bool isMoving = false;
    public bool isAttacking = false;
    public bool isIdle = false;
    public bool posTargetLeft = false;
    public bool posTargetRight = false;


    private void Awake()
    {
        // Load components
        enemy = GetComponent<Enemy>();
        player = GameManager.Instance.GetPlayer();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Start()
    {
        enemy.abilityEvents.OnAbilityCasted += HandleIdleState;
        enemy.abilityEvents.OnMeleeEndAttack += HandleIdleState;

        moveSpeed = enemy.enemyDetails.moveSpeed;
        lineOfSight = enemy.enemyDetails.lineOfSight;

        currentAbility = enemy.activeAbility.GetCurrentAbility();
        attackRange = currentAbility.abilityDetails.range;
        slowDownBuffer = currentAbility.abilityDetails.slowDownBuffer;

    }

    private void OnDisable()
    {
        player.abilityEvents.OnAbilityCasted -= HandleIdleState;
        enemy.abilityEvents.OnMeleeEndAttack -= HandleIdleState;
    }

    void FixedUpdate()
    {
        // If the ability requires slow down time
        if (slowDownBuffer != 0)
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            // Slow down at buffer distance
            if (dist < currentAbility.abilityDetails.range + slowDownBuffer)
            {
                agent.speed = 1f;
            }
            else
            {
                agent.speed = moveSpeed;
            }
            // Reset speed if not reset
        }
        else if (agent.speed != moveSpeed)
        {
            agent.speed = moveSpeed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float distanceFromPlayer = Vector2.Distance(player.transform.position, enemy.transform.position);
        if (distanceFromPlayer < attackRange && !isAttacking)
        {
            Attack();
        }
        else if (distanceFromPlayer < lineOfSight && !isAttacking)
        {
            agent.SetDestination(player.transform.position);

            if (!isMoving)
            {
                SetToMoving();
                enemy.movementEvents.RaiseMoveByPosition(moveSpeed);
                StartTracking();
            }
        }
        if (isMoving)
        {
            UpdateEnemyMovement();
        }

    }

    private void ResetStates()
    {
        isMoving = false;
        isAttacking = false;
        isIdle = false;
    }

    private void StartTracking()
    {
        lastPosition = enemy.transform.position;
    }

    private void SetToMoving()
    {
        ResetStates();
        isMoving = true;
    }

    private void SetToIdle()
    {
        ResetStates();
        isIdle = true;
    }

    private void UpdateEnemyMovement()
    {
        {
            if (!isMoving || enemy.agent.pathPending) return;

            Vector3 currentPosition = enemy.transform.position;
            Vector3 delta = currentPosition - lastPosition;

            if (currentPosition.x >= lastPosition.x && !posTargetRight)
            {
                enemy.movementEvents.RaiseFaceRight();
                posTargetRight = true;
                posTargetLeft = false;
            }
            else if (currentPosition.x < lastPosition.x && !posTargetLeft)
            {
                enemy.movementEvents.RaiseFaceLeft();
                posTargetRight = false;
                posTargetLeft = true;
            }

            lastPosition = currentPosition;

            // Optional: Stop animation early if somehow movement stops unexpectedly
            if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance && enemy.agent.velocity.sqrMagnitude < 0.01f)
            {
                StopTracking();
            }
        }
    }

    private void SetToAttacking()
    {
        ResetStates();
        isAttacking = true;
    }

    private void StopMovement()
    {
        agent.ResetPath();
        StopTracking();
    }

    private void Attack()
    {
        StopMovement();
        SetToAttacking();

        enemy.movementEvents.RaiseAttack();
        Vector3 playerPosition = player.target.position;

        Vector3 enemyDirection = (playerPosition - enemy.transform.position);
        float enemyAngle = HelperUtilities.GetAngleFromVector(enemyDirection);
        TargetDirection aimDirection = HelperUtilities.GetTargetDirection(enemyAngle);

        UpdateEnemyDirection(aimDirection);
        if (currentAbility.abilityDetails.isRanged)
        {
            RangedAttack(aimDirection, playerPosition, enemyAngle);
        }
        else
        {
            enemy.abilityEvents.RaiseMeleeAttackEvent();
        }
    }

    private void RangedAttack(TargetDirection aimDirection, Vector3 playerPosition, float enemyAngle)
    {
        Vector3 castPosition = enemy.activeAbility.GetCastPosition(aimDirection);
        // Ensure we're working in 2D by setting Z to 0 for both positions
        Vector3 castPosition2D = new Vector3(castPosition.x, castPosition.y, 0f);
        Vector3 worldPosition2D = new Vector3(playerPosition.x, playerPosition.y, 0f);

        Vector3 castPointDirection = (worldPosition2D - castPosition2D);
        float castPointAngle = HelperUtilities.GetAngleFromVector(castPointDirection);


        if (currentAbility.abilityDetails.isEnemyTargetable)
        {
            enemy.abilitySetupEvent.RaiseAbilitySetupEvent(true, aimDirection, enemyAngle, castPointAngle, castPointDirection, player);
        }
        else
        {
            enemy.abilitySetupEvent.RaiseAbilitySetupEvent(true, aimDirection, enemyAngle, castPointAngle, castPointDirection, null);
        }
        enemy.abilityEvents.RaiseCastAbilityEvent();

    }

    private void UpdateEnemyDirection(TargetDirection direction)
    {
        if (direction == TargetDirection.Left)
        {
            enemy.movementEvents.RaiseFaceLeft();
            posTargetRight = false;
            posTargetLeft = true;
        }
        else
        {
            enemy.movementEvents.RaiseFaceRight();
            posTargetRight = true;
            posTargetLeft = false;
        }
    }

    private void StopTracking()
    {
        HandleIdleState();
    }


    private void HandleIdleState()
    {
        SetToIdle();
        enemy.movementEvents.RaiseIdle();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, lineOfSight);
    }
}

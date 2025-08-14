using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(Player))]
public class PlayerControl : MonoBehaviour
{

    [SerializeField] private MovementDetailSO movementDetails;
    [SerializeField] private LayerMask collisionLayermask;

    private Player player;
    private int currentAbilityIndex = 1;
    private float moveSpeed;

    private Vector3 lastPosition;
    public bool isMoving = false;
    public bool isAttacking = false;
    public bool isIdle = false;
    public bool posTargetLeft = false;
    public bool posTargetRight = false;

    public TargetEnemy targetEnemy;
    public bool isAiming = false;
    public float maxMoveSelectDistance = 30f;

    private void Awake()
    {
        // Load components
        player = GetComponent<Player>();

        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Start()
    {
        SetPlayerAnimationspeed();
        SetPlayerStartAbility();
        player.abilityEvents.OnAbilityCasted += HandleIdleState;
        player.abilityEvents.OnMeleeEndAttack += HandleIdleState;
        GameEventManager.Instance.targetEvents.OnAimEnemy += HandleOnAimEnemy;
        GameEventManager.Instance.targetEvents.OnRemoveAim += HandleOnRemoveAim;
        GameEventManager.Instance.targetEvents.OnTargetEnemy += HandleOnTargetEnemy;
    }

    private void OnDisable()
    {
        player.abilityEvents.OnAbilityCasted -= HandleIdleState;
        player.abilityEvents.OnMeleeEndAttack -= HandleIdleState;
        GameEventManager.Instance.targetEvents.OnAimEnemy -= HandleOnAimEnemy;
        GameEventManager.Instance.targetEvents.OnRemoveAim -= HandleOnRemoveAim;
        GameEventManager.Instance.targetEvents.OnTargetEnemy -= HandleOnTargetEnemy;
    }

    private void HandleOnTargetEnemy(TargetEnemy newTargetEnemy)
    {
        // Check if an enemy is targeted and its not the same target
        if (targetEnemy && targetEnemy != newTargetEnemy)
        {
            targetEnemy.DeselectTarget();
        }
        targetEnemy = newTargetEnemy;
        AttackEnemy();
    }

    private void HandleOnRemoveAim()
    {
        Debug.Log("No aiming");
        isAiming = false;
    }

    private void HandleOnAimEnemy()
    {
        Debug.Log("Aiming");
        isAiming = true;
    }

    private void SetPlayerStartAbility()
    {
        int index = 1;
        foreach (Ability ability in player.abilityList)
        {
            if (ability.abilityDetails == player.characterDetails.startingAbility)
            {
                SetAbilityByIndex(index);
                break;
            }
            index++;
        }
    }

    private void SetAbilityByIndex(int abilityIndex)
    {
        if (abilityIndex - 1 < player.abilityList.Count)
        {
            currentAbilityIndex = abilityIndex;
            player.setActiveAbilityEvent.CallSetActiveAbilityEvent(player.abilityList[abilityIndex - 1]);
        }
    }

    private void SetPlayerAnimationspeed()
    {
        player.animator.speed = moveSpeed / Settings.baseSpeedForPlayerAnimations;
    }

    private void Update()
    {
        MouseInput();
        CastAbility();
        if (isMoving)
        {
            UpdatePlayerMovement();
        }
    }

    private void ResetStates()
    {
        isMoving = false;
        isAttacking = false;
        isIdle = false;
    }

    private void SetToIdle()
    {
        ResetStates();
        isIdle = true;
    }

    private void SetToMoving()
    {
        ResetStates();
        isMoving = true;
    }

    private void SetToAttacking()
    {
        ResetStates();
        isAttacking = true;
    }

    private void MouseInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (!isAiming) // Right click
            {
                if (targetEnemy)
                {
                    targetEnemy.DeselectTarget();
                }
                MoveToClickPosition();
            }
            // Clicked on an already selected enemy
            else
            {
                Ability currentAbility = player.activeAbility.GetCurrentAbility();
                if (currentAbility.abilityDetails.isEnemyTargetable && targetEnemy)
                {
                    AttackEnemy();
                }
            }
        }
        
    }

    private void MoveToClickPosition()
    {
        Vector3 worldPosition = GetClickPosition();

        int areaMask = NavMesh.AllAreas;
        // Project to navmesh if the click is off-mesh
        if (NavMesh.SamplePosition(worldPosition, out var hit, maxMoveSelectDistance, areaMask))
            worldPosition = hit.position;

        player.playerAgent.SetDestination(worldPosition);

        SetToMoving();
        GameEventManager.Instance.movementEvents.RaiseMoveByPosition(moveSpeed);
        StartTracking();
    }

    private void AttackEnemy()
    {
        StopMovement();
        SetToAttacking();

        GameEventManager.Instance.movementEvents.RaiseAttack();
        Vector3 enemyPosition = targetEnemy.target.position;


        Vector3 playerDirection = (enemyPosition - player.transform.position);
        float playerAngle = HelperUtilities.GetAngleFromVector(playerDirection);
        TargetDirection aimDirection = HelperUtilities.GetTargetDirection(playerAngle);

        Vector3 castPosition = player.activeAbility.GetCastPosition(aimDirection);
        // Ensure we're working in 2D by setting Z to 0 for both positions
        Vector3 castPosition2D = new Vector3(castPosition.x, castPosition.y, 0f);
        Vector3 worldPosition2D = new Vector3(enemyPosition.x, enemyPosition.y, 0f);

        Vector3 castPointDirection = (worldPosition2D - castPosition2D);
        float castPointAngle = HelperUtilities.GetAngleFromVector(castPointDirection);

        UpdatePlayerDirection(aimDirection);

        player.abilityEvents.RaiseAbilitySetupEvent(true, aimDirection, playerAngle, castPointAngle, castPointDirection);
        player.abilityEvents.RaiseCastAbilityEvent();
    }

    private void StopMovement()
    {
        player.playerAgent.ResetPath();                     
        StopTracking();                  
    }

    private Vector3 GetClickPosition()
    {
        Camera mainCamera = null;
        if (!mainCamera) mainCamera = Camera.main;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0f; // Ensure z = 0 for 2D
        return worldPosition;
    }

    private void CastAbility()
    {
        if (Input.GetMouseButton(0))
        {
            //if (player.activeAbility.isCooledDown()) {
            StopMovement();
            SetToAttacking();
            GameEventManager.Instance.movementEvents.RaiseAttack();
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

            player.abilityEvents.RaiseAbilitySetupEvent(true, aimDirection, playerAngle, castPointAngle, castPointDirection);
                player.abilityEvents.RaiseCastAbilityEvent();
            //}
        }
    }

    private void UpdatePlayerDirection(TargetDirection direction)
    {
        if (direction == TargetDirection.Left)
        {
            GameEventManager.Instance.movementEvents.RaiseFaceLeft();
            posTargetRight = false;
            posTargetLeft = true;
        } else
        {
            GameEventManager.Instance.movementEvents.RaiseFaceRight();
            posTargetRight = true;
            posTargetLeft = false;
        }
    }

    private void StartTracking()
    {
        lastPosition = transform.position;
    }

    private void StopTracking()
    {
        HandleIdleState();
    }

    private void HandleIdleState()
    {
        SetToIdle();
        GameEventManager.Instance.movementEvents.RaiseIdle();
    }

    private void UpdatePlayerMovement()
    {
        {
            if (!isMoving || player.playerAgent.pathPending) return;

            Vector3 currentPosition = transform.position;
            Vector3 delta = currentPosition - lastPosition;

            if (currentPosition.x >= lastPosition.x && !posTargetRight)
            {
                GameEventManager.Instance.movementEvents.RaiseFaceRight();
                posTargetRight = true;
                posTargetLeft = false;
            }
            else if (currentPosition.x < lastPosition.x && !posTargetLeft)
            {
                GameEventManager.Instance.movementEvents.RaiseFaceLeft();
                posTargetRight = false;
                posTargetLeft = true;
            }

            lastPosition = currentPosition;

            // Optional: Stop animation early if somehow movement stops unexpectedly
            if (player.playerAgent.remainingDistance <= player.playerAgent.stoppingDistance && player.playerAgent.velocity.sqrMagnitude < 0.01f)
            {
                StopTracking();
            }
        }
    }

}
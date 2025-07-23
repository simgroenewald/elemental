using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerControl : MonoBehaviour
{

    [SerializeField] private MovementDetailSO movementDetails;
    [SerializeField] private LayerMask collisionLayermask;


    private Player player;
    private float moveSpeed;
    private Stack<Vector3> path;
    private Vector3 targetPosition;
    private bool targetReached = false;

    private void Awake()
    {
        // Load components
        player = GetComponent<Player>();

        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Update()
    {
        MovementInput();
        MouseMovementInput();
        MoveOnPath();
    }

    private void MoveOnPath()
    {
        if (path == null) return;

        //Vector3 direction = (targetPosition - transform.position).normalized;
        //player.movementByVelocityEvent.CallMovementByVelocityEvent(direction, moveSpeed);
        //Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
        GameEventManager.Instance.movementEvents.RaiseMoveByPosition(targetPosition, transform.position, moveSpeed);

        if (Vector3.Distance(transform.position, targetPosition) < 0.15f)
        {
            transform.position = targetPosition;

            if (path.Count > 0)
            {
                targetPosition = path.Pop();
            }
            else
            {
                GameEventManager.Instance.movementEvents.RaiseIdle();
            }
        }
    }

    private void MovementInput()
    {
        // Get movement input
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");

        // Create a direction vector based on the input
        Vector2 direction = new Vector2(horizontalMovement, verticalMovement);

        // Adjust distance for diagonal movement (pythagoras approximation)
        if (horizontalMovement != 0f && verticalMovement != 0f)
        {
            direction *= 0.7f;
        }

        // If there is movement
        if (direction != Vector2.zero)
        {
            // trigger movement event
            GameEventManager.Instance.movementEvents.RaiseMoveByVelocity(direction, moveSpeed);
        }
        // else trigger idle event
        else
        {
            GameEventManager.Instance.movementEvents.RaiseIdle();
        }
    }

    private void MouseMovementInput()
    {
        if (Input.GetMouseButtonDown(1)) // Right click
        {
            Camera mainCamera = null;
            if (!mainCamera) mainCamera = Camera.main;
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0f; // Ensure z = 0 for 2D
            player.playerAgent.SetDestination(worldPosition);
            
/*            Vector3Int playerPosition = grid.WorldToCell(player.transform.position);

            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();

            Vector2 size = boxCollider.size;
            float radius = 1f;

            path = AStar.BuildPath(dungeon, dungeon.dungeonFloorPositions, dungeon.bounds, playerPosition, mousePosition, collisionLayermask, radius);
            targetPosition = path.Pop();*/
        }
    }
}
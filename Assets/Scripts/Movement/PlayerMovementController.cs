using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Queue<Vector3> currentPath;
    private Rigidbody2D rb;
    private bool isMoving;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentPath = new Queue<Vector3>();
    }

    private void FixedUpdate()
    {
        if (!isMoving || currentPath.Count == 0) return;

        Vector3 target = currentPath.Peek();
        Vector3 moveDir = (target - transform.position).normalized;
        Vector3 newPosition = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.fixedDeltaTime);

        rb.MovePosition(newPosition);

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            currentPath.Dequeue();
            if (currentPath.Count == 0)
                isMoving = false;
        }
    }

    public void SetPath(Stack<Vector3> path)
    {
        currentPath = new Queue<Vector3>(path);
        isMoving = true;
    }
}
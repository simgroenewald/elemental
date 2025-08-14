using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

[RequireComponent (typeof(SortingGroup))]
[RequireComponent (typeof(SpriteRenderer))]
[RequireComponent (typeof(Animator))]
[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(CircleCollider2D))]
[RequireComponent (typeof(PolygonCollider2D))]
[RequireComponent (typeof(AbilityEvents))]

[DisallowMultipleComponent]
public class Enemy : MonoBehaviour
{
    [HideInInspector] public EnemyDetailsSO enemyDetails;
    private CircleCollider2D circleCollider;
    private PolygonCollider2D polygonCollider;
    [HideInInspector] public SpriteRenderer[] spriteRendererArray;
    [HideInInspector] public Animator animator;
    [HideInInspector] public NavMeshAgent enemyAgent;
    [HideInInspector] public AbilityEvents abilityEvents;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        polygonCollider = GetComponent<PolygonCollider2D>();    
        spriteRendererArray = GetComponentsInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
        abilityEvents = GetComponent<AbilityEvents>();
        enemyAgent = GetComponent<NavMeshAgent>();
        enemyAgent.updateRotation = false;
        enemyAgent.updateUpAxis = false;
    }

}

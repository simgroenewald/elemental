using System.Collections.Generic;
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
[RequireComponent(typeof(EnemyMovementEvents))]
[RequireComponent(typeof(ActiveAbility))]
[RequireComponent(typeof(SetActiveAbilityEvent))]

[DisallowMultipleComponent]
public class Enemy : MonoBehaviour
{
    [SerializeField] public EnemyDetailsSO enemyDetails;
    private CircleCollider2D circleCollider;
    private PolygonCollider2D polygonCollider;
    [HideInInspector] public SpriteRenderer[] spriteRendererArray;
    [HideInInspector] public Animator animator;
    [HideInInspector] public NavMeshAgent enemyAgent;
    [HideInInspector] public AbilityEvents abilityEvents;
    [HideInInspector] public EnemyMovementEvents movementEvents;
    [HideInInspector] public ActiveAbility activeAbility;
    [HideInInspector] public SetActiveAbilityEvent setActiveAbilityEvent;

    public List<Ability> abilityList = new List<Ability>();

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        polygonCollider = GetComponent<PolygonCollider2D>();    
        spriteRendererArray = GetComponentsInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
        abilityEvents = GetComponent<AbilityEvents>();
        movementEvents = GetComponent<EnemyMovementEvents>();
        enemyAgent = GetComponent<NavMeshAgent>();
        enemyAgent.updateRotation = false;
        enemyAgent.updateUpAxis = false;
        activeAbility = GetComponent<ActiveAbility>();
        setActiveAbilityEvent = GetComponent<SetActiveAbilityEvent>();
    }

    private void Start()
    {
        CreateEnemyStartingAbilities();
    }

    private void CreateEnemyStartingAbilities()
    {
        abilityList.Clear();

        foreach (AbilityDetailsSO abilityDetails in enemyDetails.abilityList)
        {
            AddAbilityToEnemy(abilityDetails);
        }
    }

    private Ability AddAbilityToEnemy(AbilityDetailsSO abilityDetails)
    {
        Ability ability = new Ability() { abilityDetails = abilityDetails, abilityCooldownTime = 0f, isCoolingDown = false };
        abilityList.Add(ability);

        ability.abilityListPosition = abilityList.Count;

        setActiveAbilityEvent.CallSetActiveAbilityEvent(ability);

        return ability;
    }

}

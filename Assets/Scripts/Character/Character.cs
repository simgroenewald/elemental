using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(HealthEvents))]
[RequireComponent(typeof(MovementHandler))]
[RequireComponent(typeof(SetActiveAbilityEvent))]
[RequireComponent(typeof(ActiveAbility))]
[RequireComponent(typeof(CastAbility))]
[RequireComponent(typeof(MeleeAbility))]
[RequireComponent(typeof(AbilityEvents))]
[RequireComponent(typeof(MovementEvents))]
[RequireComponent(typeof(AbilitySetupEvent))]
[RequireComponent(typeof(AnimateCharacter))]
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CharacterCombat))]
[RequireComponent(typeof(CharacterState))]
[RequireComponent(typeof(CharacterMovement))]

[DisallowMultipleComponent]
public class Character : MonoBehaviour, ITargetable
{
    [HideInInspector] public CharacterDetailSO characterDetails;
    [HideInInspector] public Health health;
    [HideInInspector] public HealthEvents healthEvents;
    [HideInInspector] public SetActiveAbilityEvent setActiveAbilityEvent;
    [HideInInspector] public ActiveAbility activeAbility;
    [HideInInspector] public CastAbility castAbility;
    [HideInInspector] public MeleeAbility meleeAbility;
    [HideInInspector] public AbilityEvents abilityEvents;
    [HideInInspector] public MovementEvents movementEvents;
    [HideInInspector] public AbilitySetupEvent abilitySetupEvent;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public AnimateCharacter animateCharacter;
    [HideInInspector] public Animator animator;
    [HideInInspector] public CharacterCombat characterCombat;
    [HideInInspector] public CharacterState characterState;
    [HideInInspector] public CharacterMovement characterMovement;
    [HideInInspector] public NavMeshAgent agent;
    [SerializeField] public Transform target;

    public List<Ability> abilityList = new List<Ability>();

    protected virtual void Awake()
    {
        // Load components
        health = GetComponent<Health>();
        healthEvents = GetComponent<HealthEvents>();
        setActiveAbilityEvent = GetComponent<SetActiveAbilityEvent>();
        activeAbility = GetComponent<ActiveAbility>();
        castAbility = GetComponent<CastAbility>();
        meleeAbility = GetComponent<MeleeAbility>();
        abilityEvents = GetComponent<AbilityEvents>();
        movementEvents = GetComponent<MovementEvents>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        abilitySetupEvent = GetComponent<AbilitySetupEvent>();
        characterCombat = GetComponent<CharacterCombat>();
        characterState = GetComponent<CharacterState>();
        characterMovement = GetComponent<CharacterMovement>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    protected virtual void Initialise(CharacterDetailSO characterDetails)
    {
        this.characterDetails = characterDetails;
        CreateCharacterStartingAbilities();
        SetCharacterHealth();
    }

    private void CreateCharacterStartingAbilities()
    {
        abilityList.Clear();

        foreach (AbilityDetailsSO abilityDetails in characterDetails.abilityList)
        {
            AddAbilityToCharacter(abilityDetails);
        }
    }

    private Ability AddAbilityToCharacter(AbilityDetailsSO abilityDetails)
    {
        Ability ability = new Ability() { abilityDetails = abilityDetails, abilityCooldownTime = 0f, isCoolingDown = false };
        abilityList.Add(ability);

        ability.abilityListPosition = abilityList.Count;

        setActiveAbilityEvent.CallSetActiveAbilityEvent(ability);

        return ability;
    }

    private void SetCharacterHealth()
    {
        health.SetHealth(characterDetails.health);
    }

    public void SetCharacterPosition(Vector2Int position, Grid grid)
    {
        this.gameObject.transform.position = grid.CellToWorld((Vector3Int)position);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public Transform GetTargetTransform()
    {
        return target;
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(HealthEvents))]
[RequireComponent(typeof(Mana))]
[RequireComponent(typeof(ManaEvents))]
[RequireComponent(typeof(MovementHandler))]
[RequireComponent(typeof(AbilityActivationEvents))]
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
[RequireComponent(typeof(StatModifierEvents))]
[RequireComponent(typeof(Stats))]
[RequireComponent(typeof(StatModifierEvents))]

[DisallowMultipleComponent]
public class Character : MonoBehaviour, ITargetable
{
    [HideInInspector] public CharacterDetailSO characterDetails;
    [HideInInspector] public Health health;
    [HideInInspector] public HealthEvents healthEvents;
    [HideInInspector] public Mana mana;
    [HideInInspector] public ManaEvents manaEvents;
    [HideInInspector] public AbilityActivationEvents abilityActivationEvents;
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
    [HideInInspector] public StatModifierEvents statModifierEvents;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Ability baseAbility;
    [HideInInspector] public Stats stats;
    [SerializeField] public Transform target;

    public List<Ability> abilityList = new List<Ability>();

    protected virtual void Awake()
    {
        // Load components
        health = GetComponent<Health>();
        healthEvents = GetComponent<HealthEvents>();
        mana = GetComponent<Mana>();
        manaEvents = GetComponent<ManaEvents>();
        abilityActivationEvents = GetComponent<AbilityActivationEvents>();
        activeAbility = GetComponent<ActiveAbility>();
        castAbility = GetComponent<CastAbility>();
        meleeAbility = GetComponent<MeleeAbility>();
        abilityEvents = GetComponent<AbilityEvents>();
        movementEvents = GetComponent<MovementEvents>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        animateCharacter = GetComponent<AnimateCharacter>();
        agent = GetComponent<NavMeshAgent>();
        abilitySetupEvent = GetComponent<AbilitySetupEvent>();
        characterCombat = GetComponent<CharacterCombat>();
        characterState = GetComponent<CharacterState>();
        characterMovement = GetComponent<CharacterMovement>();
        statModifierEvents = GetComponent<StatModifierEvents>();
        stats = GetComponent<Stats>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    protected virtual void Initialise(CharacterDetailSO characterDetails)
    {
        this.characterDetails = characterDetails;
        stats.Initialise(characterDetails.statsSO);
        CreateCharacterStartingAbilities();

        CreateCharacterBaseAbility();
        SetCharacterHealth();
        SetCharacterMana();
    }

    private void CreateCharacterBaseAbility()
    {
        baseAbility = new Ability() { abilityDetails = characterDetails.baseAbility, abilityCooldownTime = 0f, isCoolingDown = false };
        abilityActivationEvents.CallSetActiveAbilityEvent(baseAbility);
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

        //setActiveAbilityEvent.CallSetActiveAbilityEvent(ability);

        return ability;
    }

    private void SetCharacterHealth()
    {
        health.SetHealth();
    }

    private void SetCharacterMana()
    {
        mana.SetMana();
    }

    public void SetCharacterPosition(Vector2Int position, Grid grid)
    {
        Vector3 newPos = grid.CellToWorld((Vector3Int)position);
        this.gameObject.transform.position = newPos;
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

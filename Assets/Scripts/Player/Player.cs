using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(PlayerControl))]
[RequireComponent(typeof(MovementHandler))]
[RequireComponent(typeof(SetActiveAbilityEvent))]
[RequireComponent(typeof(ActiveAbility))]
[RequireComponent(typeof(CastAbility))]
[RequireComponent(typeof(MeleeAbility))]
[RequireComponent(typeof(AbilityEvents))]
[RequireComponent(typeof(AnimatePlayer))]
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]

[DisallowMultipleComponent]
public class Player : MonoBehaviour
{
    [HideInInspector] public CharacterDetailSO characterDetails;
    [HideInInspector] public Health health;
    [HideInInspector] public SetActiveAbilityEvent setActiveAbilityEvent;
    [HideInInspector] public ActiveAbility activeAbility;
    [HideInInspector] public CastAbility castAbility;
    [HideInInspector] public MeleeAbility meleeAbility;
    [HideInInspector] public AbilityEvents abilityEvents;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Animator animator;
    [HideInInspector] public NavMeshAgent playerAgent;
    [SerializeField] public Transform target;

    public List<Ability> abilityList = new List<Ability>();

    private void Awake()
    {
        // Load components
        health = GetComponent<Health>();
        setActiveAbilityEvent = GetComponent<SetActiveAbilityEvent>();
        activeAbility = GetComponent<ActiveAbility>();
        castAbility = GetComponent<CastAbility>();
        meleeAbility = GetComponent<MeleeAbility>();
        abilityEvents = GetComponent<AbilityEvents>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        playerAgent = GetComponent<NavMeshAgent>();
        playerAgent.updateRotation = false;
        playerAgent.updateUpAxis = false;
    }

    public void Initialise(CharacterDetailSO characterDetails)
    {
        this.characterDetails = characterDetails;
        CreatePlayerStartingAbilities();
        SetPlayerHealth();
    }

    private void CreatePlayerStartingAbilities()
    {
        abilityList.Clear();

        foreach (AbilityDetailsSO abilityDetails in characterDetails.abilityList)
        {
            AddAbilityToPlayer(abilityDetails);
        }
    }

    private Ability AddAbilityToPlayer(AbilityDetailsSO abilityDetails)
    {
        Ability ability = new Ability() { abilityDetails = abilityDetails, abilityCooldownTime = 0f, isCoolingDown = false };
        abilityList.Add(ability);

        ability.abilityListPosition = abilityList.Count;

        setActiveAbilityEvent.CallSetActiveAbilityEvent(ability);

        return ability;
    }

    private void SetPlayerHealth()
    {
        health.SetMaxHealth(characterDetails.health);
    }

    public void SetPlayerStartPosition(DungeonRoom room, Grid grid)
    {
        Vector2Int randomPos = room.structure.floorPositions.ElementAt(UnityEngine.Random.Range(0, room.structure.floorPositions.Count));
        SetPlayerPosition(randomPos, grid);
    }

    public void SetPlayerPosition(Vector2Int position, Grid grid)
    {
        this.gameObject.transform.position = grid.CellToWorld((Vector3Int)position);
    }

}

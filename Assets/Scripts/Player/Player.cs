using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(PlayerControl))]
[RequireComponent(typeof(MovementHandler))]
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
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Animator animator;

    private void Awake()
    {
        // Load components
        health = GetComponent<Health>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void Initialise(CharacterDetailSO characterDetails)
    {
        this.characterDetails = characterDetails;
        SetPlayerHealth();
    }

    private void SetPlayerHealth()
    {
        health.SetMaxHealth(characterDetails.health);
    }

    public void SetPlayerStartPosition(DungeonRoom room, Grid grid)
    {
        Vector2Int randomPos = room.floorPositions.ElementAt(UnityEngine.Random.Range(0, room.floorPositions.Count));
        SetPlayerPosition(randomPos, grid);
    }

    public void SetPlayerPosition(Vector2Int position, Grid grid)
    {
        this.gameObject.transform.position = grid.CellToWorld((Vector3Int)position);
    }

}

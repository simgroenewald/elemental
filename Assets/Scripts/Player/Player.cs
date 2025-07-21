using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

[DisallowMultipleComponent]

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class Player : MonoBehaviour
{
    public CharacterDetailSO characterDetail;
    public Health health;
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    private void Awake()
    {
        health = GetComponent<Health>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void Initialise(CharacterDetailSO characterDetail)
    {
        this.characterDetail = characterDetail;

        SetPlayerHealth();
    }

    private void SetPlayerHealth()
    {
        health.SetMaxHealth(characterDetail.health);
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

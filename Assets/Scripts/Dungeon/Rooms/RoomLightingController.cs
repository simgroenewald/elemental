using System;
using System.Collections;
using System.Net;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
public class RoomLightingController : MonoBehaviour
{
    private bool isLit = false;
    private DungeonRoom dungeonRoom;

    private void Awake()
    {
        dungeonRoom = GetComponent<DungeonRoom>();
    }

    private void OnEnable()
    {
        StaticEventHandler.OnRoomFadeIn += OnRoomFadeIn;
        StaticEventHandler.OnRoomFadeOut += OnRoomFadeOut;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomFadeIn -= OnRoomFadeIn;
        StaticEventHandler.OnRoomFadeOut -= OnRoomFadeOut;
    }

    public void OnRoomFadeIn(RoomDisplayEventArgs roomDisplayEventArgs)
    {
        if (roomDisplayEventArgs.room == dungeonRoom && !isLit)
        {
            if (GameManager.Instance.state != GameState.bossRoom && dungeonRoom.roomType == RoomType.Boss)
                return;
            StartCoroutine(FadeInRoom(dungeonRoom));
            FadeInRoomEnemies(dungeonRoom);
            isLit = true;
        }
    }

    public void OnRoomFadeOut(RoomDisplayEventArgs roomDisplayEventArgs)
    {
        if (roomDisplayEventArgs.room == dungeonRoom && isLit)
        {
            StartCoroutine(FadeOutRoom(dungeonRoom));
            FadeOutRoomEnemies(dungeonRoom);
            isLit = false;
        }
    }

    private IEnumerator FadeInRoom(DungeonRoom dungeonRoom)
    {
        Material varLitMaterial = new Material(GameResources.Instance.variableLitShader);
        Material litMaterial = new Material(GameResources.Instance.litMaterial);

        dungeonRoom.structure.tilemapLayers.baseTilemap.GetComponent<TilemapRenderer>().material = varLitMaterial;
        dungeonRoom.structure.tilemapLayers.baseDecorationTilemap.GetComponent<TilemapRenderer>().material = varLitMaterial;
        dungeonRoom.structure.tilemapLayers.frontTilemap.GetComponent<TilemapRenderer>().material = varLitMaterial;
        dungeonRoom.structure.tilemapLayers.frontDecorationTilemap.GetComponent<TilemapRenderer>().material = varLitMaterial;

        for (float i = 0.05f; i <= 1f; i+= Time.deltaTime/Settings.fadeInTime)
        {
            varLitMaterial.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        dungeonRoom.structure.tilemapLayers.baseTilemap.GetComponent<TilemapRenderer>().material = litMaterial;
        dungeonRoom.structure.tilemapLayers.baseDecorationTilemap.GetComponent<TilemapRenderer>().material = litMaterial;
        dungeonRoom.structure.tilemapLayers.frontTilemap.GetComponent<TilemapRenderer>().material = litMaterial;
        dungeonRoom.structure.tilemapLayers.frontDecorationTilemap.GetComponent<TilemapRenderer>().material = litMaterial;
    }

    private void FadeInRoomEnemies(DungeonRoom dungeonRoom)
    {
        Material varLitMaterial = new Material(GameResources.Instance.variableLitShader);
        Material litMaterial = new Material(GameResources.Instance.litMaterial);

        foreach (var enemy in dungeonRoom.enemies)
        {
            if (enemy)
            {
                StartCoroutine(FadeInEnemy(enemy, litMaterial, varLitMaterial));
            }
        }
    }

    private IEnumerator FadeInEnemy(Enemy enemy, Material litMaterial, Material varLitMaterial)
    {
        enemy.GetComponent<SpriteRenderer>().material = varLitMaterial;
        enemy.healthBarBackground.material = varLitMaterial;
        enemy.healthBarFill.material = varLitMaterial;
        for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
        {
            varLitMaterial.SetFloat("Alpha_Slider", i);
            yield return null;
        }
        enemy.GetComponent<SpriteRenderer>().material = litMaterial;
        enemy.healthBarBackground.material = null;
        enemy.healthBarFill.material = null;
    }

    private IEnumerator FadeOutRoom(DungeonRoom dungeonRoom)
    {
        Material varLitMaterial = new Material(GameResources.Instance.variableLitShader);
        Material dimmedMaterial = new Material(GameResources.Instance.dimmedMaterial);

        dungeonRoom.structure.tilemapLayers.baseTilemap.GetComponent<TilemapRenderer>().material = varLitMaterial;
        dungeonRoom.structure.tilemapLayers.baseDecorationTilemap.GetComponent<TilemapRenderer>().material = varLitMaterial;
        dungeonRoom.structure.tilemapLayers.frontTilemap.GetComponent<TilemapRenderer>().material = varLitMaterial;
        dungeonRoom.structure.tilemapLayers.frontDecorationTilemap.GetComponent<TilemapRenderer>().material = varLitMaterial;

        for (float i = 1f; i >= 0.05f; i -= Time.deltaTime / Settings.fadeInTime)
        {
            varLitMaterial.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        dungeonRoom.structure.tilemapLayers.baseTilemap.GetComponent<TilemapRenderer>().material = dimmedMaterial;
        dungeonRoom.structure.tilemapLayers.baseDecorationTilemap.GetComponent<TilemapRenderer>().material = dimmedMaterial;
        dungeonRoom.structure.tilemapLayers.frontTilemap.GetComponent<TilemapRenderer>().material = dimmedMaterial;
        dungeonRoom.structure.tilemapLayers.frontDecorationTilemap.GetComponent<TilemapRenderer>().material = dimmedMaterial;
    }

    private void FadeOutRoomEnemies(DungeonRoom dungeonRoom)
    {
        Material varLitMaterial = new Material(GameResources.Instance.variableLitShader);
        Material dimmedMaterial = new Material(GameResources.Instance.dimmedMaterial);

        foreach (var enemy in dungeonRoom.enemies)
        { if (enemy)
            {
                StartCoroutine(FadeOutEnemy(enemy, dimmedMaterial, varLitMaterial));
            }
        }
    }

    private IEnumerator FadeOutEnemy(Enemy enemy, Material dimmedMaterial, Material varLitMaterial)
    {
        enemy.GetComponent<SpriteRenderer>().material = varLitMaterial;
        enemy.healthBarBackground.material = varLitMaterial;
        enemy.healthBarFill.material = varLitMaterial;
        for (float i = 1f; i >= 0.05f; i -= Time.deltaTime / Settings.fadeInTime)
        {
            varLitMaterial.SetFloat("Alpha_Slider", i);
            yield return null;
        }
        enemy.GetComponent<SpriteRenderer>().material = dimmedMaterial;
        enemy.healthBarBackground.material = dimmedMaterial;
        enemy.healthBarFill.material = dimmedMaterial;
    }
}

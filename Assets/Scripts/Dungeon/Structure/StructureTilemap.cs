using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StructureTilemap : MonoBehaviour
{
    [SerializeField] public GameObject structureTemplate;
    public TilemapLayers tilemapLayers = new TilemapLayers();

    // Call this during dungeon generation
    public void InitializeStructureTemplate(Transform parent = null)
    {

        // Assign the structure template from a central source (replace with yours)
        structureTemplate = GameResources.Instance.structureTemplatePrefab;

        if (structureTemplate == null)
        {
            Debug.LogError("Structure template not assigned.");
            return;
        }

        GameObject structureTemplateInstance = Instantiate(structureTemplate, parent);
        
        HideCollisionLayer();

        if (tilemapLayers == null)
        {
            Debug.LogError("TilemapLayers component missing on structureTemplate prefab.");
        }
    }

    public void HideCollisionLayer()
    {
        tilemapLayers.collisionTilemap.GetComponent<TilemapRenderer>().enabled = false;
    }
}
using System;
using System.Collections;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
public class ConnectorLightingController : MonoBehaviour
{
    private bool isLit = false;
    private Connector connector;

    private void Awake()
    {
        connector = GetComponent<Connector>();
    }

    private void OnEnable()
    {
        StaticEventHandler.OnConnectorFadeIn += OnConnectorFadeIn;
        StaticEventHandler.OnConnectorFadeOut += OnConnectorFadeOut;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnConnectorFadeIn -= OnConnectorFadeIn;
        StaticEventHandler.OnConnectorFadeOut -= OnConnectorFadeOut;
    }

    public void OnConnectorFadeIn(ConnectorDisplayEventArgs connectorDisplayEventArgs)
    {
        if (connectorDisplayEventArgs.connector == connector && !isLit)
        {
            StartCoroutine(FadeInConnector(connector));

            isLit = true;
        }
    }

    public void OnConnectorFadeOut(ConnectorDisplayEventArgs connectorDisplayEventArgs)
    {
        if (connectorDisplayEventArgs.connector == connector && isLit)
        {
            StartCoroutine(FadeOutConnector(connector));

            isLit = false;
        }
    }

    private IEnumerator FadeInConnector(Connector connector)
    {
        Material varLitMaterial = new Material(GameResources.Instance.variableLitShader);
        Material litMaterial = new Material(GameResources.Instance.litMaterial);

        connector.structure.tilemapLayers.platformTilemap.GetComponent<TilemapRenderer>().material = varLitMaterial;
        connector.structure.tilemapLayers.platformDecorationTilemap.GetComponent<TilemapRenderer>().material = varLitMaterial;
        connector.structure.tilemapLayers.frontTilemap.GetComponent<TilemapRenderer>().material = varLitMaterial;
        connector.structure.tilemapLayers.bridgeTilemap.GetComponent<TilemapRenderer>().material = varLitMaterial;

        for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
        {
            varLitMaterial.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        connector.structure.tilemapLayers.platformTilemap.GetComponent<TilemapRenderer>().material = litMaterial;
        connector.structure.tilemapLayers.platformDecorationTilemap.GetComponent<TilemapRenderer>().material = litMaterial;
        connector.structure.tilemapLayers.frontTilemap.GetComponent<TilemapRenderer>().material = litMaterial;
        connector.structure.tilemapLayers.bridgeTilemap.GetComponent<TilemapRenderer>().material = litMaterial;
    }

    private IEnumerator FadeOutConnector(Connector connector)
    {
        Material varLitMaterial = new Material(GameResources.Instance.variableLitShader);
        Material dimmedMaterial = new Material(GameResources.Instance.dimmedMaterial);

        connector.structure.tilemapLayers.platformTilemap.GetComponent<TilemapRenderer>().material = varLitMaterial;
        connector.structure.tilemapLayers.platformDecorationTilemap.GetComponent<TilemapRenderer>().material = varLitMaterial;
        connector.structure.tilemapLayers.frontTilemap.GetComponent<TilemapRenderer>().material = varLitMaterial;
        connector.structure.tilemapLayers.bridgeTilemap.GetComponent<TilemapRenderer>().material = varLitMaterial;

        for (float i = 1f; i >= 0.05f; i -= Time.deltaTime / Settings.fadeInTime)
        {
            varLitMaterial.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        connector.structure.tilemapLayers.platformTilemap.GetComponent<TilemapRenderer>().material = dimmedMaterial;
        connector.structure.tilemapLayers.platformDecorationTilemap.GetComponent<TilemapRenderer>().material = dimmedMaterial;
        connector.structure.tilemapLayers.frontTilemap.GetComponent<TilemapRenderer>().material = dimmedMaterial;
        connector.structure.tilemapLayers.bridgeTilemap.GetComponent<TilemapRenderer>().material = dimmedMaterial;
    }
}

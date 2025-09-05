using UnityEngine;

public class ConnectorFactory : MonoBehaviour
{
    public GameObject connectorPrefab;
    public GameObject bridgePrefab;
    public GameObject plaftormPrefab;

    public Connector CreateConnector(Vector2Int start, Vector2Int end, DungeonRoom parentRoom, DungeonRoom childRoom, ConnectorOrientation orientation, Transform objectParent, bool isBossRoomConnector)
    {
        GameObject connectorObject = Instantiate(connectorPrefab, objectParent);
        Connector connector = connectorObject.GetComponent<Connector>();
        connector.Initialise(start, end, parentRoom, childRoom, orientation, isBossRoomConnector);
        return connector;
    }

    public Bridge CreateBridge(ConnectorOrientation orientation, Transform objectParent)
    {
        GameObject bridgeObject = Instantiate(bridgePrefab, objectParent);
        Bridge bridge = bridgeObject.GetComponent<Bridge>();
        bridge.Initialise(orientation); // Or other setup logic
        return bridge;
    }

    public Platform CreatePlatform(ConnectorOrientation orientation, Transform objectParent)
    {
        GameObject platformObject = Instantiate(plaftormPrefab, objectParent);
        Platform platform = platformObject.GetComponent<Platform>();
        platform.Initialise(orientation); // Or other setup logic
        return platform;
    }
}

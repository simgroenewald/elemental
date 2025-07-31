
public static class TileTypeMapper
{
    public static TileType GetTileType(string tileName)
    {
        if (string.IsNullOrEmpty(tileName))
            return TileType.None;

        tileName = tileName.ToLowerInvariant();

        if (tileName.Contains("floor")) return TileType.Floor;
        if (tileName.Contains("wallback")) return TileType.WallBack;
        if (tileName.Contains("wallfront")) return TileType.WallFront;
        if (tileName.Contains("wallleft")) return TileType.WallLeft;
        if (tileName.Contains("wallright")) return TileType.WallRight;
        if (tileName.Contains("topleftconvex")) return TileType.TopLeftConvex;
        if (tileName.Contains("topleftconcave")) return TileType.TopLeftConcave;
        if (tileName.Contains("toprightconvex")) return TileType.TopRightConvex;
        if (tileName.Contains("toprightconcave")) return TileType.TopRightConcave;
        if (tileName.Contains("bottomleftconvex")) return TileType.BottomLeftConvex;
        if (tileName.Contains("bottomleftconcave")) return TileType.BottomLeftConcave;
        if (tileName.Contains("bottomrightconvex")) return TileType.BottomRightConvex;
        if (tileName.Contains("bottomrightconcave")) return TileType.BottomRightConcave;
        if (tileName.Contains("objectsmall")) return TileType.ObjectSmall;
        if (tileName.Contains("objectmedium")) return TileType.ObjectMedium;
        if (tileName.Contains("objectlarge")) return TileType.ObjectLarge;
        if (tileName.Contains("connectorStart")) return TileType.ConnectorStart;
        if (tileName.Contains("connectorEnd")) return TileType.ConnectorEnd;
        if (tileName.Contains("platform")) return TileType.Platform;
        if (tileName.Contains("dooredge")) return TileType.DoorEdge;
        if (tileName.Contains("door")) return TileType.Door;
        if (tileName.Contains("liquid")) return TileType.Liquid;
        if (tileName.Contains("pit")) return TileType.Pit;

        return TileType.None;
    }
}

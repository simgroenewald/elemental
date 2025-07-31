
public enum TileType
{
    None,
    Floor,
    // Walls
    WallBack,
    WallFront,
    WallLeft,
    WallRight,
    //Corners
    TopLeftConvex,
    TopLeftConcave,
    TopRightConvex,
    TopRightConcave,
    BottomLeftConvex,
    BottomLeftConcave,
    BottomRightConvex,
    BottomRightConcave,
    // Objects
    ObjectSmall,
    ObjectMedium,
    ObjectLarge,
    // Corridor
    MainConnector,
    ConnectorStart,
    ConnectorEnd,
    Platform,
    // Other
    Door,
    DoorEdge,
    Liquid,
    Pit
}

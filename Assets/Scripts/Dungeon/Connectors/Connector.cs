using UnityEngine;
using System;


    public class Connector
    {
        public DungeonRoom roomA;
        public DungeonRoom roomB;
        public Vector2Int start;
        public Vector2Int end;
        public ConnectorOrientation orientation;
        public Boolean isStraight;

        public Bridge bridgeMain;
        public Bridge bridgeStart;
        public Bridge bridgeEnd;
        public Platform platform;

        public Connector(Vector2Int _start, Vector2Int _end, ConnectorOrientation _orientation)
        {
            start = _start;
            end = _end;
            orientation = _orientation;

            if (orientation == ConnectorOrientation.Vertical)
            {
                if (start.x == end.x)
                {
                    isStraight = true;
                }
                else
                {
                    isStraight = false;
                }
            }
            if (orientation == ConnectorOrientation.Horizontal)
            {
                if (start.y == end.y)
                {
                    isStraight = true;
                }
                else
                {
                    isStraight = false;
                }
            }

            if (isStraight)
            {
                if (orientation == ConnectorOrientation.Vertical)
                {
                    bridgeMain = new Bridge(ConnectorOrientation.Vertical);
                }
                else
                {
                    bridgeMain = new Bridge(ConnectorOrientation.Horizontal);

                }
            }
            else
            {
                if (orientation == ConnectorOrientation.Vertical)
                {
                    bridgeStart = new Bridge(ConnectorOrientation.Vertical);
                    bridgeEnd = new Bridge(ConnectorOrientation.Vertical);
                    platform = new Platform(ConnectorOrientation.Horizontal);
                }
                else
                {
                    bridgeStart = new Bridge(ConnectorOrientation.Horizontal);
                    bridgeEnd = new Bridge(ConnectorOrientation.Horizontal);
                    platform = new Platform(ConnectorOrientation.Vertical);
                }
            }
        }

        public void GenerateAllPartStructureTiles()
        {
            if (isStraight)
            {
                bridgeMain.GenerateStructureTiles(bridgeMain.bounds, bridgeMain.floorPositions, false);
            }
            else
            {
                bridgeStart.GenerateStructureTiles(bridgeStart.bounds, bridgeStart.floorPositions, false);
                bridgeEnd.GenerateStructureTiles(bridgeEnd.bounds, bridgeEnd.floorPositions, false);
                platform.GenerateStructureTiles(platform.bounds, platform.floorPositions, true, 2);
            }
        }
    }
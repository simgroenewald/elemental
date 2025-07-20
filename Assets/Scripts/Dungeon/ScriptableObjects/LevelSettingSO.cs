using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelSettingSO", menuName = "Scriptable Objects/LevelSetting")]

    public class LevelSettingSO : ScriptableObject
    {
        public int level;
        public int roomCount;
        public List<RoomSizeCount> roomSizeCounts;
        public Dictionary<RoomSizeSO, int> roomSizeCountDict;

        public Dictionary<RoomSizeSO, int> GetRoomSizeCountDict()
        {
            var dict = new Dictionary<RoomSizeSO, int>();
            foreach (var entry in roomSizeCounts)
            {
                dict[entry.size] = entry.count;
            }
            return dict;
        }
    }
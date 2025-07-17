using UnityEngine;

[CreateAssetMenu(fileName = "RoomSizeSO", menuName = "Scriptable Objects/RoomSize")]
    public class RoomSizeSO : ScriptableObject
    {
        public string size;
        public int width;
        public int height;
        public int subRoomMinWidth;
        public int subRoomMinHeight;
    }

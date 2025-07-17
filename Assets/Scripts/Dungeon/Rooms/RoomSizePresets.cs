using UnityEngine;

[CreateAssetMenu(fileName = "RoomSizePresetsSO", menuName = "Scriptable Objects/RoomSizePresets")]

    public class RoomSizePresetsSO : ScriptableObject
    {
        public RoomSizeSO Small;
        public RoomSizeSO Medium;
        public RoomSizeSO Large;
    }

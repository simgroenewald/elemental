using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StructureTypeToGridMapperSO", menuName = "Scriptable Objects/StructureTypeToGridMapper")]

    public class StructureTypeToGridMapperSO : ScriptableObject
    {
        public List<StructureTypeToGridSO> structureTypeToGridList;
        public Dictionary<StructureType, Grid> structureTypeToGridDict;

        public Dictionary<StructureType, Grid> GetStructureTypeToGridDict()
        {
            var dict = new Dictionary<StructureType, Grid>();
            foreach (var entry in structureTypeToGridList)
            {
                dict[entry.structureType] = entry.grid;
            }
            return dict;
        }
    }

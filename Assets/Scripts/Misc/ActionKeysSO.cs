using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionKeys", menuName = "Scriptable Objects/ActionKeys")]
public class ActionKeysSO : ScriptableObject
{
    [field: SerializeField]
    public List<ActionKeyBinding> actionKeyBindings { get; set; }
}

[Serializable]

public struct ActionKeyBinding
{
    public KeyCode key;
    public int index;
}

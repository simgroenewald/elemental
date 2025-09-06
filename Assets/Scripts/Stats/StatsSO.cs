using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "StatsSO", menuName = "Scriptable Objects/StatsSO")]
public class StatsSO : ScriptableObject
{
    [SerializeField] public List<Stat> stats;
}

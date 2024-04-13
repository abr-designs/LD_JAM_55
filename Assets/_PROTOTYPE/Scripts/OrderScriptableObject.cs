using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default Order Profile", menuName = "ScriptableObjects/Order Profile", order = 1)]
public class OrderScriptableObject : ScriptableObject
{
    public bool[] allowedColors = new bool[GamePrototype.COLOR_COUNT];
}

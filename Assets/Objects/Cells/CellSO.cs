using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cells;

[CreateAssetMenu(fileName = "Cell", menuName = "Cell")]
public class CellSO : ScriptableObject
{
    public CellType type;
    public CellType inverseType;

    public bool walkable;
    public CellVisuals prefab;
}


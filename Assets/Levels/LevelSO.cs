using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using Array2DEditor;
using Cells;

[CreateAssetMenu(fileName = "Level", menuName = "Level")]
public class LevelSO : ScriptableObject
{
    public string id;
    public Vector2Int size;
    
    public Array2DCellType layout;

}

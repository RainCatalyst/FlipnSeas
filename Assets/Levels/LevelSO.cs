using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using Array2DEditor;
using Cells;

[CreateAssetMenu(fileName = "Level", menuName = "Level")]
public class LevelSO : ScriptableObject
{
    public string id;
    public int flips;
    public Vector2Int size;
    public Vector2Int entrance;
    public Vector2Int exit;
    
    public Array2DCellType layout;

}

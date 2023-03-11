using UnityEngine;
using Array2DEditor;

[CreateAssetMenu(fileName = "Level", menuName = "Level")]
public class LevelSO : ScriptableObject
{
    public string Id;
    public int Flips;
    public Vector2Int Size;
    public Vector2Int Entrance;
    public Vector2Int Exit;
    public Array2DCellType Layout;
}
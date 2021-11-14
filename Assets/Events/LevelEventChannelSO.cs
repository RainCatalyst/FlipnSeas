using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "LevelEventChannel", menuName = "Events/Level Event Channel")]
public class LevelEventChannelSO : ScriptableObject
{
    public UnityAction<GridCell> OnCellHovered;
    public UnityAction<GridCell> OnCellUnhovered;

    public void HoverCell(GridCell cell) {
        OnCellHovered?.Invoke(cell);
    }

    public void UnhoverCell(GridCell cell)
    {
        OnCellUnhovered?.Invoke(cell);
    }
}
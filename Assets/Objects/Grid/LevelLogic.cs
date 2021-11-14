using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cells;

public class LevelLogic : MonoBehaviour
{

    private void Start() {
        _gridManager = GetComponent<GridManager>();
    }

    public bool UpdateLevel() {
        // Returns true if the level is complete
        // Find all source cells
        List<GridCell> sourceCells = _gridManager.GetAllCellsOfTypes(new List<CellType>() {
            CellType.SourceRight, CellType.SourceLeft, CellType.SourceUp, CellType.SourceDown});
        // Find all path cells
        List<GridCell> pathCells = _gridManager.GetAllCellsOfTypes(new List<CellType>() {
            CellType.PathRight, CellType.PathLeft, CellType.PathUp, CellType.PathDown});
        // Reset all path cells
        foreach (GridCell cell in pathCells) {
            cell.SetBottomCell(CellManager.Instance.emptyCell);
        }

        bool allSourcesComplete = true;
        // Update paths
        foreach (GridCell sourceCell in sourceCells) {
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            Vector2Int pos = sourceCell.GridPosition;
            Vector2Int direction = CellManager.Instance.typeToDirection[sourceCell.TopCellType];
            int steps = 0;
            bool complete = false;
            // TODO: Replace with a normal exit condition
            while (steps < 100) {
                pos += direction;
                if (visited.Contains(pos)) {
                    print("Found a loop");
                    break;
                }

                if (!_gridManager.IsInsideGrid(pos))
                    break;
                var currentCell = _gridManager.GetCell(pos);
                if (currentCell.TopCellType == CellType.Destination) {
                    // Arrived at the desination
                    complete = true;
                    break;
                } else if (currentCell.TopCellType == CellType.Empty ||
                    currentCell.TopCellType == CellType.PathRight ||
                    currentCell.TopCellType == CellType.PathLeft ||
                    currentCell.TopCellType == CellType.PathUp ||
                    currentCell.TopCellType == CellType.PathDown) {
                    // Advance path
                    var pathCell = CellManager.Instance.typeToCell[CellManager.Instance.directionToPath[direction]];
                    _gridManager.SetBottomCell(pos, pathCell);
                } else {
                    break;
                }
                visited.Add(pos);
            }
            
            if (!complete)
                allSourcesComplete = false;
        }

        return allSourcesComplete;
    }

    private GridManager _gridManager;
}

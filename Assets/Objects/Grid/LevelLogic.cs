using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cells;

public class LevelLogic : MonoBehaviour
{
    public List<List<GridCell>> CurrentPaths => _currentPaths;
    private void Start() {
        _currentPaths = new List<List<GridCell>>();
        _leftMirrorToDirection = new Dictionary<Vector2Int, Vector2Int>() {
            {Vector2Int.right, Vector2Int.down},
            {Vector2Int.up, Vector2Int.left},
            {Vector2Int.down, Vector2Int.right},
            {Vector2Int.left, Vector2Int.up}
        };
        _gridManager = GetComponent<GridManager>();
    }

    public bool UpdateLevel() {
        // Returns true if the level is complete
        // Find all source cells
        List<GridCell> sourceCells = _gridManager.GetAllCellsOfType(CellType.Source);
        _currentPaths.Clear();
        // Reset all path cells
        _gridManager.MarkAllCellPath(false);

        bool allSourcesComplete = true;
        // Update paths
        foreach (GridCell sourceCell in sourceCells) {
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            Vector2Int pos = sourceCell.GridPosition;
            Vector2Int direction = sourceDirection;
            int steps = 0;
            bool complete = false;
            
            // TODO: Replace with a normal exit condition
            while (steps < 100) {
                visited.Add(pos);
                pos += direction;
                if (visited.Contains(pos)) {
                    print("Found a loop");
                    break;
                }

                if (!_gridManager.IsInsideGrid(pos))
                    break;
                var currentCell = _gridManager.GetCell(pos);
                if (currentCell.TopCell.type == CellType.Destination) {
                    // Arrived at the desination
                    visited.Add(pos);
                    complete = true;
                    break;
                } else if (currentCell.TopCell.walkable) {
                    // Advance path
                    if (currentCell.TopCell.type == CellType.MirrorLeft)
                        direction = _leftMirrorToDirection[direction];
                    else if (currentCell.TopCell.type == CellType.MirrorRight)
                        direction = -_leftMirrorToDirection[direction];
                    // currentCell.MarkPath(true);
                } else {
                    break;
                }
                
            }
            List<GridCell> path = new List<GridCell>();
            foreach (Vector2Int visitedPos in visited) {
                path.Add(_gridManager.GetCell(visitedPos));
            }
            _currentPaths.Add(path);
            
            if (!complete)
                allSourcesComplete = false;
        }

        return allSourcesComplete;
    }

    private Dictionary<Vector2Int, Vector2Int> _leftMirrorToDirection;
    private List<List<GridCell>> _currentPaths;
    private Vector2Int sourceDirection = Vector2Int.right;
    private GridManager _gridManager;
}

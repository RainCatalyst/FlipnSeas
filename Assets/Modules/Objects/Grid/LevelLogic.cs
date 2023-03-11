using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cells;

public class LevelLogic : MonoBehaviour
{
    public List<List<GridCell>> CurrentPaths => _currentPaths;

    private void Start()
    {
        _currentPaths = new List<List<GridCell>>();
        _gridManager = GetComponent<GridManager>();
    }

    public bool UpdateLevel()
    {
        // Returns true if the level is complete
        // Find all source cells
        List<GridCell> sourceCells = _gridManager.GetAllCellsOfType(CellType.Source);
        _currentPaths.Clear();
        // Reset all path cells
        _gridManager.MarkAllCellPath(false);

        bool allSourcesComplete = true;
        // Update paths
        foreach (GridCell sourceCell in sourceCells)
        {
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            Vector2Int pos = sourceCell.GridPosition;
            Vector2Int direction = _sourceDirection;
            int steps = 0;
            bool complete = false;

            // TODO: Replace with a normal exit condition
            while (steps < 100)
            {
                visited.Add(pos);
                pos += direction;
                if (visited.Contains(pos))
                {
                    print("Found a loop");
                    break;
                }

                if (!_gridManager.IsInsideGrid(pos))
                    break;
                var currentCell = _gridManager.GetCell(pos);
                // print(currentCell.TopCell.type);
                if (currentCell.TopCell.type == CellType.Destination)
                {
                    // Arrived at the desination
                    visited.Add(pos);
                    complete = true;
                    break;
                }
                else if (currentCell.TopCell.walkable)
                {
                    // Advance path
                    direction = UpdateDirection(currentCell.TopCell.type, direction);
                    // currentCell.MarkPath(true);
                }
                else
                {
                    break;
                }
            }

            List<GridCell> path = new List<GridCell>();
            foreach (Vector2Int visitedPos in visited)
            {
                path.Add(_gridManager.GetCell(visitedPos));
            }

            _currentPaths.Add(path);

            if (!complete)
                allSourcesComplete = false;
        }

        return allSourcesComplete;
    }

    private Vector2Int UpdateDirection(CellType type, Vector2Int direction)
    {
        switch (type)
        {
            case CellType.MirrorLeft:
                direction = _leftMirrorToDirection[direction];
                break;
            case CellType.MirrorRight:
                direction = -_leftMirrorToDirection[direction];
                break;
            case CellType.WhirlwindLeft:
                direction = _leftWhirlwindToDirection[direction];
                break;
            case CellType.WhirlwindRight:
                direction = -_leftWhirlwindToDirection[direction];
                break;
            case CellType.SignpostLeft:
                direction = Vector2Int.left;
                break;
            case CellType.SignpostRight:
                direction = Vector2Int.right;
                break;
            case CellType.SignpostUp:
                direction = Vector2Int.up;
                break;
            case CellType.SignpostDown:
                direction = Vector2Int.down;
                break;
        }

        return direction;
    }

    private Dictionary<Vector2Int, Vector2Int> _leftMirrorToDirection = new()
    {
        { Vector2Int.right, Vector2Int.down },
        { Vector2Int.up, Vector2Int.left },
        { Vector2Int.down, Vector2Int.right },
        { Vector2Int.left, Vector2Int.up }
    };

    private Dictionary<Vector2Int, Vector2Int> _leftWhirlwindToDirection = new()
    {
        { Vector2Int.right, Vector2Int.up },
        { Vector2Int.up, Vector2Int.left },
        { Vector2Int.down, Vector2Int.right },
        { Vector2Int.left, Vector2Int.down }
    };
    private List<List<GridCell>> _currentPaths;
    private Vector2Int _sourceDirection = Vector2Int.right;
    private GridManager _gridManager;
}
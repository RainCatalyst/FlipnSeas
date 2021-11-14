using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using Cells;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private float cellSize;
    [SerializeField] private GridCell cellPrefab;

    [SerializeField] private Transform cellHolder;
    [SerializeField] private LayerMask cellMask;

    public Vector2Int GridSize => gridSize;
    public GridCell[,] Grid => _grid;
    public GridCell HoveredCell => _hoveredCell;

    private void Start() {
        SpawnCells();
    }

    public GridCell GetCell(Vector2Int pos) {
        return _grid[pos.x, pos.y];
    }

    public List<GridCell> GetAllCellsOfTypes(List<CellType> types) {
        var cells = new List<GridCell>();
        for (int x = 0; x < gridSize.x; x++) {
            for (int y = 0; y < gridSize.y; y++) {
                if (types.Contains(_grid[x, y].TopCellType))
                    cells.Add(_grid[x, y]);
            }
        }
        return cells;
    }

    public void SetTopCell(Vector2Int pos, CellSO cell)
    {
        _grid[pos.x, pos.y].SetTopCell(cell);
    }

    public void SetBottomCell(Vector2Int pos, CellSO cell)
    {
        _grid[pos.x, pos.y].SetBottomCell(cell);
    }

    public void SetCellHighlight(Vector2Int pos, bool enable)
    {
        _grid[pos.x, pos.y].SetHighlight(enable);
    }

    public void SetAllCellHighlight(bool enable)
    {
        for (int x = 0; x < gridSize.x; x++) {
            for (int y = 0; y < gridSize.y; y++) {
                _grid[x, y].SetHighlight(enable);
            }
        }
    }

    public bool IsInsideGrid(Vector2Int pos) {
        return pos.x >= 0 && pos.y >= 0 && pos.x < gridSize.x && pos.y < gridSize.y;
    }

    private void SpawnCells() {
        // Initialize grid array
        _grid = new GridCell[gridSize.x, gridSize.y];

        // Spawn cell prefabs (empty)
        Vector2 offset = new Vector2(-cellSize * (gridSize.x / 2), -cellSize * (gridSize.y / 2));
        
        for (int x = 0; x < gridSize.x; x++) {
            for (int y = 0; y < gridSize.y; y++) {
                Vector3 cellPosition = transform.position + transform.forward * (offset.x + x * cellSize) - transform.right * (offset.y + y * cellSize);
                GridCell cell = Instantiate(cellPrefab, cellPosition, Quaternion.identity, cellHolder);
                cell.GridPosition = new Vector2Int(x, y);
                cell.SetTopCell(CellManager.Instance.emptyCell);
                cell.SetBottomCell(CellManager.Instance.emptyCell);
                _grid[x, y] = cell;
            }
        }
    }

    private void Update() {
        FindHoveredCell();
    }

    private void FindHoveredCell() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(_mouseAxis);

        if (Physics.Raycast(ray, out hit, 1000f, cellMask))
        {
            GridCell hitCell = hit.transform.GetComponent<GridCell>();
            if (_hoveredCell != hitCell) {
                if (_hoveredCell != null)
                    gameObject.SendMessage("OnCellUnhovered", _hoveredCell);
                if (hitCell != null)
                    gameObject.SendMessage("OnCellHovered", hitCell);
            }
        } else {
            if (_hoveredCell != null)
                gameObject.SendMessage("OnCellUnhovered", _hoveredCell);
        }
    }

    // Callbacks
    private void OnCellHovered(GridCell cell) {
        _hoveredCell = cell;
    }

    private void OnCellUnhovered(GridCell cell) {
        _hoveredCell = null;
    }

    // Input
    void OnMouseX(InputValue value)
    {
        _mouseAxis.x = value.Get<float>();
    }

    void OnMouseY(InputValue value)
    {
        _mouseAxis.y = value.Get<float>();
    }

    private GridCell[,] _grid;
    private GridCell _hoveredCell;
    private Vector2 _mouseAxis;
    
}

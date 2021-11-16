using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using Cells;
using DG.Tweening;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private float cellSize;
    [SerializeField] private GridCell cellPrefab;
    [SerializeField] private float rotationJitter;
    [SerializeField] private float positionJitter;
    [SerializeField] private Transform previousLevelHolder;
    [SerializeField] private Transform levelHolderPrefab;
    [SerializeField] private Transform cellHolder;
    [SerializeField] private LayerMask cellMask;

    [Header("Path Settings")]
    [SerializeField] private GridLine gridLinePrefab;

    public float CellSize => cellSize;
    public Vector2Int GridSize => gridSize;
    public GridCell[,] Grid => _grid;
    public GridCell HoveredCell => _hoveredCell;

    private void Start() {
        _gridLines = new GridLine[0];
    }

    public void UpdatePaths(List<List<GridCell>> paths) {
        foreach (GridLine line in _gridLines) {
            GameObject.Destroy(line.gameObject);
        }

        _gridLines = new GridLine[paths.Count];
        for (int i = 0; i < paths.Count; i++) {
            GridLine line = Instantiate(gridLinePrefab, transform.position, Quaternion.identity, cellHolder);
            line.SetPath(paths[i]);
            _gridLines[i] = line;
        }
    }

    public GridLine AddLine(GridCell origin, GridCell destination) {
        GridLine line = Instantiate(gridLinePrefab, transform.position, Quaternion.identity, cellHolder);
        line.SetPath(new List<GridCell>() {origin, destination});
        return line;
    }

    public void RevealPaths() {
        foreach (GridLine line in _gridLines) {
            line.RevealPath();
        }
    }

    public GridCell GetCell(Vector2Int pos) {
        return _grid[pos.x, pos.y];
    }

    public List<GridCell> GetAllCellsOfType(CellType type) {
        var cells = new List<GridCell>();
        for (int x = 0; x < gridSize.x; x++) {
            for (int y = 0; y < gridSize.y; y++) {
                if (_grid[x, y].TopCell.type == type)
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

    public void MarkCellPath(Vector2Int pos, bool mark)
    {
        _grid[pos.x, pos.y].MarkPath(mark);
    }

    public void SetAllCellHighlight(bool enable)
    {
        for (int x = 0; x < gridSize.x; x++) {
            for (int y = 0; y < gridSize.y; y++) {
                _grid[x, y].SetHighlight(enable);
            }
        }
    }

    public void MarkAllCellPath(bool mark)
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                _grid[x, y].MarkPath(mark);
            }
        }
    }

    public bool IsInsideGrid(Vector2Int pos) {
        return pos.x >= 0 && pos.y >= 0 && pos.x < gridSize.x && pos.y < gridSize.y;
    }

    public void InitializeGrid(Vector2Int size) {
        gridSize = size;
        // Initialize grid array
        _grid = new GridCell[gridSize.x, gridSize.y];
        _gridLines = new GridLine[0];

        // Spawn cell prefabs (empty)
        Vector2 offset = new Vector2(-cellSize * (gridSize.x / 2), -cellSize * (gridSize.y / 2));
        var spawnSequence = DOTween.Sequence();
        for (int x = 0; x < gridSize.x; x++) {
            for (int y = 0; y < gridSize.y; y++) {
                Vector3 cellPosition = transform.position + transform.forward * (offset.x + x * cellSize) - transform.right * (offset.y + y * cellSize);
                Quaternion rotationOffset = Quaternion.AngleAxis(Random.Range(-rotationJitter, rotationJitter), Vector3.up);
                Vector3 positionOffset = Vector3.right * Random.Range(-positionJitter, positionJitter) + Vector3.forward * Random.Range(-positionJitter, positionJitter);
                GridCell cell = Instantiate(cellPrefab, cellPosition + positionOffset, Quaternion.identity * rotationOffset, cellHolder);
                cell.GridPosition = new Vector2Int(x, y);
                cell.SetTopCell(CellManager.Instance.emptyCell);
                cell.SetBottomCell(CellManager.Instance.emptyCell);
                _grid[x, y] = cell;

                spawnSequence.InsertCallback(0.1f + 0.05f * (x + y), () => cell.FallAnimation());
            }
        }
        // spawnSequence.PrependInterval(3);
    }

    public void UnparentCurrentLevel() {
        if (cellHolder.childCount > 0)
        {
            SetAllCellHighlight(false);
            cellHolder.SetParent(previousLevelHolder);
            cellHolder = Instantiate(levelHolderPrefab, transform.position, Quaternion.identity, transform);
        }
    }

    public void FindHoveredCell() {
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

    private List<Transform> previousLevels;
    private GridLine[] _gridLines;
    private GridCell[,] _grid;
    private GridCell _hoveredCell;

    private Vector2 _mouseAxis;
    
}

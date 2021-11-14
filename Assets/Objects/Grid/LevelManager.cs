using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(GridManager))]
public class LevelManager : MonoBehaviour
{
    [SerializeField] private Vector2Int highlightSize;

    [Header("Events")]
    [SerializeField] private LevelEventChannelSO levelEventChannel;
    [SerializeField] private GameEventChannelSO gameEventChannel;
    private void Start() {
        _gridManager = GetComponent<GridManager>();
        _levelLogic = GetComponent<LevelLogic>();
        _highlightedCells = new List<GridCell>();
    }

    private void OnEnable()
    {
        gameEventChannel.OnLevelLoad += OnLevelLoad;
    }

    private void OnDisable()
    {
        gameEventChannel.OnLevelLoad -= OnLevelLoad;
    }

    private void Update() {
        if (_clicked) {
            _clicked = false;
            if (_canClick) {
                if (_hoveredCell != null) {
                    FlipHighlighted();
                    StartCoroutine(UpdateLevel());
                }   
            }
        }
    }

    private void FlipHighlighted() {
        foreach (GridCell cell in _highlightedCells)
        {
            var inverseCell = CellManager.Instance.GetInverse(cell.TopCellType);
            cell.SetBottomCell(inverseCell);
            cell.Flip();
        }
    }

    private void FlipUpdated() {
        for (int x = 0; x < _gridManager.GridSize.x; x++) {
            for (int y = 0; y < _gridManager.GridSize.y; y++) {
                if (_gridManager.Grid[x, y].Updated)
                    _gridManager.Grid[x, y].Flip();
            }
        }
    }

    private void HighlightCells(Vector2Int pos) {
        _highlightedCells.Clear();
        var offset = highlightSize / 2;
        for (int x = 0; x < highlightSize.x; x++) {
            for (int y = 0; y < highlightSize.y; y++) {
                var cellPos = new Vector2Int(x, y) + pos - offset;
                if (_gridManager.IsInsideGrid(cellPos)) {
                    _highlightedCells.Add(_gridManager.GetCell(cellPos));
                    _gridManager.SetCellHighlight(cellPos, true);
                }
            }
        }
    }

    // Callbacks
    private void OnClick() {
        _clicked = true;
    }

    private void OnCellHovered(GridCell cell)
    {
        _hoveredCell = cell;
        HighlightCells(cell.GridPosition);
    }

    private void OnCellUnhovered(GridCell cell)
    {
        _hoveredCell = null;
        _gridManager.SetAllCellHighlight(false);
        _highlightedCells.Clear();
    }

    private void OnLevelLoad(LevelSO level)
    {
        print("Loading level");
        _currentLevel = level;
        var cells = level.layout.GetCells();
        var levelCenter = _gridManager.GridSize / 2 - level.size / 2;
        // TODO: Also replace cells outside of level
        for (int x = 0; x < level.size.x; x++)
        {
            for (int y = 0; y < level.size.y; y++)
            {
                CellSO cellSO = CellManager.Instance.typeToCell[cells[level.size.y - y - 1, x]];
                var cellPos = levelCenter + new Vector2Int(x, y);
                _gridManager.SetBottomCell(cellPos, cellSO);
                StartCoroutine(FlipCellDelay(_gridManager.GetCell(cellPos), 0.1f + 0.035f * (x + y)));
            }
        }
    }

    private IEnumerator FlipCellDelay (GridCell cell, float delay) {
        yield return new WaitForSeconds(delay);
        cell.Flip();
    }

    private IEnumerator UpdateLevel () {
        _canClick = false;
        yield return new WaitForSeconds(0.5f);
        print(_levelLogic.UpdateLevel());
        FlipUpdated();
        _canClick = true;
    }

    private List<GridCell> _highlightedCells;

    private bool _canClick = true;
    private bool _clicked;
    private GridManager _gridManager;

    private LevelLogic _levelLogic;
    private GridCell _hoveredCell;
    private LevelSO _currentLevel;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

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
        // Update currenty hovered cell
        _gridManager.FindHoveredCell();

        if (_clicked) {
            _clicked = false;
            if (_canClick && _hoveredCell != null) {
                FlipHighlighted();
                UpdateLevel();
            }
        }
    }

    private void FlipHighlighted() {
        foreach (GridCell cell in _highlightedCells)
        {
            var inverseCell = CellManager.Instance.GetInverse(cell.TopCell.type);
            cell.SetBottomCell(inverseCell);
            cell.Flip();
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

    private void UpdateLevel() {
        _levelLogic.UpdateLevel();
        _gridManager.UpdatePaths(_levelLogic.CurrentPaths);
        _gridManager.RevealPaths();
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
        _canClick = false;
        _currentLevel = level;
        var cells = level.layout.GetCells();
        var levelCenter = _gridManager.GridSize / 2 - level.size / 2;
        var levelSequence = DOTween.Sequence().OnComplete(AfterLevelLoad);
        // TODO: Also replace cells outside of level
        for (int x = 0; x < level.size.x; x++)
        {
            for (int y = 0; y < level.size.y; y++)
            {
                CellSO cellSO = CellManager.Instance.typeToCell[cells[level.size.y - y - 1, x]];
                var cellPos = levelCenter + new Vector2Int(x, y);
                _gridManager.SetBottomCell(cellPos, cellSO);
                levelSequence.InsertCallback(0.1f + 0.035f * (x + y), () => _gridManager.GetCell(cellPos).Flip());
            }
        }

    }

    private void AfterLevelLoad() {
        _canClick = true;
        UpdateLevel();
    }

    private List<GridCell> _highlightedCells;

    private bool _canClick = true;
    private bool _clicked;
    private GridManager _gridManager;

    private LevelLogic _levelLogic;
    private GridCell _hoveredCell;
    private LevelSO _currentLevel;
}

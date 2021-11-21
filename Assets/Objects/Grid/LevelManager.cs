using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

[RequireComponent(typeof(GridManager))]
public class LevelManager : MonoBehaviour
{
    [SerializeField] private Vector2Int highlightSize;

    [SerializeField] private AudioSource fallSound;
    [SerializeField] private AudioSource flipSound;

    [Header("Events")]
    [SerializeField] private LevelEventChannelSO levelEventChannel;
    [SerializeField] private GameEventChannelSO gameEventChannel;

    public Vector2 LevelSize => new Vector2(_gridManager.GridSize.x, _gridManager.GridSize.y) * _gridManager.CellSize;

    private void Start() {
        _gridManager = GetComponent<GridManager>();
        _levelLogic = GetComponent<LevelLogic>();
        _exitPosition = transform.position;
    }

    private void OnEnable()
    {
        gameEventChannel.OnLevelLoad += OnLevelLoad;
        gameEventChannel.OnLevelRestart += OnLevelRestart;
        gameEventChannel.OnLevelWon += OnLevelWon;
        gameEventChannel.OnLevelLost += OnLevelLost;
    }

    private void OnDisable()
    {
        gameEventChannel.OnLevelLoad -= OnLevelLoad;
        gameEventChannel.OnLevelRestart -= OnLevelRestart;
        gameEventChannel.OnLevelWon -= OnLevelWon;
        gameEventChannel.OnLevelLost -= OnLevelLost;
    }

    private void Update() {
        // Update currenty hovered cell
        _gridManager.FindHoveredCell();

        if (_clicked) {
            _clicked = false;
            if (_canClick && _hoveredCell != null) {
                FlipHighlighted(_hoveredCell.GridPosition);
                _flipsLeft--;
                levelEventChannel.UseFlip();
                UpdateLevel();
            }
        }
    }

    private void FlipHighlighted(Vector2Int pos) {
        // _flipSequence = DOTween.Sequence().OnComplete(() => {
        //     _gridManager.RevealPaths();
        //     print("Revealing");
        // }).SetAutoKill(false);
        var offset = highlightSize / 2;
        for (int x = 0; x < highlightSize.x; x++) {
            for (int y = 0; y < highlightSize.y; y++) {
                var cellPos = new Vector2Int(x, y) + pos - offset;
                if (_gridManager.IsInsideGrid(cellPos))
                {
                    var cell = _gridManager.GetCell(cellPos);
                    var inverseCell = CellManager.Instance.GetInverse(cell.TopCell.type);
                    if (inverseCell.type != cell.TopCell.type) {
                        cell.SetBottomCell(inverseCell);
                        cell.Flip(0.035f * (x + y));
                    }
                    //flipSequence.InsertCallback(0.035f * (x + y), () => cell.Flip());
                }
            }
        }

        PlayFlip(0.145f);
    }

    private void HighlightCells(Vector2Int pos) {
        //_highlightedCells.Clear();
        var offset = highlightSize / 2;
        for (int x = 0; x < highlightSize.x; x++) {
            for (int y = 0; y < highlightSize.y; y++) {
                var cellPos = new Vector2Int(x, y) + pos - offset;
                if (_gridManager.IsInsideGrid(cellPos)) {
                    var cell = _gridManager.GetCell(cellPos);
                    if (cell.TopCell.type != CellManager.Instance.GetInverse(cell.TopCell.type).type)
                    {
                        _gridManager.SetCellHighlight(cellPos, true);
                    }
                    //_highlightedCells.Add(_gridManager.GetCell(gri));
                    
                }
            }
        }
    }

    private void UpdateLevel() {
        bool complete = _levelLogic.UpdateLevel();
        _gridManager.UpdatePaths(_levelLogic.CurrentPaths);
        _gridManager.RevealPaths();
        if (complete)
            gameEventChannel.WinLevel();
        else if (_flipsLeft <= 0)
            gameEventChannel.LoseLevel();
    }

    private Sequence LoadCells(LevelSO level) {
        _canClick = false;
        _flipsLeft = level.flips;

        var cells = level.layout.GetCells();
        //var levelCenter = _gridManager.GridSize / 2 - level.size / 2;
        var levelSequence = DOTween.Sequence().OnComplete(AfterLevelLoad);
        // TODO: Also replace cells outside of level
        for (int x = 0; x < level.size.x; x++)
        {
            for (int y = 0; y < level.size.y; y++)
            {
                CellSO cellSO = CellManager.Instance.typeToCell[cells[level.size.y - y - 1, x]];
                var cellPos = new Vector2Int(x, y);
                _gridManager.SetBottomCell(cellPos, cellSO);
            }
        }
        levelSequence.PrependInterval(1f);
        return levelSequence;
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
        // _highlightedCells.Clear();
    }

    private void PlayFlip(float delay) {
        flipSound.pitch = Random.Range(0.91f, 1.175f);
        flipSound.PlayDelayed(delay);
    }

    private void PlayFall(float delay) {
        fallSound.pitch = Random.Range(0.95f, 1.05f);
        fallSound.PlayDelayed(delay);
    }

    private void OnLevelLoad(LevelSO level)
    {
        print("Loading level");
        _gridManager.UnparentCurrentLevel();
        // Move level
        GridCell exitCell = null;
        if (_currentLevel != null) {
            float levelWidthX = (level.size.x + 1);
            //if (level.size.x % 2 == 0)
            //\    levelWidthX += 0.5f;
            transform.position = _exitPosition + transform.forward * levelWidthX * _gridManager.CellSize / 2;
            exitCell = _gridManager.GetCell(_currentLevel.exit);
        }
        
        _currentLevel = level;

        // Initialize grid
        _gridManager.InitializeGrid(level.size);

        // Set exit position
        var exitCellPos = _gridManager.GetCell(level.exit).transform.position;
        _exitPosition = new Vector3(exitCellPos.x, _exitPosition.y, exitCellPos.z);

        // Update cells
        var levelSequence = LoadCells(level);
        // Connect exit line
        if (exitCell != null) {
            var exitLine = _gridManager.AddLine(exitCell, _gridManager.GetCell(level.entrance));
            levelSequence.AppendCallback(() => exitLine.RevealPath());
            levelSequence.AppendCallback(() => exitLine.Pulse(true));
        }

        PlayFall(0.55f);
    }

    private void OnLevelRestart(){
        LoadCells(_currentLevel);
    }

    private void AfterLevelLoad() {
        _canClick = true;
        for (int x = 0; x < _gridManager.GridSize.x; x++) {
            for (int y = 0; y < _gridManager.GridSize.y; y++) {
                _gridManager.GetCell(new Vector2Int(x, y)).Flip(0.035f * (x + y));
            }
        }
        PlayFlip(0.145f);
        UpdateLevel();
        gameEventChannel.StartLevel();
    }

    private void OnLevelWon() {
        _canClick = false;
        _gridManager.PulsePaths(true);
    }

    private void OnLevelLost() {
        _canClick = false;
        _gridManager.PulsePaths(false);
    }

    //private Dictionary<GridCell, Vector2Int> _highlightedCells = new Dictionary<GridCell, Vector2Int>();

    //private Sequence _flipSequence;
    private Vector3 _exitPosition;
    private int _flipsLeft = 0;

    private bool _canClick = true;
    private bool _clicked;
    private GridManager _gridManager;

    private LevelLogic _levelLogic;
    private GridCell _hoveredCell;
    private LevelSO _currentLevel;
}

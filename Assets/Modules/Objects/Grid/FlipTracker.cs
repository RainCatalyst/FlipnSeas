using UnityEngine;

public class FlipTracker : MonoBehaviour
{
    [SerializeField] private GridCell flipCellPrefab;
    [SerializeField] private float offsetSize;
    [SerializeField] private CellSO topSideCell;
    [SerializeField] private CellSO bottomSideCell;

    [Header("Events")]
    [SerializeField] private LevelEventChannelSO levelEventChannel;
    [SerializeField] private GameEventChannelSO gameEventChannel;

    private void OnEnable()
    {
        levelEventChannel.OnFlipUsed += OnFlipUsed;
        gameEventChannel.OnLevelStarted += OnLevelStarted;
        gameEventChannel.OnLevelClosed += OnLevelClosed;
        gameEventChannel.OnLevelLoad += OnLevelLoad;
    }

    private void OnDisable()
    {
        levelEventChannel.OnFlipUsed -= OnFlipUsed;
        gameEventChannel.OnLevelStarted -= OnLevelStarted;
        gameEventChannel.OnLevelClosed -= OnLevelClosed;
        gameEventChannel.OnLevelLoad -= OnLevelLoad;
    }

    private void OnFlipUsed() {
        _flipsLeft = Mathf.Clamp(_flipsLeft - 1, 0, _flipCells.Length);
        if (_flipCells[_flipsLeft].TopCell == topSideCell)
            _flipCells[_flipsLeft].Flip();
    }

    private void OnLevelClosed() {
        // Unparent coins
        foreach (GridCell cell in _flipCells) {
            cell.transform.SetParent(null, true);
            Destroy(cell);
        }
    }

    private void OnLevelLoad(LevelSO level) {
        _flipsLeft = level.Flips;

        // Spawn flip cells
        _flipCells = new GridCell[_flipsLeft];
        
        for (int i = 0; i < _flipsLeft; i++) {
            // I am losing my mind
            float xOffset = (i - _flipsLeft / 2);
            if (_flipsLeft % 2 == 0)
                xOffset += 0.5f;
            Vector3 offset = transform.forward * (xOffset * offsetSize);
            GridCell cell = Instantiate(flipCellPrefab, transform.position + offset, Quaternion.identity, transform);
            cell.SetTopCell(topSideCell);
            cell.SetBottomCell(bottomSideCell);
            cell.FallAnimation();
            _flipCells[i] = cell;
        }
    }

    private void OnLevelStarted() {
        _flipsLeft = _flipCells.Length;
        int count = 0;
        foreach (GridCell cell in _flipCells) {
            if (cell.TopCell != topSideCell) {
                cell.Flip(0.25f * count);
                count++;
            }
        }
    }

    private int _flipsLeft;
    private GridCell[] _flipCells = new GridCell[0];
}

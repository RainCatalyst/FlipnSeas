using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Cells;
using DG.Tweening;

public class GridCell : MonoBehaviour {
    [SerializeField] private GameObject cellHighlight;

    [SerializeField] private GameObject overlay;
    [SerializeField] private Transform cellHolder;
    [SerializeField] private Transform topHolder;
    [SerializeField] private Transform bottomHolder;
    
    [Header("Events")]
    [SerializeField] private LevelEventChannelSO levelEventChannel;

    public bool Updated => _updated;

    public bool MarkedAsPath => _markedAsPath;

    public Vector2Int GridPosition
    {
        get => _gridPosition;
        set { _gridPosition = value; }
    }

    public CellSO TopCell => _topCell;
    public CellSO BottomCell => _bottomCell;

    private void Start() {
        _animation = GetComponent<Animation>();
        holderRotation = cellHolder.localRotation;
        topRotation = topHolder.localRotation;
        bottomRotation = bottomHolder.localRotation;
    }

    public void MarkPath(bool mark) {
        _markedAsPath = mark;
        overlay.SetActive(mark);
    }

    public void SetHighlight(bool enable) {
        cellHighlight.SetActive(enable);
        _highlighted = enable;
    }

    public void SetTopCell(CellSO cellSO) {
        // Replace top cell with new CellSO
        _topCell = cellSO;
        if (_topCellVisuals != null)
            GameObject.Destroy(_topCellVisuals.gameObject);
        _topCellVisuals = Instantiate(cellSO.prefab, topHolder.position, topRotation, topHolder);
    }

    public void SetBottomCell(CellSO cellSO) {
        // Replace bottom cell with new CellSO
        if (_bottomCell != cellSO)
            _updated = true;
        _bottomCell = cellSO;
        if (_bottomCellVisuals != null)
            GameObject.Destroy(_bottomCellVisuals.gameObject);
        _bottomCellVisuals = Instantiate(cellSO.prefab, bottomHolder.position, bottomRotation, bottomHolder);
        
    }

    public void Flip() {
        flipSequence?.Complete();
        // Swap references
        var temp = _topCell;
        _topCell = _bottomCell;
        _bottomCell = temp;
        var temp2 = _topCellVisuals;
        _topCellVisuals = _bottomCellVisuals;
        _bottomCellVisuals = temp2;
        _updated = false;

        // Play flip animation
        flipSequence = DOTween.Sequence().OnComplete(SwapSides);
        flipSequence.Append(cellHolder.DORotate(new Vector3(180f, 0, 0), 0.25f).SetEase(Ease.InOutCubic));
        flipSequence.Insert(0, cellHolder.DOLocalMoveY(0.1f, 0.125f).SetEase(Ease.InOutCubic).SetLoops(2, LoopType.Yoyo));
    }

    public void SwapSides() {
        _topCellVisuals.transform.SetParent(topHolder);
        _topCellVisuals.transform.localRotation = topRotation;
        _topCellVisuals.transform.localPosition = Vector3.zero;
        _bottomCellVisuals.transform.SetParent(bottomHolder);
        _bottomCellVisuals.transform.localRotation = bottomRotation;
        _bottomCellVisuals.transform.localPosition = Vector3.zero;
        cellHolder.transform.localRotation = holderRotation;
    }
    private bool _highlighted = false;
    private bool _updated = false;

    private bool _markedAsPath = false;
    private CellVisuals _topCellVisuals;
    private CellVisuals _bottomCellVisuals;

    private Sequence flipSequence;

    private Quaternion holderRotation;

    private Quaternion topRotation;
    private Quaternion bottomRotation;

    private CellSO _topCell;
    private CellSO _bottomCell;
    private Vector2Int _gridPosition;
    private Animation _animation;
}
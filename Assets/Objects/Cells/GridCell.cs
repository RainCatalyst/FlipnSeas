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
        cellHolder.localPosition = new Vector3(0, 5f, 0);
    }

    public void FallAnimation() {
        cellHolder.DOLocalMoveY(0f, 0.6f).SetEase(Ease.InQuad);
        cellHolder.DORotate(new Vector3(Random.Range(-90f, 90f), Random.Range(-20f, 20f), 0f), 0.6f).From().SetEase(Ease.InQuad);
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
        flipSequence?.Complete();
        // Replace top cell with new CellSO
        _topCell = cellSO;
        if (_topCellVisuals != null)
            GameObject.Destroy(_topCellVisuals.gameObject);
        _topCellVisuals = Instantiate(cellSO.prefab, topHolder.position, topRotation, topHolder);
    }

    public void SetBottomCell(CellSO cellSO) {
        flipSequence?.Complete();
        // Replace bottom cell with new CellSO
        if (_bottomCell != cellSO)
            _updated = true;
        _bottomCell = cellSO;
        if (_bottomCellVisuals != null)
            GameObject.Destroy(_bottomCellVisuals.gameObject);
        _bottomCellVisuals = Instantiate(cellSO.prefab, bottomHolder.position, bottomRotation, bottomHolder);
    }

    public Sequence Flip(float visualsDelay = 0f) {
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
        // flipSequence.Append(cellHolder.DORotate(new Vector3(180f, 0, 0), .25f).SetEase(Ease.InOutCubic));
        // flipSequence.Insert(0, cellHolder.DOLocalMoveY(0.15f, .125f).Setase(Ease.InOutCubic).SetLoops(2, LoopType.Yoyo));

        flipSequence.Append(cellHolder.DOLocalMoveY(0.3f, 0.2f).SetEase(Ease.InOutCubic).SetLoops(2, LoopType.Yoyo));
        flipSequence.Insert(0.05f, cellHolder.DOLocalRotate(new Vector3(180f, 0, 0), 0.3f).SetEase(Ease.InOutCubic));

        // flipSequence.Append(cellHolder.DOLocalMoveY(0.3f, 1f).SetEase(Ease.InOutCubic).SetLoops(2, LoopType.Yoyo));
        // flipSequence.Insert(0.05f, cellHolder.DOLocalRotate(new Vector3(180f, 0, 0), 1f).SetEase(Ease.InOutCubic));
        // //flipSequence.InsertCallback(0.55f, () => _bottomCellVisuals.gameObject.SetActive(false));

        flipSequence.PrependInterval(visualsDelay);
        return flipSequence;
    }

    public void SwapSides() {
        _topCellVisuals.transform.SetParent(topHolder);
        _topCellVisuals.transform.localRotation = Quaternion.identity;
        _topCellVisuals.transform.localPosition = Vector3.zero; // new Vector3(0, 0.0005f, 0);
        _bottomCellVisuals.transform.SetParent(bottomHolder);
        _bottomCellVisuals.transform.localRotation = Quaternion.identity;
        _bottomCellVisuals.transform.localPosition = Vector3.zero; // - new Vector3(0, -0.0005f, 0);
        //_bottomCellVisuals.gameObject.SetActive(true);
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
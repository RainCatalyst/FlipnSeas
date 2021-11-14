using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Cells;

public class GridCell : MonoBehaviour {
    [SerializeField] private GameObject cellHighlight;

    [SerializeField] private Transform cellHolder;
    [SerializeField] private Transform topHolder;
    [SerializeField] private Transform bottomHolder;
    
    [Header("Events")]
    [SerializeField] private LevelEventChannelSO levelEventChannel;

    public bool Updated => _updated;

    public Vector2Int GridPosition
    {
        get => _gridPosition;
        set { _gridPosition = value; }
    }

    public CellType TopCellType => _topCell.type;
    public CellType BottomCellType => _bottomCell.type;

    private void Start() {
        _animation = GetComponent<Animation>();
        holderRotation = cellHolder.localRotation;
        topRotation = topHolder.localRotation;
        bottomRotation = bottomHolder.localRotation;
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
        var temp = _topCell;
        _topCell = _bottomCell;
        _bottomCell = temp;
        var temp2 = _topCellVisuals;
        _topCellVisuals = _bottomCellVisuals;
        _bottomCellVisuals = temp2;
        _updated = false;
        // foreach (AnimationState state in _animation)
        // {
        //     state.speed = 0.1f;
        // }
        // Play flip animation
        _animation.Play();
    }

    public void SwapSides() {
        StartCoroutine(SwapSidesCoroutine());
    }

    private IEnumerator SwapSidesCoroutine() {
        yield return null;
        
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
    private CellVisuals _topCellVisuals;
    private CellVisuals _bottomCellVisuals;

    private Quaternion holderRotation;

    private Quaternion topRotation;
    private Quaternion bottomRotation;

    private CellSO _topCell;
    private CellSO _bottomCell;
    private Vector2Int _gridPosition;
    private Animation _animation;
}
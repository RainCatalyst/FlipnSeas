using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cells;

public class CellManager : MonoBehaviour
{
    [SerializeField] CellSO[] cells;

    public CellSO emptyCell;

    public Dictionary<CellType, CellSO> typeToCell;
    public Dictionary<CellType, CellType> typeToInverse;

    public Dictionary<CellType, Vector2Int> typeToDirection;
    public Dictionary<Vector2Int, CellType> directionToPath;


    private void Start() {
        typeToCell = new Dictionary<CellType, CellSO>();
        typeToInverse = new Dictionary<CellType, CellType>();
        typeToDirection = new Dictionary<CellType, Vector2Int>() {
            {CellType.SourceRight, Vector2Int.right},
            {CellType.SourceLeft, Vector2Int.left},
            {CellType.SourceUp, Vector2Int.up},
            {CellType.SourceDown, Vector2Int.down}
        };

        directionToPath = new Dictionary<Vector2Int, CellType>() {
          {Vector2Int.right, CellType.PathRight},
          {Vector2Int.left, CellType.PathLeft},
          {Vector2Int.up, CellType.PathUp},
          {Vector2Int.down, CellType.PathDown}  
        };
        // Initialize dictionaries
        foreach (CellSO cell in cells) {
            typeToCell.Add(cell.type, cell);
            typeToInverse.Add(cell.type, cell.inverseType);
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public CellSO GetInverse(CellType type) {
        return typeToCell[typeToInverse[type]];
    }

    public static CellManager Instance => _instance;
    private static CellManager _instance;
}
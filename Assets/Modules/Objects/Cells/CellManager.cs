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

    private void Start() {
        typeToCell = new Dictionary<CellType, CellSO>();
        typeToInverse = new Dictionary<CellType, CellType>();
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
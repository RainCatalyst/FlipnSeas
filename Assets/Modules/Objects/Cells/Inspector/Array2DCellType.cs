using UnityEngine;
using Cells;

namespace Array2DEditor
{
    [System.Serializable]
    public class Array2DCellType : Array2D<CellType>
    {
        [SerializeField]
        CellRowCellType[] cells = new CellRowCellType[Consts.defaultGridSize];

        protected override CellRow<CellType> GetCellRow(int idx)
        {
            return cells[idx];
        }
    }

    [System.Serializable]
    public class CellRowCellType : CellRow<CellType> { }
}

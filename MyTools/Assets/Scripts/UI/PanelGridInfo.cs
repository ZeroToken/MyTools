/*============================================================
                 Author Name: hongfeng.wu
                 Create Time: 2016-5-5 17:1:58
============================================================*/
using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class PanelGridInfo
{
    #region Member variables
    public int cellWidth = 200;

    public int offsetColumnCount;

    public int cellHeight = 200;

    public int offsetLineCount;

    public Transform cellPrefab;

    #endregion

    #region Property
    public int line { get; set; }
    public int column { get; set; }
    public int cellCount { get { return line * column; } }

    private Vector2 mSize;
    /// <summary>
    /// The grid size
    /// </summary>
    public Vector2 Size
    {
        get
        {
            if (mSize == Vector2.zero)
                mSize = new Vector2(cellWidth * column, cellHeight * line);
            return mSize;
        }
    }
    public List<Transform> cells { get; set; }

    private UIPanel mPanel;
    public UIPanel Panel
    {
        get { return mPanel; }
        set
        {
            mPanel = value;
            line = GetLine();
            column = GetColumn();
        }
    }

    private Vector2 mPanelSize = Vector2.zero;
    /// <summary>
    /// The panel size
    /// </summary>
    public Vector2 PanelSize
    {
        get
        {
            if (mPanelSize == Vector2.zero && Panel != null)
                mPanelSize = Panel.GetViewSize();
            return mPanelSize;
        }
    }
    /// <summary>
    /// The four corners of the matrix cell
    /// </summary>
    public Vector3[] Corners
    {
        get
        {
            if (Panel != null)
            {
                return Panel.localCorners;
                //Vector3[] corners = Panel.worldCorners;
                //for (int i = 0; i < corners.Length; i++)
                //    corners[i] = Panel.transform.InverseTransformPoint(corners[i]);
                //return corners;
            }
            return null;
        }
    }
    /// <summary>
    /// The form center
    /// </summary>
    public Vector3 Center
    {
        get
        {
            Vector3[] corners = Corners;
            if (corners != null)
                return Vector3.Lerp(corners[0], corners[2], 0.5f);
            return Vector2.zero;
        }
    }
    /// <summary>
    /// The four corners of the table cell
    /// </summary>
    public Transform[] CornerCells
    {
        get
        {
            Vector3[] corners = Corners;
            Transform[] cornerCells = new Transform[corners.Length];
            Vector3[] cornerCellPositions = new Vector3[corners.Length];
            for (int i = 0; i < cornerCellPositions.Length; i++)
                cornerCellPositions[i] = Panel.transform.InverseTransformPoint(cells[0].position);
            Vector3 cell = Vector3.zero;
            for (int i = 0; i < cells.Count; i++)
            {
                if(cells[i] == null) continue;
                cell = Panel.transform.InverseTransformPoint(cells[i].position);
                if (cell.x >= corners[1].x && cell.x <= corners[3].x && cell.y <= corners[1].y && cell.y >= corners[3].y)
                {
                    //The bottom left corner cell
                    if (cornerCells[0] == null || Mathf.Abs(cell.x - corners[0].x) <= Mathf.Abs(cornerCellPositions[0].x - corners[0].x)
                        && Mathf.Abs(corners[0].y - cell.y) <= Mathf.Abs(corners[0].y - cornerCellPositions[0].y))
                    {
                        cornerCells[0] = cells[i];
                        cornerCellPositions[0] = Panel.transform.InverseTransformPoint(cells[i].position);
                    }
                    //The upper left corner cell
                    if (cornerCells[1] == null || Mathf.Abs(cell.x - corners[1].x) <= Mathf.Abs(cornerCellPositions[1].x - corners[1].x)
                        && Mathf.Abs(cell.y - corners[1].y) <= Mathf.Abs(cornerCellPositions[1].y - corners[1].y))
                    {
                        cornerCells[1] = cells[i];
                        cornerCellPositions[1] = Panel.transform.InverseTransformPoint(cells[i].position);
                    }
                    //The upper right corner cell
                    if (cornerCells[2] == null || Mathf.Abs(corners[2].x - cell.x) <= Mathf.Abs(corners[2].x - cornerCellPositions[2].x)
                        && Mathf.Abs(cell.y - corners[2].y) <= Mathf.Abs(cornerCellPositions[2].y - corners[2].y))
                    {
                        cornerCells[2] = cells[i];
                        cornerCellPositions[2] = Panel.transform.InverseTransformPoint(cells[i].position);
                    }
                    //The bottom right corner cell
                    if (cornerCells[3] == null || Mathf.Abs(corners[3].x - cell.x) <= Mathf.Abs(corners[3].x - cornerCellPositions[3].x)
                        && Mathf.Abs(corners[3].y - cell.y) <= Mathf.Abs(corners[3].y - cornerCellPositions[3].y))
                    {
                        cornerCells[3] = cells[i];
                        cornerCellPositions[3] = Panel.transform.InverseTransformPoint(cells[i].position);
                    }
                }
            }
            return cornerCells;
        }
    }
    #endregion

    #region Event
    /// <summary>
    /// Calculate and obtain the line
    /// </summary>
    /// <returns></returns>
    public int GetColumn()
    {
        return (Mathf.Max((int)PanelSize.x, cellWidth) / cellWidth) + offsetColumnCount;
    }
    /// <summary>
    /// Calculate and obtain the column
    /// </summary>
    /// <returns></returns>
    public int GetLine()
    {
        return (Mathf.Max((int)PanelSize.y, cellHeight) / cellHeight) + offsetLineCount;
    }
    /// <summary>
    /// Expand table cell
    /// </summary>
    /// <param name="parent"></param>
    public void SpreadOutCells(Transform parent, int minCount, System.Action<Transform> onRefreshCell = null)
    {
        if (cellPrefab != null)
        {
            int x = 0, y = 0;
            cells = parent.SetChildren(Mathf.Min(minCount, cellCount), cellPrefab, (idx, cell) =>
            {
                if (cell != null)
                {
                    cell.localPosition = new Vector3(cellWidth * x, -cellHeight * y, 0);
                    if (onRefreshCell != null)
                        onRefreshCell(cell);
                }
                if (++x >= column)
                {
                    x = 0;
                    y++;
                }
            });
        }
    }
    /// <summary>
    /// Through all the cells
    /// </summary>
    /// <param name="onForeach"></param>
    public void ForeachCells(System.Action<Transform> onForeach)
    {
        if (onForeach != null)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                onForeach(cells[i]);
            }
        }
    }

    public Vector3 InverseTransformPoint(Transform cell)
    {
        if (Panel != null)
        {
            return Panel.transform.InverseTransformPoint(cell.position);
        }
        return Vector3.zero;
    }
    #endregion
}

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
                Vector3[] corners = Panel.worldCorners;
                for (int i = 0; i < corners.Length; i++)
                    corners[i] = Panel.transform.InverseTransformPoint(corners[i]);
                return corners;
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
            for (int i = 0; i < cornerCells.Length; i++)
                cornerCells[i] = cells[0];
            float distanceAx = 0, distanceBx = 0, distanceAy, distanceBy;
            Vector3 cell = Vector3.zero;
            for (int i = 1; i < cells.Count; i++)
            {
                if(cells[i] == null) continue;
                cell = cells[i].localPosition;
                if (cell.x >= corners[0].x && cell.x <= corners[3].x && cell.y <= corners[0].y && cell.y >= corners[3].y)
                {
                    //The upper left corner cell
                    distanceAx = cornerCells[0].localPosition.x - corners[0].x;
                    distanceBx = cell.x - corners[0].x;
                    if (distanceAx < 0 || distanceBx >= 0 && Mathf.Abs(distanceBx) <= Mathf.Abs(distanceAx))
                    {
                        distanceAy = cornerCells[0].localPosition.y - corners[0].y;
                        distanceBy = cell.y - corners[0].y;
                        if (distanceAy > 0 || distanceBy <= 0 && Mathf.Abs(distanceBy) <= Mathf.Abs(distanceAy))
                            cornerCells[0] = cells[i];
                    }
                }



                //distanceAx = cornerCells[0].localPosition.x - corners[0].x;
                //distanceB = cells[i].localPosition.x - corners[0].x;
                ////The upper left corner cell
                //if (distanceA < 0 || distanceB >= 0 && distanceB < distanceA)
                //{
                //    cornerCells[0] = cells[i];
                //    //The bottom left corner cell
                //    distanceA = corners[1].y - cornerCells[1].localPosition.y;
                //    distanceB = corners[1].y - cells[i].localPosition.y;
                //    if (distanceA < 0 || distanceB >= 0 && distanceB < distanceA)
                //    {
                //        cornerCells[1] = cells[i];
                //    }
                //}
                //else
                //{
                //    //The upper right corner cell
                //    distanceA = corners[2].x - cornerCells[2].localPosition.x;
                //    distanceB = corners[2].x - cells[i].localPosition.x;
                //    if (distanceA < 0 || distanceB >= 0 && distanceB < distanceA)
                //    {
                //        cornerCells[2] = cells[i];
                //        //The bottom right corner cell
                //        distanceA = corners[3].y - cornerCells[3].localPosition.y;
                //        distanceB = corners[3].y - cells[i].localPosition.y;
                //        if (distanceA < 0 || distanceB >= 0 && distanceB < distanceA)
                //        {
                //            cornerCells[3] = cells[i];
                //        }
                //    }
                //} 
            }
                //float offsetX = 0, offsetY = 0;
                //Transform cell = null;
                //for(int i = 0; i < cells.Count; i++)
                //{
                //    cell = cells[i];
                //    //The upper left corner cell
                //    offsetX = cell.localPosition.x - corners[0].x;
                //    offsetY = Mathf.Abs(cell.localPosition.y - corners[0].y);
                //    if (offsetX >= 0 && offsetX <= cellWidth && offsetY <= cellHeight)
                //        if(cornerCells[0] == null) cornerCells[0] = cell;
                //    else
                //    {
                //        //The bottom left corner cell
                //        offsetX = cell.localPosition.x - corners[1].x;
                //        offsetY = Mathf.Abs(corners[1].y - cell.localPosition.y);
                //        if (offsetX >= 0 && offsetX <= cellWidth && offsetY <= cellHeight)
                //            if (cornerCells[1] == null) cornerCells[1] = cell;
                //        else
                //        {
                //            //The upper right corner cell
                //            offsetX = corners[2].x - cell.localPosition.x;
                //            offsetY = cell.localPosition.y - corners[2].y;
                //            if (offsetX >= 0 && offsetX <= cellWidth && offsetY <= cellHeight)
                //                if (cornerCells[2] == null) cornerCells[2] = cell;
                //            else
                //            {

                //                //The bottom right corner cell
                //                offsetX = corners[3].x - cell.localPosition.x;
                //                offsetY = Mathf.Abs(corners[3].y - cell.localPosition.y);
                //                if (offsetX >= 0 && offsetX <= cellWidth && offsetY <= cellHeight)
                //                    if (cornerCells[3] == null) cornerCells[3] = cell;
                //            }
                //        }
                //    }
                //    cell = null;
                //}
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
    #endregion
}

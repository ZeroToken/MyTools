/*============================================================
                 Author Name: hongfeng.wu
                 Create Time: 2016-5-4 14:25:27
============================================================*/
using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class GridSlider : MonoBehaviour
{
    #region Member variables
    public UIScrollView.Movement movement;

    public UIWidget.Pivot pivot;

    public bool loopMove;

    private int column;

    private int line;

    private int cellCount;
    /// <summary>
    /// Whether the index is a positive sequence
    /// </summary>
    public bool isPositiveIndex = true;

    public PanelGridInfo grid;
    #endregion

    #region Property
    private UIScrollView mScrollView;
    public UIScrollView ScrollView
    {
        get
        {
            if (mScrollView == null)
                mScrollView = NGUITools.FindInParents<UIScrollView>(gameObject);
            return mScrollView;
        }
    }
    /// <summary>
    /// control reset scrollview
    /// </summary>
    public bool repositionNow { get; set; }
    /// <summary>
    /// control asynchronous slider
    /// </summary>
    private bool sliderNow { get; set; }

    private Vector3 mSliderToPosition;
    /// <summary>
    /// slider to a position
    /// </summary>
    public Vector3 sliderToPostion
    {
        get
        { return mSliderToPosition; }
        set
        {
            mSliderToPosition = value;
            sliderNow = true;
        }
    }
    /// <summary>
    /// delegate refresh cell is switched
    /// </summary>
    public DelegateAssembly.onTransformRefresh onSwitchCellRefresh { get; set; }
    /// <summary>
    /// delegate refresh in the visual field of the cell
    /// </summary>
    public DelegateAssembly.onTransformRefresh onVisualCellRefresh { get; set; }
    #endregion

    #region Event
    void Awake()
    {
        grid.Panel = NGUITools.FindInParents<UIPanel>(gameObject);
        if (grid.Panel != null) grid.Panel.onClipMove = OnMoveGrid;
    }
    /// <summary>
    /// Refresh cells when dragging
    /// </summary>
    /// <param name="panel"></param>
    void OnMoveGrid(UIPanel panel)
    {
        if (movement == UIScrollView.Movement.Horizontal)
            OnMoveHorizontal();
        else
            OnMoveVertical();
    }
    /// <summary>
    /// Refresh the cell when the horizontal drag
    /// </summary>
    void OnMoveHorizontal()
    {
        float extents = grid.Size.x * 0.5f;
        Vector3[] corners = grid.Corners;
        int i = 0;
        grid.ForeachCells((cell) =>
        {
            if (cell == null) return;
            float distance = cell.localPosition.x - grid.Center.x;
            if (distance < -extents)
            {
                Vector3 pos = cell.localPosition;
                pos.x += grid.Size.x;
                int realIndex = Mathf.RoundToInt(pos.x / grid.cellWidth);
                if ((0 <= realIndex && realIndex < column) || loopMove)
                {
                    cell.localPosition = pos;
                    this.SwitchCellRefresh(cell);
                }
            }
            else if (distance > extents)
            {
                Vector3 pos = cell.localPosition;
                pos.x -= grid.Size.x;
                int realIndex = Mathf.RoundToInt(pos.x / grid.cellWidth);
                if ((0 <= realIndex && realIndex < column) || loopMove)
                {
                    cell.localPosition = pos;
                    this.SwitchCellRefresh(cell);
                }
            }
            if (cell.localPosition.x < corners[2].x && cell.localPosition.x > corners[0].x)
            {
                if (onVisualCellRefresh != null) onVisualCellRefresh(GetCellIndex(cell), cell);
            }
            i++;
        });
    }
    /// <summary>
    /// Refresh the cell when the vertical drag
    /// </summary>
    void OnMoveVertical()
    {
        float extents = grid.Size.y * 0.5f;
        Vector3[] corners = grid.Corners;
        grid.ForeachCells((cell) =>
        {
            if (cell == null) return;
            float distance = cell.localPosition.y - grid.Center.y;
            if (distance < -extents)
            {
                Vector3 pos = cell.localPosition;
                pos.y += grid.Size.y;
                int realIndex = -Mathf.RoundToInt(pos.y / grid.cellHeight);
                if (0 <= realIndex && realIndex < line || loopMove)
                {
                    cell.localPosition = pos;
                    this.SwitchCellRefresh(cell);
                }
            }
            else if (distance > extents)
            {
                Vector3 pos = cell.localPosition;
                pos.y -= grid.Size.y;
                int realIndex = -Mathf.RoundToInt(pos.y / grid.cellHeight);
                if (0 <= realIndex && realIndex < line || loopMove)
                {
                    cell.localPosition = pos;
                    this.SwitchCellRefresh(cell);
                }
            }
            if (cell.localPosition.x > corners[2].y && cell.localPosition.x < corners[0].y)
            {
                if (onVisualCellRefresh != null) onVisualCellRefresh(GetCellIndex(cell), cell);
            }
        });
    }
    /// <summary>
    /// Asynchronous Refresh
    /// </summary>
    void Update()
    {
        if(repositionNow)
        {
            this.ScrollViewReset();
            repositionNow = false;
        }
        if(sliderNow)
        {
            try
            {
                this.SliderTo(sliderToPostion);
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.Message);
            }
            sliderNow = false;
        }
    }
    public void Initialize(int totalCount, DelegateAssembly.onTransformRefresh onRefrshCell)
    {
        this.onSwitchCellRefresh = onRefrshCell;
        this.Initialize(totalCount);
    }
    /// <summary>
    /// Initialize grid
    /// </summary>
    /// <param name="totalCount">cell count</param>
    public virtual void Initialize(int totalCount)
    {
        this.cellCount = totalCount;
        this.line = GetLine();
        this.column = GetColumn();
        this.grid.SpreadOutCells(transform, totalCount, SwitchCellRefresh);
        this.repositionNow = true;
    }
    /// <summary>
    /// reset scrollview info
    /// </summary>
    public void ScrollViewReset()
    {
        if (ScrollView != null)
        {
            ScrollView.movement = movement;
            ScrollView.contentPivot = pivot;
            ScrollView.ResetPosition();
        }
    }
    /// <summary>
    /// Calculate and obtain the column
    /// </summary>
    /// <param name="cellCount"></param>
    /// <returns></returns>
    public int GetColumn()
    {
        if (movement == UIScrollView.Movement.Horizontal)
            return cellCount / grid.line + Mathf.Min(cellCount % grid.line, 1);
        else if (movement == UIScrollView.Movement.Vertical)
            return grid.column;
        return 0;
    }
    /// <summary>
    ///  Calculate and obtain the line
    /// </summary>
    /// <param name="cellCount"></param>
    /// <returns></returns>
    public int GetLine()
    {
        if (movement == UIScrollView.Movement.Horizontal)
            return grid.line;
        else if (movement == UIScrollView.Movement.Vertical)
            return cellCount / grid.column + Mathf.Min(cellCount % grid.column, 1);
        return 0;
    }
    /// <summary>
    /// Calculate and obtain the cell column index
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public int GetColumnIndex(Transform cell)
    {
        if (cell != null)
        {
            int columnIndex = Mathf.RoundToInt(cell.localPosition.x / grid.cellWidth) % Mathf.Max(column, grid.column);
            if (columnIndex < 0) columnIndex += column;
            return columnIndex;
        }
        return 0;
    }
    /// <summary>
    /// Calculate and obtain the cell line index
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public int GetLineIndex(Transform cell)
    {
        if (cell != null)
        {
            int lineIndex = Mathf.RoundToInt(cell.localPosition.y / grid.cellHeight) % Mathf.Max(line, grid.line);
            if (lineIndex <= 0)
                lineIndex = Mathf.Abs(lineIndex);
            else
                lineIndex = line - lineIndex;
            return lineIndex;
        }
        return 0;
    }
    /// <summary>
    /// Calculate and obtain the cell index
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public int GetCellIndex(Transform cell)
    {
        int lineIndex = GetLineIndex(cell), columnIndex = GetColumnIndex(cell);
        return isPositiveIndex ? Mathf.Max(column, grid.column) * lineIndex + columnIndex : Mathf.Max(line, grid.line) * columnIndex + lineIndex;
    }
    /// <summary>
    /// Refresh cell is switched
    /// </summary>
    /// <param name="cell"></param>
    public void SwitchCellRefresh(Transform cell)
    {
        if (cell != null)
        {
            int index = this.GetCellIndex(cell);
            cell.name = grid.cellPrefab.name + index;
            cell.gameObject.SetActive(index < cellCount);
            if (onSwitchCellRefresh != null && cell.gameObject.activeSelf) 
                onSwitchCellRefresh(index, cell);
        }
    }
    /// <summary>
    /// Limit does not exceed the border
    /// </summary>
    /// <param name="index"></param>
    public void FocusOnCellWithLimit(int index)
    {
        index = Mathf.Max(0, index);
        if (movement == UIScrollView.Movement.Horizontal)
        {
            index = index / line;
            int realGridColumn = grid.column - grid.offsetColumnCount;
            index = index + realGridColumn > column ? column - realGridColumn : index;
        }
        else if(movement == UIScrollView.Movement.Vertical)
        {
            index = index / column;
            int realGridLine = grid.line - grid.offsetLineCount;
            index = index + realGridLine > line ? line - realGridLine : index;
        }
        this.FocusOnCell(index);
    }
    /// <summary>
    /// focus cell
    /// </summary>
    /// <param name="index">cell index</param>
    public void FocusOnCell(int index)
    {
        Vector3 to = Vector3.zero;
        Transform[] cornerCells = grid.CornerCells;
        if (movement == UIScrollView.Movement.Horizontal)
        {
            to = ScrollView.transform.localPosition - new Vector3(grid.cellWidth * (index - (GetCellIndex(cornerCells[0]) / line)), 0, 0);
        }
        else if (movement == UIScrollView.Movement.Vertical)
        {
            to = ScrollView.transform.localPosition + new Vector3(0, grid.cellHeight * (index - (GetCellIndex(cornerCells[2]) / column)), 0);
        }
        sliderToPostion = to;
    }
    /// <summary>
    /// Slide to a coordinate
    /// </summary>
    /// <param name="target"></param>
    /// <param name="stength"></param>
    public void SliderTo(Vector3 target, float stength = 20)
    {
        SpringPanel springPanel = SpringPanel.Begin(ScrollView.gameObject, target, stength);
        ScrollView.InvalidateBounds();
        springPanel.onFinished = () =>
        {
            InvalidateBounds();
        };
    }
    public void InvalidateBounds()
    {
        ScrollView.InvalidateBounds();
        ScrollView.restrictWithinPanel = true;
        ScrollView.RestrictWithinBounds(false, ScrollView.canMoveHorizontally, ScrollView.canMoveVertically);
    }
    #endregion
}


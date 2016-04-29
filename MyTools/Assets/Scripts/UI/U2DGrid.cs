using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class U2DGrid : MonoBehaviour
{
    public Arrangement arrangement = Arrangement.Horizontal;
    public int cellWidth = 200;
    public int cellHeight = 200;
    public UIWidget.Pivot pivot = UIWidget.Pivot.TopLeft;
    public bool isChildOnCenter;
    public int springStrength = 20;
    public int offsetWidthCount = 1;
    public int offsetHeightCount = 1;
    public bool isLoop;
    public bool isInvalidateBounds;
    public float moveDeltaOffset = 0.01f;
    public bool isResetScrollView;
    public bool isAsyncFocus;
    public int focusIndex;

    public enum Arrangement
    {
        Horizontal,
        Vertical,
        MatrixHorizontal,
        MatrixVertical,
    }

    private UIPanel mPanel;
    public UIPanel Panel
    {
        get
        {
            if (mPanel == null)
                mPanel = NGUITools.FindInParents<UIPanel>(gameObject);
            return mPanel;
        }
    }

    private UIScrollView mScroolView;
    public UIScrollView ScrollView
    {
        get
        {
            if (mScroolView == null)
                mScroolView = Panel.GetComponent<UIScrollView>();
            return mScroolView;
        }
    }

    private UICenterOnChild uiCenterOnChild;
    public UICenterOnChild UICenterOnChild
    {
        get
        {
            if (uiCenterOnChild == null)
                uiCenterOnChild = GetComponent<UICenterOnChild>();
            return uiCenterOnChild;
        }
    }

    private UIScrollBar mScrollBar;
    public UIScrollBar ScrollBar
    {
        get
        {
            if(mScrollBar == null)
            {
                switch(arrangement)
                {
                    case Arrangement.Horizontal:
                    case Arrangement.MatrixHorizontal:
                        if (ScrollView != null && ScrollView.horizontalScrollBar != null)
                            mScrollBar = (UIScrollBar)ScrollView.horizontalScrollBar;
                        break;
                    case Arrangement.Vertical:
                    case Arrangement.MatrixVertical:
                        if (ScrollView != null && ScrollView.horizontalScrollBar != null)
                            mScrollBar = (UIScrollBar)ScrollView.verticalScrollBar;
                        break;
                }
            }
            return mScrollBar;
        }
    }

    private List<Transform> mChildren;
    public List<Transform> Children
    {
        get
        {
            if (mChildren == null)
                mChildren = new List<Transform>();
            return mChildren;
        }
    }

    public Vector3[] Corners
    {
        get
        {
            Vector3[] corners = Panel.worldCorners;
            for (int i = 0; i < corners.Length; i++)
                corners[i] = transform.InverseTransformPoint(corners[i]);
            return corners;
        }
    }

    public Vector3 Center
    {
        get
        {
            Vector3[] corners = Corners;
            return Vector3.Lerp(corners[0], corners[2], 0.5f);
        }
    }

    public int TransChildCount { get { return Children.Count; } }
    public int PanelWidthCount { get; set; }
    public int PanelHeightCount { get; set; }
    public int PanelCellCount { get; set; }
    public int Line { get; set; }
    public int Column { get; set; }
    public int HorizonCount
    {
        get
        {
            if (arrangement == Arrangement.Horizontal)
                return ComputeWidthCount() - offsetWidthCount;
            else if (arrangement == Arrangement.Vertical)
                return ComputeHeightCount() - offsetHeightCount;
            return 0;
        }
    }

    public System.Action<int, Transform> onRefreshChild { get; set; }
    public System.Action<int, Transform> onHorizonRefresh { get; set; }
    public int TotalCount { get; set; }

    private Transform childPrefab;

    private int mDragIndex;

    public int DragIndex
    {
        get
        {
            return mDragIndex;
        }
    }

    void Awake()
    {
        if (Panel != null) Panel.onClipMove = WrapContent;
        if (isChildOnCenter)
        {
            if (UICenterOnChild == null)
            {
                uiCenterOnChild = gameObject.AddComponent<UICenterOnChild>();
                uiCenterOnChild.enabled = pivot != UIWidget.Pivot.Center;
                uiCenterOnChild.moveDeltaOffset = moveDeltaOffset;
                if(isInvalidateBounds) 
                    uiCenterOnChild.onFinished = InvalidateBounds;
            }
        }
        else if (UICenterOnChild != null)
        {
            Destroy(uiCenterOnChild);
            uiCenterOnChild = null;
        }
    }

    void Update()
    {
        if (isResetScrollView)
        {
            this.InitializeScrollView();
            this.isResetScrollView = false;
        }
        if (isAsyncFocus)
        {
            Focus(focusIndex);
            isAsyncFocus = false;
        }
    }

    public void Initialize(int totalCount, Transform prefab, UIWidget.Pivot pivot)
    {
        this.pivot = pivot;
        this.Initialize(totalCount, prefab);
    }

    public virtual void Initialize(int totalCount, Transform prefab)
    {
        this.TotalCount = totalCount;
        this.childPrefab = prefab;
        this.PanelWidthCount = ComputeWidthCount();
        this.PanelHeightCount = ComputeHeightCount();
        this.PanelCellCount = this.PanelWidthCount * this.PanelHeightCount;
        this.Column = ComputeColumn();
        this.Line = ComputeLine();
        this.InitializeCells(this.TotalCount > this.PanelCellCount ? this.PanelCellCount : this.TotalCount, this.childPrefab);
        this.isResetScrollView = true;
    }

    public void InitializeCells(int cellsCount, Transform cellPrefab)
    {
        if (cellPrefab != null)
        {
            int x = 0, y = 0;
            mChildren = null;
            mChildren = transform.SetChildrenGet<Transform>(cellsCount, cellPrefab, (idx, cell) => 
            {
                if (cell != null)
                {
                    cell.localPosition = new Vector3(cellWidth * x, -cellHeight * y, cell.localPosition.z);
                    if (++x >= PanelWidthCount)
                    {
                        x = 0;
                        y++;
                    }
                    UpdateChild(cell, idx);
                }
            });
        }
    }

    public void InitializeCellsPosition()
    {
        int x = 0, y = 0;
        Children.ForEach((value) =>
        {
            if (value != null)
            {
                value.localPosition = new Vector3(cellWidth * x, -cellHeight * y, value.localPosition.z);
                if (++x >= PanelWidthCount)
                {
                    x = 0;
                    y++;
                }
            }
        });
    }

    public void InitializeScrollView()
    {
        if (ScrollView != null)
        {
            ScrollView.contentPivot = pivot;
            if (arrangement == Arrangement.Horizontal || arrangement == Arrangement.MatrixHorizontal)
                ScrollView.movement = UIScrollView.Movement.Horizontal;
            else
                ScrollView.movement = UIScrollView.Movement.Vertical;
            ScrollView.ResetPosition();
        }
    }

    public int ComputeWidthCount()
    {
        switch (arrangement)
        {
            case Arrangement.Vertical:
                return 1;
            default:
                Vector3[] corners = Corners;
                return ((int)Mathf.Abs(corners[2].x - corners[0].x) / cellWidth) + offsetWidthCount;
        }

    }

    public int ComputeHeightCount()
    {
        switch (arrangement)
        {
            case Arrangement.Horizontal:
                return 1;
            default:
                Vector3[] corners = Corners;
                return ((int)Mathf.Abs(corners[2].y - corners[0].y) / cellHeight) + offsetHeightCount;
        }
    }

    public int ComputeLine()
    {
        switch (arrangement)
        {
            case Arrangement.Horizontal:
                return 1;
            case Arrangement.Vertical:
                return TotalCount;
            case Arrangement.MatrixHorizontal:
                return PanelHeightCount;
            case Arrangement.MatrixVertical:
                return TotalCount / PanelWidthCount + (TotalCount % PanelWidthCount > 0 ? 1 : 0);
            default:
                return 0;
        }
    }

    public int ComputeColumn()
    {
        switch (arrangement)
        {
            case Arrangement.Horizontal:
                return TotalCount;
            case Arrangement.Vertical:
                return 1;
            case Arrangement.MatrixHorizontal:
                return TotalCount / PanelHeightCount + (TotalCount % PanelHeightCount > 0 ? 1 : 0);
            case Arrangement.MatrixVertical:
                return PanelWidthCount;
            default:
                return 0;
        }
    }

    public virtual void SetChildren(int count, Transform child)
    {
        if (child != null)
        {
            mChildren = null;
            mChildren = transform.SetChildrenGet<Transform>(count, child);
        }
    }

    [ContextMenu("ResetChildrenPosition")]
    public void ResetToInitialize()
    {
        this.InitializeCellsPosition();
        this.InitializeScrollView();
    }

    public void WrapContent(UIPanel panel)
    {
        switch (arrangement)
        {
            case Arrangement.Horizontal:
            case Arrangement.MatrixHorizontal:
                WrapHorizontal();
                break;
            case Arrangement.Vertical:
            case Arrangement.MatrixVertical:
                WrapVertical();
                break;
        }
        if (ScrollView != null)
            ScrollView.centerOnChild = null;
    }

    private void WrapHorizontal()
    {
        float extents = cellWidth * PanelWidthCount * 0.5f;
        float ext2 = extents * 2f;
        Vector3[] corners = Corners;
        for (int i = 0, imax = Children.Count; i < imax; ++i)
        {
            Transform t = Children[i];
            if (t == null) continue;
            float distance = t.localPosition.x - Center.x;
            if (distance < -extents)
            {
                Vector3 pos = t.localPosition;
                pos.x += ext2;
                int realIndex = Mathf.RoundToInt(pos.x / cellWidth);
                if ((0 <= realIndex && realIndex < Column) || isLoop)
                {
                    t.localPosition = pos;
                    UpdateChild(t, i);
                }
            }
            else if (distance > extents)
            {
                Vector3 pos = t.localPosition;
                pos.x -= ext2;
                int realIndex = Mathf.RoundToInt(pos.x / cellWidth);
                if ((0 <= realIndex && realIndex < Column) || isLoop)
                {
                    t.localPosition = pos;
                    UpdateChild(t, i);
                }
            }
            if (t.localPosition.x < corners[2].x && t.localPosition.x > corners[0].x)
            {
                if (onHorizonRefresh != null) onHorizonRefresh(CalcRealIndex(t), t);
            }     
        }
    }

    private void WrapVertical()
    {
        float extents = cellHeight * PanelHeightCount * 0.5f;
        float ext2 = extents * 2f;
        Vector3[] corners = Corners;
        for (int i = 0, imax = Children.Count; i < imax; ++i)
        {
            Transform t = Children[i];
            if (t == null) continue;
            float distance = t.localPosition.y - Center.y;
            if (distance < -extents)
            {
                Vector3 pos = t.localPosition;
                pos.y += ext2;
                int realIndex = -Mathf.RoundToInt(pos.y / cellHeight);
                if (0 <= realIndex && realIndex < Line || isLoop)
                {
                    t.localPosition = pos;
                    UpdateChild(t, i);
                }
            }
            else if (distance > extents)
            {
                Vector3 pos = t.localPosition;
                pos.y -= ext2;
                int realIndex = -Mathf.RoundToInt(pos.y / cellHeight);
                if (0 <= realIndex && realIndex < Line || isLoop)
                {
                    t.localPosition = pos;
                    UpdateChild(t, i);
                }
            }
            if (t.localPosition.x > corners[2].y && t.localPosition.x < corners[0].y)
            {
                if (onHorizonRefresh != null) onHorizonRefresh(CalcRealIndex(t), t);
            }
        }
    }

    protected virtual void UpdateChild(Transform child, int index)
    {
        int realIndex = CalcRealIndex(child);
        child.name = childPrefab.name + realIndex;
        child.gameObject.SetActive(realIndex < TotalCount);
        if (onRefreshChild != null && child.gameObject.activeSelf)
            onRefreshChild(realIndex, child);
    }

    public int CalcRealIndex(Transform trans)
    {
        if (trans != null)
            return CalcRealIndex(trans.localPosition);
        return -1;
    }

    public int CalcRealIndex(Vector3 position)
    {
        switch (arrangement)
        {
            case Arrangement.Horizontal:
                return CalcHorizontalIndex(position.x);
            case Arrangement.Vertical:
                return CalcVerticalIndex(position.y);
            case Arrangement.MatrixHorizontal:
                return Line * CalcHorizontalIndex(position.x) + CalcVerticalIndex(position.y);
            case Arrangement.MatrixVertical:
                return Column * CalcVerticalIndex(position.y) + CalcHorizontalIndex(position.x);
            default:
                return 0;
        }
    }

    private int CalcVerticalIndex(float positionY)
    {
        int cacheIndex = 0, line = Line;
        cacheIndex = Mathf.RoundToInt(positionY / cellHeight);
        cacheIndex = cacheIndex % line;
        cacheIndex = cacheIndex < 0 ? Mathf.Abs(cacheIndex) : (cacheIndex > 0 ? line - cacheIndex : 0);
        return cacheIndex;
    }

    private int CalcHorizontalIndex(float positionX)
    {
        int cacheIndex = 0;
        cacheIndex = Mathf.RoundToInt(positionX / cellWidth);
        cacheIndex = cacheIndex % Column;
        cacheIndex = cacheIndex < 0 ? cacheIndex + Column : cacheIndex;
        return cacheIndex;
    }

    public void FocusOn(int index)
    {
        switch (arrangement)
        {
            case Arrangement.MatrixHorizontal:
            case Arrangement.Horizontal:
                index = index / Line;
                int onPanelWidthCount = PanelWidthCount - offsetWidthCount;
                index = index + onPanelWidthCount > Column ? Column - onPanelWidthCount : index;
                break;
            case Arrangement.MatrixVertical:
            case Arrangement.Vertical:
                index = index / Column;
                int onPanelHeightCount = PanelHeightCount - offsetHeightCount;
                index = index + onPanelHeightCount > Line ? Line - onPanelHeightCount : index;
                break;
        }
        focusIndex = index;
        isAsyncFocus = true;
        //Focus(index);
    }

    public void Focus(int index)
    {
        if (Children.Count > HorizonCount)
        {
            Vector3 focusPos = Vector3.zero;
            switch (arrangement)
            {
                case Arrangement.MatrixHorizontal:
                case Arrangement.Horizontal:
                    focusPos = ScrollView.transform.localPosition - new Vector3(cellWidth * (index - (CalcRealIndex(MinPosTransform) / Line)), 0, 0);
                    break;
                case Arrangement.MatrixVertical:
                case Arrangement.Vertical:
                    focusPos = ScrollView.transform.localPosition + new Vector3(0, cellHeight * (index - (CalcRealIndex(MaxPosTransform) / Column)), 0);
                    break;
            }
            MoveTo(focusPos, springStrength);
        }
    }

    public void Forward()
    {
        if (arrangement == Arrangement.Horizontal)
        {
            MoveTo(ScrollView.transform.localPosition - new Vector3(cellWidth, 0, 0), springStrength);
        }
        else if(arrangement == Arrangement.Vertical)
        {
            MoveTo(ScrollView.transform.localPosition + new Vector3(0, cellHeight, 0), springStrength);
        }

    }



    public void MoveTo(Vector3 target, float stength)
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

    public Transform MinPosTransform
    {
        get
        {
            return mChildren.Find((x) =>
            {
                switch (arrangement)
                {
                    case Arrangement.Horizontal:
                    case Arrangement.MatrixHorizontal:
                        float offsetX = x.localPosition.x - Corners[0].x;
                        return offsetX >= 0 && offsetX <= cellWidth;
                    case Arrangement.Vertical:
                    case Arrangement.MatrixVertical:
                        float offsetY = x.localPosition.y - Corners[0].y;
                        return offsetY >= 0 && offsetY <= cellHeight;
                }
                return false;
            });
        }
    }

    public Transform MaxPosTransform
    {
        get
        {
            return mChildren.Find((x) =>
            {
                switch (arrangement)
                {
                    case Arrangement.Horizontal:
                    case Arrangement.MatrixHorizontal:
                        float offsetX = Corners[2].x - x.localPosition.x;
                        return offsetX >= 0 && offsetX <= cellWidth;
                    case Arrangement.Vertical:
                    case Arrangement.MatrixVertical:
                        float offsetY = Corners[2].y - x.localPosition.y;
                        return offsetY >= 0 && offsetY <= cellHeight;
                }
                return false;
            });
        }
    }


    public void DragLeft(int span = 1)
    {
        span = span == 0 ? 1 : span;
        mDragIndex = CalcRealIndex(arrangement == Arrangement.Horizontal ? MinPosTransform : MaxPosTransform);
        if (mDragIndex >= 0)
        {
            if (mDragIndex - span <= -span)
                span = 1;
            else if (mDragIndex - span < 0)
                span = mDragIndex;
            mDragIndex -= span;
        }
        Focus(mDragIndex);
    }

    public void DragRight(int span = 1)
    {
        span = span == 0 ? 1 : span;
        mDragIndex = CalcRealIndex(arrangement == Arrangement.Horizontal ? MinPosTransform : MaxPosTransform);
        mDragIndex = mDragIndex < 0 ? 0 : mDragIndex;
        if (mDragIndex < TotalCount - HorizonCount + 1)
        {
            if (mDragIndex + HorizonCount >= TotalCount)
                span = 1;
            else
            {
                int offset = Mathf.Abs(TotalCount - (mDragIndex + span));
                if (offset < HorizonCount)
                    span = offset;
            }
            mDragIndex += span;
        }
        Focus(mDragIndex);
    }
}

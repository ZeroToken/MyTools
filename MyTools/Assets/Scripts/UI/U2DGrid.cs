using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class U2DGrid : MonoBehaviour
{
    public UIGrid.Sorting sorting;
    public Arrangement arrangement = Arrangement.Horizontal;
    public int maxPerLine;
    public int cellWidth = 200;
    public int cellHeight = 200;
    public UIWidget.Pivot pivot = UIWidget.Pivot.TopLeft;
    public bool isChildOnCenter;
    public int springStrength = 20;
    public bool isOffsetCount;

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

    public int TransChildCount
    {
        get
        {
            return transform.childCount;
        }
    }

    public int WidthCount
    {
        get
        {
            int width = (int)Mathf.Abs(Corners[2].x - Corners[0].x);
            if (isOffsetCount)
                return width / cellWidth + (width % cellWidth > 0 ? 1 : 0);
            return width / cellWidth;
        }
    }

    public int HeightCount
    {
        get
        {
            int height = (int)Mathf.Abs(Corners[2].y - Corners[0].y);
            if (isOffsetCount)
                return height / cellHeight + (height % cellHeight > 0 ? 1 : 0);
            return height / cellHeight;
        }
    }

    public int HorizonCount
    {
        get
        {
            if (arrangement == Arrangement.Horizontal)
                return WidthCount;
            else if (arrangement == Arrangement.Vertical)
                return HeightCount;
            return 0;
        }
    }

    private System.Action<int, Transform> mOnRefreshChild;
    public System.Action<int, Transform> onRefreshChild
    {
        get
        {
            return mOnRefreshChild;
        }
        set
        {
            mOnRefreshChild = value;
        }
    }

    public System.Action<int, Transform> onHorizonRefresh;

    private int mTotalCount;
    public int TotalCount
    {
        get
        {
            return mTotalCount;
        }
    }

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
        if (TransChildCount > 0)
        {
            for (int i = 0; i < TransChildCount; i++)
                Children.Add(transform.GetChild(i));
            this.mTotalCount = TransChildCount;
            Sorting();
            ResetChildrenPosition();
        }
        if (Panel != null) Panel.onClipMove = WrapContent;
        if (isChildOnCenter)
        {
            if (UICenterOnChild == null)
            {
                uiCenterOnChild = gameObject.AddComponent<UICenterOnChild>();
                uiCenterOnChild.enabled = pivot != UIWidget.Pivot.Center;
            }
        }
        else if (UICenterOnChild != null)
        {
            Destroy(uiCenterOnChild);
            uiCenterOnChild = null;
        }
    }

    public virtual void Initialize(int totalCount, Transform prefab)
    {
        this.mTotalCount = totalCount;
        this.childPrefab = prefab;
        if (arrangement == Arrangement.Horizontal)
        {
            int widthCount = WidthCount;
            SetChildren(totalCount > widthCount + 1 ? widthCount + 1 : totalCount, prefab);
        }
        else if (arrangement == Arrangement.Vertical)
        {
            int heightCount = HeightCount;
            SetChildren(totalCount > heightCount + 1 ? heightCount + 1 : totalCount, prefab);
        }
        else
        {
            SetChildren(totalCount, prefab);
        }
        Sorting();
        ResetChildrenPosition();
    }

    private void Sorting()
    {
        if (sorting == UIGrid.Sorting.Alphabetic)
            mChildren.Sort(UIGrid.SortByName);
        else if (sorting == UIGrid.Sorting.Horizontal)
            mChildren.Sort(UIGrid.SortHorizontal);
        else if (sorting == UIGrid.Sorting.Vertical)
            mChildren.Sort(UIGrid.SortVertical);
    }

    public virtual void SetChildren(int count, Transform child)
    {
        if (child != null)
        {
            mChildren = null;
            mChildren = transform.SetChildrenGet(count, child, onRefreshChild);
        }
    }
    [ContextMenu("ResetChildrenPosition")]
    public void ResetChildrenPosition()
    {
        Transform child = null;
        int x = 0, y = 0;
        int tempMaxPerLine = 0;
        for (int i = 0; i < TransChildCount; i++)
        {
            child = mChildren[i];
            if (child != null)
            {
                if (arrangement == Arrangement.Horizontal)
                {
                    tempMaxPerLine = 0;
                    child.localPosition = new Vector3(cellWidth * x, -cellHeight * y, child.localPosition.z);
                }
                else if (arrangement == Arrangement.Vertical)
                {
                    tempMaxPerLine = 0;
                    child.localPosition = new Vector3(cellWidth * y, -cellHeight * x, child.localPosition.z);
                }
                else if (arrangement == Arrangement.MatrixVertical)
                {
                    tempMaxPerLine = maxPerLine;
                    child.localPosition = new Vector3(cellWidth * x, -cellHeight * y, child.localPosition.z);
                }
                else if (arrangement == Arrangement.MatrixHorizontal)
                {
                    tempMaxPerLine = maxPerLine;
                    child.localPosition = new Vector3(cellWidth * y, -cellHeight * x, child.localPosition.z);
                }
            }
            if (++x >= tempMaxPerLine && tempMaxPerLine > 0)
            {
                x = 0;
                ++y;
            }
            child = null;
        }
        if (ScrollView != null)
        {
            ScrollView.contentPivot = pivot;
            if (arrangement == Arrangement.Horizontal || arrangement == Arrangement.MatrixHorizontal)
                ScrollView.movement = UIScrollView.Movement.Horizontal;
            else
                ScrollView.movement = UIScrollView.Movement.Vertical;
            ScrollView.ResetPosition();
        }
        if (isChildOnCenter && uiCenterOnChild != null && pivot != UIWidget.Pivot.Center) uiCenterOnChild.Recenter();
    }


    public void WrapContent(UIPanel panel)
    {
        if (arrangement == Arrangement.Horizontal)
        {
            WrapHorizontal();
        }
        else if (arrangement == Arrangement.Vertical)
        {
            WrapVertical();
        }
        else
        {
            WrapMatrix();
        }
    }

    private void WrapHorizontal()
    {
        float extents = cellWidth * Children.Count * 0.5f;
        Vector3 center = Vector3.Lerp(Corners[0], Corners[2], 0.5f);
        float ext2 = extents * 2f;
        if (mChildren.Count > 0)
        {
            for (int i = 0, imax = mChildren.Count; i < imax; ++i)
            {
                Transform t = mChildren[i];
                if (t == null) continue;
                float distance = t.localPosition.x - center.x;
                if (distance < -extents)
                {
                    Vector3 pos = t.localPosition;
                    pos.x += ext2;
                    int realIndex = Mathf.RoundToInt(pos.x / cellWidth);
                    if (0 <= realIndex && realIndex <= mTotalCount - 1)
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
                    if (0 <= realIndex && realIndex <= mTotalCount - 1)
                    {
                        t.localPosition = pos;
                        UpdateChild(t, i);
                    }
                }

                if (t.localPosition.x < Corners[2].x && t.localPosition.x > Corners[0].x)
                {
                    if (onHorizonRefresh != null) onHorizonRefresh(CalcRealIndex(t), t);
                }

            }
        }
    }

    private void WrapVertical()
    {
        float extents = cellHeight * TransChildCount * 0.5f;
        float ext2 = extents * 2f;
        Vector3 center = Vector3.Lerp(Corners[0], Corners[2], 0.5f);
        for (int i = 0, imax = TransChildCount; i < imax; ++i)
        {
            Transform t = Children[i];
            if (t == null) continue;
            float distance = t.localPosition.y - center.y;
            if (distance < -extents)
            {
                Vector3 pos = t.localPosition;
                pos.y += ext2;
                int realIndex = -Mathf.RoundToInt(pos.y / cellHeight);
                //Debug.Log(realIndex);
                if (0 <= realIndex && realIndex <= mTotalCount - 1)
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
                if (0 <= realIndex && realIndex <= mTotalCount - 1)
                {
                    t.localPosition = pos;
                    UpdateChild(t, i);
                }
            }
        }
    }

    private void WrapMatrix()
    {
        Vector3 center = Vector3.Lerp(Corners[0], Corners[2], 0.5f);
        for (int i = 0; i < TransChildCount; i++)
        {
            Transform child = Children[i];
            if (child != null)
            {
                float min = 0, max = 0, distance = 0;
                if (arrangement == Arrangement.MatrixHorizontal)
                {
                    min = Corners[0].x - cellWidth;
                    max = Corners[2].x + cellWidth;
                    distance = child.localPosition.x - center.x;
                    distance += mPanel.clipOffset.x - transform.localPosition.x;
                }
                else if (arrangement == Arrangement.MatrixVertical)
                {
                    min = Corners[0].y - cellHeight;
                    max = Corners[2].y + cellHeight;
                    distance = child.localPosition.y - center.y;
                    distance += Panel.clipOffset.y - transform.localPosition.y;
                }
                if (!UICamera.IsPressed(child.gameObject))
                {
                    child.gameObject.SetActive(distance > min && distance < max);
                }
            }
        }
    }

    protected virtual void UpdateChild(Transform child, int index)
    {
        int realIndex = CalcRealIndex(child);
        child.name = childPrefab.name + realIndex;
        if (onRefreshChild != null) onRefreshChild(realIndex, child);
    }

    public int CalcRealIndex(Transform trans)
    {
        if (trans != null)
        {
            return (arrangement == Arrangement.Vertical) ?
               Mathf.Abs(Mathf.RoundToInt(trans.localPosition.y / cellHeight)) :
               Mathf.Abs(Mathf.RoundToInt(trans.localPosition.x / cellWidth));
        }
        return -1;
    }

    public void FocusOn(int index)
    {
        if (TransChildCount > HorizonCount)
        {
            Vector3 focusPos = Vector3.zero;
            if (arrangement == Arrangement.Horizontal)
            {
                int minRealIndex = CalcRealIndex(MinPosTransform);
                focusPos = ScrollView.transform.localPosition - new Vector3(cellWidth * (index - minRealIndex), 0, 0);
            }
            else if (arrangement == Arrangement.Vertical)
            {
                int minRealIndex = CalcRealIndex(MaxPosTransform);
                focusPos = ScrollView.transform.localPosition + new Vector3(0, cellHeight * (index - minRealIndex), 0);
            }
            MoveTo(focusPos, springStrength);
        }
    }

    public void FocusOnIndex(int index)
    {
        index = index + HorizonCount >= TotalCount ? TotalCount - HorizonCount : index;
        FocusOn(index);
    }

    public void MoveTo(Vector3 target, float stength)
    {
        SpringPanel springPanel = SpringPanel.Begin(ScrollView.gameObject, target, stength);
        ScrollView.InvalidateBounds();
        springPanel.onFinished = () =>
        {
            ScrollView.InvalidateBounds();
            ScrollView.restrictWithinPanel = true;
            ScrollView.RestrictWithinBounds(false, ScrollView.canMoveHorizontally, ScrollView.canMoveVertically);
        };
    }

    public Transform MinPosTransform
    {
        get
        {
            return mChildren.Find((x) =>
            {
                if (arrangement == Arrangement.Horizontal)
                {
                    float offsetX = x.localPosition.x - Corners[0].x;
                    return offsetX >= 0 && offsetX <= cellWidth;
                }
                else if (arrangement == Arrangement.Vertical)
                {
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
                if (arrangement == Arrangement.Horizontal)
                {
                    float offsetX = Corners[2].x - x.localPosition.x;
                    return offsetX >= 0 && offsetX <= cellWidth;
                }
                else if (arrangement == Arrangement.Vertical)
                {
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
        FocusOn(mDragIndex);
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
        FocusOn(mDragIndex);
    }
}

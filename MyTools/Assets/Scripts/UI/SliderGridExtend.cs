/*============================================================
                 Author Name: hongfeng.wu
                 Create Time: 2016-5-3 16:29:45
============================================================*/
using UnityEngine;
using System.Collections.Generic;

public class SliderGridExtend : MonoBehaviour 
{
    public SliderGrid grid;
    public Transform prefab;

    public void Initialize(int count, System.Action<int, Transform> onRefreshChild, bool reassigned = false)
    {
        if (grid != null)
        {
            if (grid.onRefreshChild == null || reassigned)
                grid.onRefreshChild = onRefreshChild;
            grid.Initialize(count, prefab);
        }
    }

    public void Initialize<T>(int count, System.Action<int, T> onRefreshChild, bool reassigned = false) where T : Behaviour
    {
        if (grid != null)
        {
            if (grid.onRefreshChild == null || reassigned) grid.onRefreshChild = (idx, child) =>
            {
                if (child != null)
                {
                    T component = child.GetComponent<T>();
                    if (component != null) onRefreshChild(idx, component);
                }
            };
            grid.Initialize(count, prefab);
        }
    }

    public void Initialize<T>(int count, List<object> data, bool reassigned = false) where T : Component
    {
        if (grid != null)
        {
            if (grid.onRefreshChild == null || reassigned) grid.onRefreshChild = (idx, child) =>
            {
                if (child != null)
                {
                    T t = child.GetComponent<T>();
                    if (t != null) t.gameObject.SendMessage("Initialize", data[idx]);
                }
            };
            grid.Initialize(count, prefab);
        }
    }

    public void ParsePivot(int count)
    {
        if (grid != null) grid.pivot = count > grid.HorizonCount ? UIWidget.Pivot.TopLeft : UIWidget.Pivot.Center;
    }
}

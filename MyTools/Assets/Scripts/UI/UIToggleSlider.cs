using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIToggleSlider : MonoBehaviour
{
    public UIGrid uiToggleGrid;
    public Transform uiTogglePrefab;
    public U2DGrid uiViewGrid;
    public Transform uiViewPrefab;
    public int viewCount;
    public System.Action<int, Transform> onViewRefresh;
    private List<UIToggle> uiToggles;


    void Start()
    {
        if (uiViewGrid != null)
        {
            uiViewGrid.onHorizonRefresh = OnHorizonRefresh;
            uiViewGrid.onRefreshChild = onViewRefresh;
            UIRefresh();
        }

    }

    void OnHorizonRefresh(int idx, Transform child)
    {
        if (uiToggles != null && uiToggles.Count > idx)
            uiToggles[idx].value = true;
    }

    protected void UIRefresh()
    {
        if (uiToggleGrid != null)
        {
            uiToggles = uiToggleGrid.transform.SetChildrenGet<UIToggle>(viewCount, uiTogglePrefab, (idx, child) =>
            {
                if (child != null)
                    child.startsActive = idx == 0;
            });
        }
        if (uiViewGrid != null)
        {
            uiViewGrid.Initialize(viewCount, uiViewPrefab);
        }
    }

    public void Initialize(int count, System.Action<int, Transform> onViewRefresh)
    {
        this.onViewRefresh = onViewRefresh;
        this.viewCount = count;
        UIRefresh();
    }
    public void Initialize(int count)
    {
        this.viewCount = count;
        UIRefresh();
    }

    public static UIToggleSlider AttachTo(GameObject go, int count)
    {
        if (go != null)
        {
            UIToggleSlider slider = go.GetComponent<UIToggleSlider>();
            if (slider == null) slider = go.AddComponent<UIToggleSlider>();
            if (slider != null)
            {
                slider.Initialize(count);
            }
            return slider;
        }
        return null;
    }

    public static UIToggleSlider AttachTo(GameObject go, int count, System.Action<int, Transform> onViewRefresh)
    {
        if (go != null)
        {
            UIToggleSlider slider = go.GetComponent<UIToggleSlider>();
            if (slider == null) slider = go.AddComponent<UIToggleSlider>();
            if (slider != null)
            {
                slider.Initialize(count, onViewRefresh);
            }
            return slider;
        }
        return null;
    }
}

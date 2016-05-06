using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIToggleSlider : MonoBehaviour
{
    public UIGrid uiToggleGrid;
    public Transform uiTogglePrefab;
    public SliderGrid uiViewGrid;
    public Transform uiViewPrefab;
    public bool isLoop;
    public bool isAutoPlay;
    public float delay;
    public int viewCount;

    public System.Action<int, Transform> onViewRefresh;
    private List<UIToggle> toggles;

    void Start()
    {
        if (uiViewGrid != null)
        {
            uiViewGrid.isLoop = isLoop;
            uiViewGrid.onHorizonRefresh = OnHorizonRefresh;
            uiViewGrid.onRefreshChild = onViewRefresh;
            UIRefresh();
        }
    }

    private float tempDelay;
    void Update()
    {
        if (isAutoPlay)
        {
            if (uiViewGrid != null && !uiViewGrid.ScrollView.isDragging && delay > 0)
            {
                tempDelay += Time.deltaTime;
                if (tempDelay >= delay)
                {
                    RunSlider();
                    tempDelay = 0;
                }
            }
            else
                tempDelay = 0;

        }
    }


    void RunSlider()
    {
        if (enabled && uiViewGrid != null && !uiViewGrid.ScrollView.isDragging)
        {
            uiViewGrid.Forward();
        }
    }

    void OnHorizonRefresh(int idx, Transform child)
    {
        if (toggles != null && toggles.Count > idx)
            toggles[idx].value = true;
    }

    protected void UIRefresh()
    {
        if (uiToggleGrid != null)
        {
            toggles = uiToggleGrid.transform.SetChildren<UIToggle>(viewCount, uiTogglePrefab, (idx, child) =>
            {
                if (child != null)
                {
                    child.startsActive = idx == 0;
                }
            });
            uiToggleGrid.repositionNow = true;
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

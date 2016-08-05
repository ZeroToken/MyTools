using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ScrollView : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_rectTransfrom;

    [SerializeField]
    private ScrollRect m_ScrollRect;

    [SerializeField]
    private RectTransform m_content;

    [SerializeField]
    private GameObject cellPrefab;

    [SerializeField]
    private Arrangement arrangement;

    [SerializeField] [Range(0, 1000)]
    private int fixedCount = 1;

    [SerializeField] [Range(0, 960)] 
    private float cellWidth = 100;

    [SerializeField] [Range(0, 960)] 
    private float widthSpace = 0;

    [SerializeField] [Range(0, 640)] 
    private float cellHeight = 100;

    [SerializeField] [Range(0, 640)] 
    private float heightSpace = 0;

    [SerializeField] [Range(0, 5)]
    private int viewOffsetCount = 1;

    [SerializeField] [Range(0, 999)] 
    private int cellCount = 0;

    [SerializeField] [Range(0, 60)]
    private float moveDuration = 1;

    public ScrollRect scrollRect
    {
        get
        {
            if (m_ScrollRect == null)
            {
                m_ScrollRect = GetComponent<ScrollRect>();
            }
            return m_ScrollRect;
        }
    }

    public RectTransform content
    {
        get
        {
            if(m_content == null)
                m_content = scrollRect.content;
            return m_content;
        }
    }

    public RectTransform rectTransfrom
    {
        get
        {
            if(m_rectTransfrom == null)
                m_rectTransfrom = GetComponent<RectTransform>();
            return m_rectTransfrom;
        }
    }

    private List<ScrollCell> cells = new List<ScrollCell>();

    private Queue<ScrollCell> backupCells = new Queue<ScrollCell>();

    private int cacheCurrentIndex = -1;

    public System.Action<ScrollCell> onScrollCellRefresh;

    private MoveParam moveParam;

    private enum Arrangement
    {
        Horizontal,
        Vertical,
    }

    private void Awake()
    {
        scrollRect.onValueChanged.AddListener(OnScrollRectChange);
        scrollRect.horizontal = arrangement == Arrangement.Horizontal;
        scrollRect.vertical = arrangement == Arrangement.Vertical;
        Initialize(cellCount);
    }

    private void OnScrollRectChange(Vector2 vector)
    {
        switch (arrangement) 
        {
            case Arrangement.Horizontal:
                if (vector.x < 0.0f || vector.x > 1.0f)
                {
                    return;
                }
                break;
            case Arrangement.Vertical:
                if (vector.y >= 1.0f || vector.y < 0.0f) 
                {
                    return;
                }
                break;
        }
        OnUpdateCells();
    }

    private void OnUpdateCells()
    {
        int _currentIndex = GetCurrentIndex();
        if(cacheCurrentIndex == _currentIndex) return;
        cacheCurrentIndex = _currentIndex;
        int _starIndex = _currentIndex * fixedCount;
        int _endIndex = (_currentIndex + GetViewCount()) * fixedCount;
        for (int i = cells.Count - 1; i >= 0; i--) 
        {
            if (cells[i].index < _starIndex || cells[i].index >= _endIndex) 
            {
                backupCells.Enqueue (cells[i]);
                cells[i].index = -1;
                cells.Remove (cells[i]);
            }
        }
        for(int i = _starIndex; i < _endIndex; i++)
        {
            if(i >= cellCount || cells.Find(x => x.index == i) != null)
                continue;
            cells.Add(InstantiateCell(i));
        }
    }

    public void Initialize(int count)
    {
        this.Clear();
        ResetContentSizeDelta();
        int _viewCount = Mathf.Min(GetViewCount(), count) * fixedCount;
        for(int i = 0; i < _viewCount; i++)
        {
            cells.Add(InstantiateCell(i));
        }
        foreach(var _cell in backupCells)
        {
            _cell.gameObject.SetActive(false);
        }
    }

    public void Clear()
    {
        for(int i = cells.Count - 1; i >= 0; i--)
        {
            backupCells.Enqueue(cells[i]);
        }
        cells.Clear();
    }

    public void ResetContentSizeDelta()
    {
        Vector2 _size = Vector2.zero;
        switch(arrangement)
        {
            case Arrangement.Horizontal:
                {
                    int _column = Mathf.CeilToInt((float)cellCount / fixedCount);
                    float x = cellWidth * _column + widthSpace * (_column - 1);
                    float y = cellHeight * fixedCount + heightSpace * (fixedCount - 1);
                    _size = new Vector2(x, y);
                }
                break;
            case Arrangement.Vertical:
                {
                    int _line = Mathf.CeilToInt((float)cellCount / fixedCount);
                    float x = cellWidth * fixedCount + widthSpace * (fixedCount - 1);
                    float y = cellHeight * _line + heightSpace * (_line - 1);
                    _size = new Vector2(x, y);
                }
                break;
        }
        if(content) content.sizeDelta = _size;
    }
   
    public Vector3 GetCellPosiotion(int index)
    {
        float x = 0, y = 0, z = 0;
        switch(arrangement)
        {
            case Arrangement.Horizontal:
                x = (index / fixedCount) * (cellWidth + widthSpace);
                y = -(index % fixedCount) * (cellHeight + heightSpace);
                break;
            case Arrangement.Vertical: 
                x = (index % fixedCount) * (cellWidth + widthSpace);
                y = -(index / fixedCount) * (cellHeight + heightSpace);
                break;
        }
        return new Vector3(x, y, z);
    }

    public int GetCurrentIndex()
    {
        switch (arrangement) 
        {
            case Arrangement.Horizontal:
                return Mathf.FloorToInt(Mathf.Abs(content.anchoredPosition.x) / (cellWidth + widthSpace));
            case  Arrangement.Vertical:
                return Mathf.FloorToInt(Mathf.Abs(content.anchoredPosition.y) / (cellHeight + heightSpace));
        }
        return 0;
    }

    public int GetViewCount()
    {
        switch(arrangement)
        {
            case Arrangement.Horizontal:
                return Mathf.CeilToInt(rectTransfrom.sizeDelta.x / (cellWidth + widthSpace)) + viewOffsetCount;
            case Arrangement.Vertical:
                return Mathf.CeilToInt(rectTransfrom.sizeDelta.y / (cellHeight + heightSpace)) + viewOffsetCount;
            default:
                return 0;
                
        }
    }
        
    public ScrollCell InstantiateCell(int index)
    {
        if(backupCells.Count > 0)
        {
            ScrollCell _cell = backupCells.Dequeue();
            _cell.index = index;
            _cell.SetPosition(GetCellPosiotion(index));
            _cell.gameObject.name = "Cell" + index;
            if(onScrollCellRefresh != null)
                onScrollCellRefresh(_cell);
            return _cell;
        }
        else
        {
            return InstantiateCell(cellPrefab, content, GetCellPosiotion(index), index);
        }
    }

    public ScrollCell InstantiateCell(GameObject prefab, Transform parent, Vector3 localPosition, int index)
    {
        if(prefab == null) return null;
        GameObject childGo = GameObject.Instantiate (prefab) as GameObject;
        childGo.name = "Cell" + index;
        childGo.layer = content.gameObject.layer;
        childGo.transform.SetParent (content, false);
        childGo.transform.localPosition = localPosition;
        childGo.transform.localScale = Vector2.one;
        childGo.SetActive(true);
        ScrollCell _cell = new ScrollCell(childGo, index);
        if(onScrollCellRefresh != null)
            onScrollCellRefresh(_cell);
        return _cell;
    }

    public void MoveTo(int index)
    {
        index = Mathf.Min(index, cellCount);
        float _value = (float)index / cellCount;
        Vector3 target = Vector3.zero;
        switch(arrangement)
        {
            case Arrangement.Horizontal:
                target = new Vector3(_value, 0, 0);
                break;
            case Arrangement.Vertical:
                target = new Vector3(0, _value, 0);
                break;
        }
        if(moveParam == null)
            moveParam = new MoveParam(scrollRect);  
        moveParam.Start(target, moveDuration);
    }

    private void FixedUpdate()
    {
        if(moveParam != null) moveParam.Update();
    }

    private class MoveParam
    {
        private ScrollRect target;
        private Vector3 endPosition;
        private float duration;
        private bool isUpdate;
        private float startTime;       
        private float time = 0;
        public MoveParam() { }
        public MoveParam(ScrollRect _target) { this.target = _target; }
        public void Update()
        {
            if(isUpdate)
            {
                if(time <= duration)
                {
                    time += Time.deltaTime;
                    target.normalizedPosition = Vector3.Lerp(target.normalizedPosition, endPosition, time / duration);
                }
                else
                {
                    isUpdate = false;
                }
            }
        }
        public void Start(Vector3 _end, float _duration)
        {
            Debug.Log("_end: " + _end);
            this.endPosition = _end;
            this.duration = _duration;
            this.startTime = Time.time;
            this.isUpdate = true;
            this.time = 0;
        }
    }
}

public class ScrollCell
{
    public GameObject gameObject;

    public int index;

    public ScrollCell(GameObject _cell, int _index)
    {
        this.gameObject = _cell;
        this.index = _index;
    }

    public T GetComponent<T>() where T : Component
    {
        if(gameObject)
            return gameObject.GetComponent<T>();
        else
            return null;
    }

    public T GetComponentInChildren<T>() where T : Component
    {
        if(gameObject)
            return gameObject.GetComponentInChildren<T>();
        else
            return null;
    }

    public void SetPosition(Vector3 pos)
    {
        if(gameObject) gameObject.transform.localPosition = pos;
    }
     
}

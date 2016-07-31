using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ScrollView : MonoBehaviour
{
    private ScrollRect mScrollRect;

    public ScrollRect scrollRect
    {
        get
        {
            if (mScrollRect == null)
            {
                mScrollRect = GetComponent<ScrollRect>();
            }
            return mScrollRect;
        }
    }

    private void Awake()
    {
        scrollRect.onValueChanged.AddListener(OnScrollRectChange);
    }

    private void OnScrollRectChange(Vector2 vector)
    {
        Debug.Log(vector);
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScrollViewTest : MonoBehaviour {

    public ScrollView scrollView;

    public int index = 0;

    public void Awake()
    {
        scrollView.onScrollCellRefresh = (cell) =>
        {
            cell.GetComponentInChildren<Text>().text = cell.index.ToString();
        };
    }

    public void OnClick()
    {
        scrollView.MoveTo(index);
    }
}

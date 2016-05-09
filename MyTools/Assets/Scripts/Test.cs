using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

    public SliderGrid grid;
    public Transform prefab;

    public GameObject btn;
    public GridSlider sliderGrid;
    public int index;
    public int count = 100;
    void Awake()
    {
        //grid.Initialize(100, prefab);



        UIEventListener.Get(btn).onClick = (go) =>
        {
            sliderGrid.FocusOnCellWithLimit(index);
        };
    }

    void Start()
    {
        sliderGrid.Initialize(count, (idx, cell) =>
        {
            cell.GetComponentInChildren<UILabel>().text = idx.ToString();
        });
    }

}

using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

    [UIAttribute("Test")]
    private GameObject test;
    [UIAttribute("VVV")]
    VVV vvv;

    void Awake()
    {
        UIAttribute.Init(this);
        Debug.Log(test.name);
        Debug.Log(vvv.name);
    }

}

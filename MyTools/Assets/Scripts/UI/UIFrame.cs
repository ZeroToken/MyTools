using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIFrame : MonoBehaviour 
{
    private Dictionary<string, Transform> allUI;
    void Awake()
    {
        allUI = transform.AllChildrenIgnore<UIFrame>();
    }

    public T Get<T>(string uiName) where T : Component
    {
        if (allUI.ContainsKey(uiName))
        {
            return allUI[uiName].GetComponent<T>();
        }
        return null;
    }
}

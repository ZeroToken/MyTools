using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIFrame : MonoBehaviour 
{
    private Dictionary<string, Transform> allUI;
    void Awake()
    {
        allUI = transform.AllChildrenIgnore<UIFrame>();
    }

    public Transform GetTransform(string uiName)
    {
        if (allUI.ContainsKey(uiName) && allUI[uiName] != null)
            return allUI[uiName].transform;
        else
            Debug.Log("Not found UI: " + uiName);
        return null;
    }

    public Component GetComponent(string uiName, Type type)
    {
        var ui = GetTransform(uiName);
        if (ui != null) return ui.GetComponent(type);
        return null;
    }

    public GameObject GetGameObject(string uiName)
    {
        var ui = GetTransform(uiName);
        if (ui != null) return ui.gameObject;
        return null;
    }

    public T Get<T>(string uiName) where T : UnityEngine.Object
    {
        var ui = GetTransform(uiName);
        if(ui != null) return ui.GetComponent<T>();
        return default(T);
    }
}

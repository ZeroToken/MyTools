using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIFrame : MonoBehaviour 
{
    private Dictionary<string, Transform> allUI;
    void Awake()
    {
        allUI = GetAllChildrenIgnore(transform);
    }

    public Dictionary<string, Transform> GetAllChildrenIgnore(Transform parent)
    {
        Dictionary<string, Transform> children = new Dictionary<string, Transform>();
        foreach (Transform child in parent)
        {
            if (!children.ContainsKey(child.name))
            {
                children.Add(child.name, child);
                if (child.GetComponent<UIFrame>() == null)
                {
                    var c = GetAllChildrenIgnore(child);
                    foreach (var v in c)
                    {
                        children.Add(v.Key, v.Value);
                    }
                }
            }
        }
        return children;
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

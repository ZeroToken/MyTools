using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class TransformExtensions 
{
    public static List<Transform> GetAllChildren(this Transform transform)
    {
        List<Transform> children = new List<Transform>();
        foreach (Transform child in transform)
        {
            children.Add(child);
            children.AddRange(child.GetAllChildren());
        }
        return children;
    }

    public static List<Transform> GetAllChildrenIgnore<T>(this Transform transform) where T : Component
    {
        List<Transform> children = new List<Transform>();
        foreach (Transform child in transform)
        {
            children.Add(child);
            children.AddRange(child.GetAllChildrenIgnore<T>());
        }
        return children;
    }

    public static Dictionary<string, Transform> AllChildren(this Transform transform)
    {
        Dictionary<string, Transform> children = new Dictionary<string,Transform>();
        foreach (Transform child in transform)
        {
            children.Add(child.name, child);
            var c = child.AllChildren();
            foreach (var v in c)
            {
                children.Add(v.Key, v.Value);
            }
        }
        return children;
    }

    public static Dictionary<string, Transform> AllChildrenIgnore<T>(this Transform transform) where T : Component
    {
        Dictionary<string, Transform> children = new Dictionary<string, Transform>();
        foreach (Transform child in transform)
        {
            children.Add(child.name, child);
            if (child.GetComponent<T>() == null)
            {
                var c = child.AllChildren();
                foreach (var v in c)
                {
                    children.Add(v.Key, v.Value);
                }
            }
        }
        return children;
    }
}

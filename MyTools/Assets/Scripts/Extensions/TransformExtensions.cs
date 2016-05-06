using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class TransformExtensions 
{
    //public static List<Transform> GetAllChildren(this Transform transform)
    //{
    //    List<Transform> children = new List<Transform>();
    //    foreach (Transform child in transform)
    //    {
    //        children.Add(child);
    //        children.AddRange(child.GetAllChildren());
    //    }
    //    return children;
    //}
    //public static Dictionary<string, Transform> FindAllChildren(this Transform transform)
    //{
    //    Dictionary<string, Transform> children = new Dictionary<string,Transform>();
    //    foreach (Transform child in transform)
    //    {
    //        children.Add(child.name, child);
    //        var c = child.FindAllChildren();
    //        foreach (var v in c)
    //        {
    //            children.Add(v.Key, v.Value);
    //        }
    //    }
    //    return children;
    //}

    //public static List<Transform> FindAllChild(this Transform transform)
    //{
    //    return transform.FindChildren<Transform>();
    //}

    //public static List<T> FindChildren<T>(this Transform transform) where T:Component
    //{
    //    List<T> children = new List<T>();
    //    foreach (Transform child in transform)
    //    {
    //        var component = child.GetComponent<T>();
    //        if (component != null) children.Add(component);
    //    }
    //    return children;
    //}

    //public static void SetChildren<T>(this Transform transform, int count, Transform childPrefab, System.Action<int, T> onRefreshChild) where T : Component
    //{
    //    if (childPrefab != null)
    //    {
    //        childPrefab.gameObject.SetActive(true);
    //        transform.SetChildCount<T>(count, childPrefab, onRefreshChild);
    //        childPrefab.gameObject.SetActive(false);
    //    }
    //}

    //public static void SetChildren(this Transform transform, int count, Transform childPrefab)
    //{
    //    transform.SetChildren<Transform>(count, childPrefab, null);
    //}

    //public static List<T> SetChildrenGet<T>(this Transform transform, int count, Transform childPrefab) where T : Component
    //{
    //    transform.SetChildren(count, childPrefab);
    //    return transform.FindChildren<T>();
    //}

    //public static int ChildrenActiveCount(this Transform transform)
    //{
    //    int activeCount = 0;
    //    for (int i = 0; i < transform.childCount; i++)
    //    {
    //        if (transform.GetChild(i).gameObject.activeSelf)
    //            activeCount++;
    //    }
    //    return activeCount;
    //}

    public static void SetChildCount(this Transform transform, int count, Transform childPrefab, System.Action<int, Transform> onRefreshChild = null)
    {
        count = Mathf.Max(0, count);
        int childCount = transform.childCount;
        if (childCount == count)
            return;
        if (childCount > count)
        {
            for (int i = 0; i < childCount; i++)
            {
                if (i < count)
                    if (onRefreshChild != null) onRefreshChild(i, transform.GetChild(i));
                    else
                        GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
        else if (childPrefab != null)
        {
            for (int i = childCount; i < count; i++)
            {
                Transform newChildTrans = Object.Instantiate(childPrefab) as Transform;
                newChildTrans.parent = transform;
                newChildTrans.localPosition = childPrefab.localPosition;
                newChildTrans.localRotation = childPrefab.localRotation;
                newChildTrans.localScale = childPrefab.localScale;
                newChildTrans.name = childPrefab.name + i;
                newChildTrans.gameObject.SetActive(true);
                if (onRefreshChild != null) onRefreshChild(i, newChildTrans);
            }
        }
    }

    public static void SetChildCount<T>(this Transform transform, int count, Transform childPrefab, System.Action<int, T> onRefreshChild = null) where T : Component
    {
        SetChildCount(transform, count, childPrefab, (index, child) =>
        {
            if (onRefreshChild != null) onRefreshChild(index, child.GetComponent<T>());
        });
    }

    public static List<Transform> SetChildren(this Transform transform, int count, Transform childPrefab, System.Action<int, Transform> onRefreshChild = null)
    {
        List<Transform> children = new List<Transform>();
        transform.SetChildCount(count, childPrefab, (index, child) =>
        {
            children.Add(child);
            if (onRefreshChild != null)
                onRefreshChild(index, child);
        });
        return children;
    }

    public static List<T> SetChildren<T>(this Transform transform, int count, Transform childPrefab, System.Action<int, T> onRefreshChild = null) where T : Component
    {
        List<T> children = new List<T>();
        transform.SetChildCount<T>(count, childPrefab, (index, child) =>
        {
            children.Add(child);
            if (onRefreshChild != null)
                onRefreshChild(index, child);
        });
        return children;
    }
}

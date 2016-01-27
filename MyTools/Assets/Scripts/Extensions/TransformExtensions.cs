using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class TransformExtensions 
{
    /// <summary>
    /// 获取所有子物体，包含子物体下的所有子物体，并保存至List
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
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
    /// <summary>
    /// 获取所有子物体，包含子物体下的所有子物体,并保存至Dictionary
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static Dictionary<string, Transform> FindAllChildren(this Transform transform)
    {
        Dictionary<string, Transform> children = new Dictionary<string,Transform>();
        foreach (Transform child in transform)
        {
            children.Add(child.name, child);
            var c = child.FindAllChildren();
            foreach (var v in c)
            {
                children.Add(v.Key, v.Value);
            }
        }
        return children;
    }
    /// <summary>
    /// 获取当前Transform下的所有子物体
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static List<Transform> FindAllChild(this Transform transform)
    {
        return transform.FindChildren<Transform>();
    }

    public static List<T> FindChildren<T>(this Transform transform)
    {
        List<T> children = new List<T>();
        foreach (Transform child in transform)
        {
            var component = child.GetComponent<T>();
            if (component != null) children.Add(component);
        }
        return children;
    }


    public static void SetChildCount<T>(this Transform transform, int count, Transform childPrefab, System.Action<int, T> onRefreshChild = null)
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
                    onRefreshChild(i, transform.GetChild(i).GetComponent<T>());
                else
                    GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
        else if(childPrefab != null)
        {
            for (int i = childCount; i < count; i++)
            {
                Transform newChildTrans = Object.Instantiate(childPrefab) as Transform;
                newChildTrans.gameObject.name = childPrefab.gameObject.name;
                newChildTrans.parent = transform;
                newChildTrans.localPosition = childPrefab.localPosition;
                newChildTrans.localRotation = childPrefab.localRotation;
                newChildTrans.localScale = childPrefab.localScale;
                newChildTrans.name += i;
                if (onRefreshChild != null) onRefreshChild(i, newChildTrans.GetComponent<T>());
            }
        }
    }
    public static void SetChildren<T>(this Transform transform, int count, Transform childPrefab, System.Action<int, T> onRefreshChild)
    {
        if (childPrefab != null)
        {
            childPrefab.gameObject.SetActive(true);
            transform.SetChildCount<T>(count, childPrefab, onRefreshChild);
            childPrefab.gameObject.SetActive(false);
        }
    }

    public static void SetChildren(this Transform transform, int count, Transform childPrefab)
    {
        transform.SetChildren<Transform>(count, childPrefab, null);
    }

    public static List<T> SetChildrenGet<T>(this Transform transform, int count, Transform childPrefab)
    {
        transform.SetChildren(count, childPrefab);
        return transform.FindChildren<T>();
    }

    public static List<T> SetChildrenGet<T>(this Transform transform, int count, Transform childPrefab, System.Action<int, T> onRefreshChild)
    {
        transform.SetChildren<T>(count, childPrefab, onRefreshChild);
        return transform.FindChildren<T>();
    }

    public static int ChildrenActiveCount(this Transform transform)
    {
        int activeCount = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf)
                activeCount++;
        }
        return activeCount;
    }
}

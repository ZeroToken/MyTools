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
        List<Transform> children = new List<Transform>();
        foreach (Transform child in transform)
            children.Add(child);
        return children;
    }


    public static List<Transform> SetChildren(this Transform transform, int count, Transform childPrefab)
    {
        count = Mathf.Max(0, count);
        int childCount = transform.childCount;
        if (childCount == count)
            return transform.FindAllChild() ;
        if (childCount > count)
        {
            for (int i = childCount - 1; i >= count; i--)
                GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
            return transform.FindAllChild();
        }
        else if(childPrefab != null)
        {
            List<Transform> children = new List<Transform>();
            childPrefab.gameObject.SetActive(true);
            for (int i = childCount; i < count; i++)
            {
                Transform newChildTrans = Object.Instantiate(childPrefab) as Transform;
                newChildTrans.gameObject.name = childPrefab.gameObject.name;
                newChildTrans.parent = transform;
                newChildTrans.localPosition = childPrefab.localPosition;
                newChildTrans.localRotation = childPrefab.localRotation;
                newChildTrans.localScale = childPrefab.localScale;
                newChildTrans.name += i;
                children.Add(newChildTrans);
            }
            childPrefab.gameObject.SetActive(false);
            return children;
        }
        return new List<Transform>();
    }

    public static void SetChild(this Transform transform, int count, Transform child)
    {
        if (transform != null && child != null)
        {
            child.gameObject.SetActive(true);
            transform.SetChildren(count, child);
            child.gameObject.SetActive(false);
        }
    }

    public static List<Transform> SetChildGet(this Transform transform, int count, Transform child)
    {
        List<Transform> children = new List<Transform>();
        if (transform != null && child != null)
        {
            transform.SetChild(count, child);
            for (int i = 0; i < count; i++)
            {
                children.Add(transform.GetChild(i));
            }
        }
        return children;
    }

    public static List<Transform> SetChildGet(this Transform transform, int count, Transform child, System.Action<int, Transform> onRefreshChild)
    {
        List<Transform> children = new List<Transform>();
        if (transform != null && child != null)
        {
            transform.SetChild<Transform>(count, child, (idx, childTransform) =>
            {
                children.Add(transform.GetChild(idx));
                if (onRefreshChild != null) onRefreshChild(idx, childTransform);
            });
        }
        return children;
    }

    public static int GetActiveChildCount(this Transform transform)
    {
        int activeCount = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf)
                activeCount++;
        }
        return activeCount;
    }

    public static List<T> SetChild<T>(this Transform transform, int count, Transform child, System.Action<int, T> onRefreshChild) where T : Component
    {
        List<T> ts = new List<T>();
        if (transform != null && child != null)
        {
            child.gameObject.SetActive(true);
            transform.SetChildren(count, child);
            child.gameObject.SetActive(false);
            if (onRefreshChild != null)
            {
                T tchild = null;
                for (int i = 0; i < count; i++)
                {
                    child = transform.GetChild(i);
                    if (child != null)
                    {
                        tchild = child.GetComponent<T>();
                        if (tchild != null) ts.Add(tchild);
                    }
                    onRefreshChild(i, tchild);
                    tchild = null;
                }
            }
        }
        return ts;
    }
}

using UnityEngine;
using System.Collections;
using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class UIComponent : Attribute
{
    public UIComponent(string uiName)
    {
        Debug.Log(TypeId);
    }
}

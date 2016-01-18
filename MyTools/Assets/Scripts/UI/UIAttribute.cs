using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class UIAttribute : Attribute
{
    public UIAttribute(string uiName)
    {
        this.uiName = uiName;
    }

    private string uiName;

    public string UIName { get { return uiName; } }

    public static void Init(object target)
    {
        var targetGo = target as MonoBehaviour;
        if (targetGo != null)
        {
            UIFrame uiFrame = targetGo.GetComponent<UIFrame>();
            if (uiFrame == null) uiFrame = targetGo.gameObject.AddComponent<UIFrame>();
            if (uiFrame != null)
            {
                var targetFields = target.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                for (int i = 0; i < targetFields.Length; i++)
                {
                    var fieldAttrs = targetFields[i].GetCustomAttributes(typeof(UIAttribute), false);
                    if (fieldAttrs != null && fieldAttrs.Length > 0 && fieldAttrs[0] != null)
                    {
                        var uiFieldAttr = fieldAttrs[0] as UIAttribute;
                        if (uiFieldAttr != null)
                        {
                            var fieldType = targetFields[i].FieldType;
                            if (fieldType == typeof(Transform))
                                targetFields[i].SetValue(target, uiFrame.GetTransform(uiFieldAttr.UIName));
                            else if (fieldType == typeof(GameObject))
                                targetFields[i].SetValue(target, uiFrame.GetGameObject(uiFieldAttr.UIName));
                            else if (fieldType.IsSubclassOf(typeof(Component)))
                            {
                                Component comp = uiFrame.GetComponent(uiFieldAttr.UIName, fieldType);
                                if (comp == null) Debug.Log("UI: '" + uiFieldAttr.UIName + "' type of '" + fieldType + "' not found");
                                targetFields[i].SetValue(target, comp);
                            }
                        }
                    }
                }
            }
        }
    }
}

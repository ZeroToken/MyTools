using UnityEngine;
using LuaInterface;
using UnityEngine.UI;

public class VVV : MonoBehaviour {
    public Image image;

    public int index;

    public string imageName;

    //public Text lbText;
    // Use this for initialization
    void Start () {
        //Application.RegisterLogCallback ((condition, stackTrace, type) =>
        //{
        //    if (lbText != null) lbText.text += type.ToString() + ":" + condition + "\n";
        //    //Debug.l
        //    //Debug.Log(condition);
        //    //Debug.Log(stackTrace);
        //});
        //var v = LuaManager.Instance;
        //Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaa:" + v.Require("RequireTest")["aa"]);
        //(v.Require("RequireTest")["vvv"] as LuaFunction).Call();
        image.SetAtlas("SmallIcon");
    }

    [ContextMenu("SetImageByIndex")]
    public void SetImageByIndex()
    {
        image.SetSprite(index);
        image.SetNativeSize();
    }
    [ContextMenu("SetImageByName")]
    public void SetImageByName()
    {
        image.SetSprite(imageName);
        image.SetNativeSize();
    }
}

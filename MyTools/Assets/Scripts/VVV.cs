using UnityEngine;
using LuaInterface;
using UnityEngine.UI;

public class VVV : MonoBehaviour {

    public Text lbText;
	// Use this for initialization
	void Start () {
        Application.RegisterLogCallback ((condition, stackTrace, type) =>
        {
            if (lbText != null) lbText.text += type.ToString() + ":" + condition + "\n";
            //Debug.l
            //Debug.Log(condition);
            //Debug.Log(stackTrace);
        });
        var v = LuaManager.Instance;
        Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaa:" + v.Require("RequireTest")["aa"]);
        (v.Require("RequireTest")["vvv"] as LuaFunction).Call();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

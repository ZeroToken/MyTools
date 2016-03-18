using UnityEngine;
using System.Collections;
using LuaInterface;

public class VVV : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Application.RegisterLogCallback ((condition, stackTrace, type) =>
        {
            //Debug.l
            //Debug.Log(condition);
            //Debug.Log(stackTrace);
        });
        var v = LuaManager.Instance;
        Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaa:" + v.Require("RequireTest")["aa"]);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

/// <summary>
/// **** wangyel1 ****
/// 用GL制作外围的线和内部填充，不知道效率怎么样.
///GUI只针对num为6时，如果看不同num效果，请打开Scene3——FreeSet。
/// </summary>
using UnityEngine;
using System.Collections;

public class DrawLine_Test : MonoBehaviour
{
	public Color colorRe = Color.grey; //框内部颜色//
	public Color colorOut = Color.green;
	public Color colorGUI = Color.yellow;
	public int num = 6; //角的个数，6表示六角形，可设置为任意多边形//
	public string[] toBarStr = new string[]{"","",""};
	public string totalFont;
	private Material mat = null;
	private Vector3[] circlePositon; //圆周上的位置，也是角所能达到的最大位置//
	private Vector3[] maxClampPo; //限制角方向最大的值//
	private int selGridInt = 0; //GUI的返回值//
	//GUI中增加或减少数值的表格名称，需配合num变量来设置，这里num为6，数组由6个+和6个-组成//
	private string[] selStrings = new string[] {"+","+","+","+","+","+","--","--","--","--","--","--"};
	private string[] toolbarStrings = new string[] {"","","","","",""}; //显示剩余点数//
	private Vector3 circleCenter; //圆心位置//
	private Vector3[] point; //圆周上点的位置//
	private int[] randomAdd; //随机增量，并保存对应点的初始变化量//
	private float totalNum = 0; //剩余点数//
	private int toolBarInt = 0; //一下三个int变量GUI所用//
	private int toBar = 0;
	
	void Start ()
	{
		mat = new Material ("Shader \"Lines/Colored Blended\" {" +
            "SubShader { Pass { " +
            "    Blend SrcAlpha OneMinusSrcAlpha " +
            "    ZWrite Off Cull Off Fog { Mode Off } " +
            "    BindChannels {" +
            "      Bind \"vertex\", vertex Bind \"color\", color }" +
            "} } }");
		mat.hideFlags = HideFlags.HideAndDontSave;
		mat.shader.hideFlags = HideFlags.HideAndDontSave; 
		
		randomAdd = new int[num];
		circlePositon = new Vector3[num];
		point = new Vector3[num];
		circleCenter = new Vector3 (0.5f, 0.6f, 0);
		for (int i = 0; i< num; i++) { //确定圆周与圆周上的点，初始化一个随机图表//
			float angle = i * 2 * 3.14159f / num;
			point [i] = new Vector3 (Mathf.Sin (angle), Mathf.Cos (angle), 0) / 4;
			randomAdd [i] = Random.Range (1, 10);
			totalNum += randomAdd [i] * 10;
			Vector3 addOrSub = randomAdd [i] * point [i] / 10;
			circlePositon [i] = circleCenter + addOrSub; 
		}
		totalNum = 600 - totalNum;
	}
	
	void Add (int addNum, string addOrSub)
	{ //点击递增递减//
		switch (addOrSub) {
		case "add":
			if (randomAdd [addNum] >= 10) {
				randomAdd [addNum] = 10;
				return;
			}
			randomAdd [addNum] += 1;
			circlePositon [addNum] += point [addNum] * 0.1f;
			totalNum -= 10;
			break;
		case "sub":
			if (randomAdd [addNum] <= 1) {
				randomAdd [addNum] = 1;
				return;
			}	
			randomAdd [addNum] -= 1;
			circlePositon [addNum] -= point [addNum] * 0.1f;
			totalNum += 10;
			break;
		}
	}
	
	void OnGUI ()
	{ //制作GUI//
		GUI.color = colorGUI;
		GUI.contentColor = Color.green;
		//增加减少点数//
		for (int t = 0; t<num; t++) {
			toolbarStrings [t] = t + "->" + (randomAdd [t]) * 10;
		}
		selGridInt = GUI.SelectionGrid (new Rect (Screen.width / 2 - 120, Screen.height - 80, 350, 60), selGridInt, selStrings, num);
		if (GUI.changed) {
			for (int g = 0; g<selStrings.Length; g++) {
				if (g < num && selGridInt == g) {
					Add (g, "add");
				}
				if (g >= num && selGridInt == g) {
					Add (g - num, "sub");
				}
			}
		}
		//名称显示和剩余点数//
		toBar = GUI.SelectionGrid (new Rect (Screen.width / 2 - 200, Screen.height - 115, 70, 95), toBar, toBarStr, 1);
		toolBarInt = GUI.SelectionGrid (new Rect (Screen.width / 2 - 120, Screen.height - 115, 350, 30), toolBarInt, toolbarStrings, num);
		GUI.contentColor = Color.red;
		string total = totalFont + "  " + totalNum;
		GUI.Label (new Rect (Screen.width / 2 - 200, Screen.height - 150, 100, 30), total);
	}

	void OnPostRender ()
	{ //用GL绘制状态图//
		if (!mat) {
			return;
		}
		GL.PushMatrix ();
		mat.SetPass (0);
		GL.LoadOrtho ();
		GL.Begin (GL.LINES);
		GL.Color (colorOut);
		for (int a = 0; a < num-1f; a++) {
			GL.Vertex3 (circlePositon [a].x, circlePositon [a].y, 0f);
			GL.Vertex3 (circlePositon [a + 1].x, circlePositon [a + 1].y, 0f);

		}
		GL.Vertex3 (circlePositon [num - 1].x, circlePositon [num - 1].y, 0f);
		GL.Vertex3 (circlePositon [0].x, circlePositon [0].y, 0f);
		GL.End ();
		
		GL.Begin (GL.TRIANGLES);
		GL.Color (new Color (colorRe.r, colorRe.g, colorRe.b, 0.3f));
		for (int t= 0; t<num-1f; t++) {
			GL.Vertex3 (circlePositon [t].x, circlePositon [t].y, 0f);
			GL.Vertex3 (circlePositon [t + 1].x, circlePositon [t + 1].y, 0f);
			GL.Vertex3 (circleCenter.x, circleCenter.y, 0);
		}
		GL.Vertex3 (circlePositon [num - 1].x, circlePositon [num - 1].y, 0f);
		GL.Vertex3 (circlePositon [0].x, circlePositon [0].y, 0f);
		GL.Vertex3 (circleCenter.x, circleCenter.y, 0);
		GL.End ();
		
		GL.PopMatrix ();
	}
}
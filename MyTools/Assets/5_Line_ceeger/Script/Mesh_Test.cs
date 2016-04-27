/// <summary>
/// **** wangyel1 ****.
/// 用Mesh制作，可制作任意形状的多边形，但似乎麻烦了些
/// </summary>
using UnityEngine;
using System.Collections;

public class Mesh_Test : MonoBehaviour
{
	public int num = 6; //状态图的角数，可以任意设定，但背景图要对应制作//
	public string[] toBarStr = new string[]{"","",""}; //在脚本中使用汉字，屏幕上为乱码，所以在检视板指定汉字//
	public string totalFont;
	int selGridInt = 0; //GUI的返回值//
	string[] selStrings ; //GUI中“+” 和 “-” 按钮上的符号//
	string[] toolbarStrings ; //所加点数字符//
	Vector3[] point; //圆周上点的位置//
	int[] randomAdd; //随机增量，并保存对应点的初始变化量//
	Vector3[] circlePositon; //圆周上的位置//
	float totalNum = 0; //剩余点数//
	int toolBarInt = 0; //以下两个int变量GUI所用//
	int toBar = 0;
	int temp1 = 0; //递增临时变量//
	int temp2 = 0;
	Vector3[] vertice; //顶点数组//
	int[] tri ; //三角数组//
	Vector2[] uv; //uv数组//
	Mesh mesh;
	
	void Start ()
	{
		MeshFilter meshFilter = GetComponent<MeshFilter> ();   
		mesh = meshFilter.mesh;
		tri = new int[num * 3]; //以下根据num大大小设定具体的数组//
		vertice = new Vector3[num * 3];
		uv = new Vector2[num * 3];
		point = new Vector3[num];
		randomAdd = new int[num];
		circlePositon = new Vector3[num];
		selStrings = new string[num * 2];
		toolbarStrings = new string[num];
		for (int s= 0; s< num * 2; s ++) {
			if (s < num) {
				selStrings [s] = "+";
				toolbarStrings [s] = "";
			} else
				selStrings [s] = "--";
		}
		
		for (int t = 0; t<num*3; t++) { //设定triangles//
			tri [t] = t;
		}
		for (int v = 0; v<num *3; v ++) { //设定uv//
			switch (v % 3) {
			case 0:
				uv [v] = new Vector2 (0, 0);
				break;
			case 1:
				uv [v] = new Vector2 (0, 1);
				break;
			case 2:
				uv [v] = new Vector2 (1, 1);
				break;
			}
		}
		
		for (int i = 0; i< num; i++) { //确定圆周与圆周上的点，初始化一个随机图表//
			float angle = i * 2 * 3.14159f / num;
			point [i] = new Vector3 (Mathf.Sin (angle), Mathf.Cos (angle), 0);
			randomAdd [i] = Random.Range (1, 10);
			totalNum += randomAdd [i] * 10;
			circlePositon [i] = randomAdd [i] * point [i];
		}
		CirclePoint ();
		totalNum = num * 100 - totalNum;
		mesh.vertices = vertice;
		mesh.uv = uv;
		mesh.triangles = tri;
	}
	
	void CirclePoint () //设定mesh的各个点//
	{
		for (int v = 0; v<num *3; v ++) {
			switch (v % 3) {
			case 0:
				vertice [v] = Vector3.zero;
				break;
			case 1:
				vertice [v] = circlePositon [temp1]; 
				temp1 += 1;
				if (temp1 == num)
					temp1 = 0;
				break;
			case 2:
				temp2 += 1;
				if (temp2 == num) {
					temp2 = 0;
				}
				vertice [v] = circlePositon [temp2]; 
				break;
			}
		}
	}
	
	void Add (int addNum, string addOrSub) //点击递增递减//
	{ 
		switch (addOrSub) {
		case "add":
			if (randomAdd [addNum] >= 10) {
				randomAdd [addNum] = 10;
				return;
			}
			randomAdd [addNum] += 1;
			circlePositon [addNum] += point [addNum];
			totalNum -= 10;
			break;
		case "sub":
			if (randomAdd [addNum] <= 1) {
				randomAdd [addNum] = 1;
				return;
			}	
			randomAdd [addNum] -= 1;
			circlePositon [addNum] -= point [addNum];
			totalNum += 10;
			break;
		}
		CirclePoint ();
		mesh.vertices = vertice;
	}
	
	void OnGUI () //制作GUI//
	{ 
		GUI.color = Color.yellow;
		GUI.contentColor = Color.green;
		for (int t = 0; t<num; t++) {  //增加减少点数//
			toolbarStrings [t] = t + "->" + (randomAdd [t]) * 10;
		}
		selGridInt = GUI.SelectionGrid (new Rect (270, Screen.height - 80, num * 60, 60), selGridInt, selStrings, num);
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
		toolBarInt = GUI.SelectionGrid (new Rect (270, Screen.height - 115, num * 60, 30), toolBarInt, toolbarStrings, num);
		GUI.contentColor = Color.red;
		toBar = GUI.SelectionGrid (new Rect (180, Screen.height - 115, 70, 95), toBar, toBarStr, 1);
		string total = totalFont + "  " + totalNum;
		GUI.Label (new Rect (Screen.width / 2 - 200, Screen.height - 150, 100, 30), total);
	}
}

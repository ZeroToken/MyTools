/// <summary>
/// 动态制作背景线图。
/// 颜色可在材质中修改颜色达到。
/// </summary>
using UnityEngine;
using System.Collections;

public class DongTai_background : MonoBehaviour
{
	public Mesh_Test meshNum;
	int num = 6; //背景图的角数，获取Mesh_Test中的num数//
	Vector3[] point; //圆周上点的位置//
	Vector3[] circlePositon; //圆周上的位置//
	Vector3[] vertice; //顶点数组//
	int[] tri ; //三角数组//
	Vector2[] uv; //uv数组//
	int temp1 = 0; //递增临时变量//
	int temp2 = 0;
	Mesh mesh;
	
	void Start ()
	{
		num = meshNum.GetComponent<Mesh_Test>().num;
		MeshFilter meshFilter = GetComponent<MeshFilter> ();   
		mesh = meshFilter.mesh;
		tri = new int[num * 3];
		vertice = new Vector3[num * 3];
		uv = new Vector2[num * 3];
		point = new Vector3[num];
		circlePositon = new Vector3[num];
		
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
		
		for (int i = 0; i< num; i++) { //设定完整图形//
			float angle = i * 2 * 3.14159f / num;
			point [i] = new Vector3 (Mathf.Sin (angle), Mathf.Cos (angle), 0);
			circlePositon [i] = point [i] * 10;
		}
		
		for (int v = 0; v<num *3; v ++) { //设定顶点//
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
		mesh.vertices = vertice;
		mesh.uv = uv;
		mesh.triangles = tri;
	}
}

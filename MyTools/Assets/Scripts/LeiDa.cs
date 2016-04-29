using UnityEngine;
using System.Collections;

public class LeiDa : MonoBehaviour {

    [HideInInspector]
    [SerializeField]
    private int mVertexCount;
    public int vertexCount
    {
        get
        {
            return mVertexCount;
        }
        set
        {
            if (value <= 0) value = 1;
            mVertexCount = value;
            if (vertexSizes.Length != value)
                InitVertexSizes(value);
        }
    }
    public float globalSize = 1;
    public float[] vertexSizes;
    public string materialColorName = "_TintColor";
    public Color color;
    public float initVertexSize = 1;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    /// <summary>
    /// 平均弧度
    /// </summary>
    public float AvgRad { get { return 2 * Mathf.PI / vertexCount; } }
    /// <summary>
    /// 圆上的顶点坐标
    /// </summary>
    public Vector3[] CircleVertexPositions
    {
        get
        {
            Vector3[] positions = new Vector3[vertexCount];
            for (int i = 0; i < vertexCount; i++)
            {
                float angel = AvgRad * i;
                positions[i] = new Vector3(Mathf.Sin(angel), Mathf.Cos(angel), 0) * globalSize * vertexSizes[i];
            }
            return positions;
        }
    }

    public Vector3[] Vertices
    {
        get
        {
            Vector3[] positions = CircleVertexPositions;
            Vector3[] vertices = new Vector3[vertexCount * 3];
            int switch1 = 0, switch2 = 0;
            for (int i = 0, max = vertices.Length; i < max; i++)
            {
                switch (i % 3)
                {
                    case 0:
                        vertices[i] = Vector3.zero;
                        break;
                    case 1:
                        vertices[i] = positions[switch1];
                        switch1 += 1;
                        if (switch1 == vertexCount)
                            switch1 = 0;
                        break;
                    case 2:
                        switch2 += 1;
                        if (switch2 == vertexCount)
                            switch2 = 0;
                        vertices[i] = positions[switch2];
                        break;
                }
            }
            return vertices;
        }
    }

    public Vector2[] UV
    {
        get
        {
            Vector2[] uv = new Vector2[vertexCount * 3];
            for (int i = 0, max = uv.Length; i < max; i++)
            {
                int index = i % 3;
                if (index == 0)
                    uv[i] = new Vector2(0, 0);
                else if (index == 1)
                    uv[i] = new Vector2(0, 1);
                else if (index == 2)
                    uv[i] = new Vector2(1, 1);
            }
            return uv;
        }
    }

    public int[] Triangles
    {
        get
        {
            int[] triangles = new int[vertexCount * 3];
            for (int i = 0, max = triangles.Length; i < max; i++)
                triangles[i] = i;
            return triangles;
        }
    }


    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        DrawMap(vertexCount);
    }

    public void InitVertexSizes(int size)
    {
        if (vertexSizes != null)
        {
            //if (size != vertexSizes.Length)
            //{
            vertexSizes = new float[size];
            for (int i = 0; i < size; i++)
            {
                vertexSizes[i] = initVertexSize;
            }
            //}
        }
    }
    [ContextMenu("Draw Map")]
    public void DrawMap()
    {
        if (meshRenderer != null)
        {
            Material material = null;
            for (int i = 0; i < meshRenderer.materials.Length; i++)
            {
                material = meshRenderer.materials[i];
                if (material != null && material.color != null)
                {
                    material.SetColor(materialColorName, color);
                }

            }
        }
        if (meshFilter != null)
            meshFilter.mesh = new Mesh() { vertices = Vertices, uv = UV, triangles = Triangles };
    }

    public void DrawMap(int count, float[] verxSizes)
    {
        this.mVertexCount = count <= 0 ? 1 : count;
        this.vertexSizes = verxSizes;
        this.DrawMap();
    }

    public void DrawMap(int count)
    {
        this.vertexCount = count;
        this.DrawMap();
    }

    public void DrawMap(float[] verxSizes)
    {
        this.vertexSizes = verxSizes;
        this.DrawMap();
    }
}

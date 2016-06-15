using UnityEngine;
using System.Collections;

public class PanelMask : MonoBehaviour
{

    public UIPanel panel;
    public string shaderName;
    // Use this for initialization
    void Start()
    {
        panel = GetComponent<UIPanel>();
        Clip();
    }

    public Vector4 PanelArea()
    {
        Vector4 nguiArea = new Vector4();
        Vector4 clipRegion = panel.finalClipRegion;
        nguiArea.x = clipRegion.x - clipRegion.z / 2;
        nguiArea.y = clipRegion.y - clipRegion.w / 2;
        nguiArea.z = clipRegion.x + clipRegion.z / 2;
        nguiArea.w = clipRegion.y + clipRegion.w / 2;

        UIRoot uiRoot = UIRoot.list[0];
        float h = 2;
        Vector4 clipArea = new Vector4();
        Vector3 pos = panel.transform.position - uiRoot.transform.position;
        clipArea.x = pos.x + nguiArea.x * h / uiRoot.manualHeight;
        clipArea.y = pos.y + nguiArea.y * h / uiRoot.manualHeight;
        clipArea.z = pos.x + nguiArea.z * h / uiRoot.manualHeight;
        clipArea.w = pos.y + nguiArea.w * h / uiRoot.manualHeight;
        return clipArea;
    }

    void Clip()
    {
        Vector4 clipArea = PanelArea();
        ParticleSystem[] particleSystems = this.GetComponentsInChildren<ParticleSystem>();
        Shader _shader = Shader.Find(shaderName);
        Material _mat = null;
        for (int i = 0; i < particleSystems.Length; i++)
        {
            _mat = particleSystems[i].renderer.sharedMaterial;
            if (_mat != null)
            {
                if (_shader != null)
                _mat.shader = _shader;
                _mat.SetFloat("_MinX", clipArea[0]);
                _mat.SetFloat("_MinY", clipArea[1]);
                _mat.SetFloat("_MaxX", clipArea[2]);
                _mat.SetFloat("_MaxY", clipArea[3]);
            }
        }
    }
}

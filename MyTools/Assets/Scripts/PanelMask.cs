using UnityEngine;
using System.Collections;

public class PanelMask : MonoBehaviour {

    public UIPanel panel;
	// Use this for initialization
	void Start () 
    {
        panel = GetComponent<UIPanel>();
        Clip();
	}

    public Vector4 PanelArea()
    {
        Vector4 nguiArea = new Vector4();
        var clipRegion = panel.finalClipRegion;
        nguiArea.x = clipRegion.x - clipRegion.z / 2;
        nguiArea.y = clipRegion.y - clipRegion.w / 2;
        nguiArea.z = clipRegion.x + clipRegion.z / 2;
        nguiArea.w = clipRegion.y + clipRegion.w / 2;

        UIRoot uiRoot = null;
        float h = 2;
        Vector4 clipArea = new Vector4();
        var pos = panel.transform.position - uiRoot.transform.position;
        clipArea.x = pos.x + nguiArea.x * h / uiRoot.manualHeight;
        clipArea.y = pos.y + nguiArea.y * h / uiRoot.manualHeight;
        clipArea.z = pos.x + nguiArea.z * h / uiRoot.manualHeight;
        clipArea.w = pos.y + nguiArea.w * h / uiRoot.manualHeight;
        return clipArea;
    }

    void Clip()
    {
        Vector4 clipArea = PanelArea();
        var particleSystems = this.GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem particleSystem in particleSystems)
        {
            particleSystem.renderer.sharedMaterial.SetFloat("_MinX", clipArea[0]);
            particleSystem.renderer.sharedMaterial.SetFloat("_MinY", clipArea[1]);
            particleSystem.renderer.sharedMaterial.SetFloat("_MaxX", clipArea[2]);
            particleSystem.renderer.sharedMaterial.SetFloat("_MaxY", clipArea[3]);
        }

        MeshRenderer[] _renderers = this.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].renderer.sharedMaterial.SetFloat("_MinX", clipArea[0]);
            _renderers[i].renderer.sharedMaterial.SetFloat("_MinY", clipArea[1]);
            _renderers[i].renderer.sharedMaterial.SetFloat("_MaxX", clipArea[2]);
            _renderers[i].renderer.sharedMaterial.SetFloat("_MaxY", clipArea[3]);
        }

    }
}

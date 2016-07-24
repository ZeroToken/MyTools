using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ImageAtlas : MonoBehaviour
{
    [SerializeField]
    private ImageAtlasObject atlas;
    [SerializeField, TooltipAttribute("Under Resources")]
    private string atlasPath = "Sprites/";

    public bool IsNull
    {
        get
        {
            return atlas == null;
        }
    }

    public ImageAtlasObject Atlas
    {
        get
        {
            return atlas;
        }
        set
        {
            atlas = value;
        }
    }

    public void SetAtlas(string atlasName)
    {
        atlas = LoadAtlas(atlasPath + atlasName);
    }

    public ImageAtlasObject LoadAtlas(string path)
    {
        return Resources.Load(path, typeof(ImageAtlasObject)) as ImageAtlasObject;
    }
}

public static class ImageExtension
{
    public static ImageAtlas GetAltas(this Image image)
    {
        if (image)
        {
            return image.GetComponent<ImageAtlas>();
        }
        return null;
    }

    public static void SetSprite(this Image image, string spriteName)
    {
        if (image)
        {
            ImageAtlas _atlas = image.GetAltas();
            if (_atlas && !_atlas.IsNull)
                image.sprite = _atlas.Atlas.GetSprite(spriteName);
            else
                Debug.LogError(string.Format("[Image]{0} : Atlas is empty", image.name));
        }
    }

    public static void SetSprite(this Image image, int index)
    {
        if (image)
        {
            ImageAtlas _atlas = image.GetAltas();
            if (_atlas && !_atlas.IsNull)
                image.sprite = _atlas.Atlas.GetSprite(index);
            else
                Debug.LogError(string.Format("[Image]{0} : Atlas is empty", image.name));
        }
    }

    public static void SetAtlas(this Image image, string altasName)
    {
        if (image)
        {
            ImageAtlas atlas = image.GetAltas();
            if (atlas == null)
                atlas = image.gameObject.AddComponent<ImageAtlas>();
            if (atlas) atlas.SetAtlas(altasName);
        }
    }

}

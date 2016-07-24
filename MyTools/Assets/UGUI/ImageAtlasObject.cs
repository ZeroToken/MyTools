using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class ImageAtlasObject : ScriptableObject
{
    [SerializeField]
    private List<Sprite> sprites = new List<Sprite>();
    private Dictionary<string, Sprite> cacheSprites;


    public Sprite this[string name]
    {
        get
        {
            if(cacheSprites == null || cacheSprites.Count == 0 
                || cacheSprites.Count != sprites.Count)
            {
                cacheSprites = ToDictionary();
            }
            if (cacheSprites.ContainsKey(name))
                return cacheSprites[name];
            return null;
        }
    }
    public Sprite this[int index]
    {
        get
        {
            if (index < sprites.Count)
                return sprites[index];
            else
                Debug.LogError("[Image Atlas] Index out of bounds");
            return null;
        }
    }

    public Sprite GetSprite(string spriteName)
    {
        return this[spriteName];
    }

    public Sprite GetSprite(int index)
    {
        return this[index];
    }
#if UNITY_EDITOR
    public void Serialization(string path)
    {
        string[] filePaths = Directory.GetFiles(path);
        for (int i = 0; i < filePaths.Length; i++)
        {
            if (!filePaths[i].EndsWith(".meta"))
            {
                sprites.Add(UnityEditor.AssetDatabase.LoadAssetAtPath(filePaths[i], typeof(Sprite)) as Sprite);
            }
        }
    }
#endif

    public void Serialization(IList<Object> targets)
    {
        sprites.Clear();
        for (int i = 0; i < targets.Count; i++)
        {
            sprites.Add(targets[i] as Sprite);
        }
    }

    public Dictionary<string, Sprite> ToDictionary()
    {
        Dictionary<string, Sprite> _spTable = new Dictionary<string, Sprite>();
        for (int i = 0; i < sprites.Count; i++)
        {
            if (!_spTable.ContainsKey(sprites[i].name))
            {
                _spTable.Add(sprites[i].name, sprites[i]);
            }
        }
        return _spTable;
    }
}

using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

public class SpritePackerEditor : Editor
{
    private static string spriteScriptObjectTablePath = "Assets/Resources/Sprites/";
    [MenuItem("Assets/Sprite Packer")]
    public static void OnPacker() {

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (Directory.Exists(path))
        {
            if (!Directory.Exists(spriteScriptObjectTablePath))
            {
                Directory.CreateDirectory(spriteScriptObjectTablePath);
            }
            ImageAtlasObject spriteTable = ScriptableObject.CreateInstance<ImageAtlasObject>();
            spriteTable.Serialization(path);
            Debug.Log(spriteScriptObjectTablePath + Path.GetFileName(path) + ".asset");
            AssetDatabase.CreateAsset(spriteTable, spriteScriptObjectTablePath + Path.GetFileName(path) + ".asset");
            Selection.activeObject = spriteTable;
        }
        else
        {
            Debug.LogError("The current path is not a folder");
        }
    }
}

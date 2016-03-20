/***************************************************************************

文件名（File Name）    :  LuaEditor.cs

作者（Author）         :  whf

创建时间（Create Time）:  2016-3-20 18:41:55

****************************************************************************/
using UnityEngine;
using UnityEditor;
using LuaInterface;
using System.IO;

public class LuaEditor : Editor
{
    [MenuItem("Lua/Create Lua Library")]
    public static void CreateLibrary()
    {
        string[] paths = Directory.GetFiles(LuaConfig.LUA_LIBRARY_SOURCE_PATH, "*.lua", SearchOption.AllDirectories);
        foreach (var v in paths)
        {
            string path = v.Replace(LuaConfig.LUA_LIBRARY_SOURCE_PATH, LuaConfig.LUA_LIBRARY_PACK_PATH) + ".txt";
            string pathParent = Directory.GetParent(path).ToString();
            if (!Directory.Exists(pathParent))
                Directory.CreateDirectory(pathParent);
            Debug.Log("create: " + path);
            File.WriteAllBytes(path, File.ReadAllBytes(v));
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("Lua/Clear Lua Library")]
    public static void ClearLibrary()
    {
        string[] paths = Directory.GetFiles(LuaConfig.LUA_LIBRARY_SOURCE_PATH, "*.lua", SearchOption.AllDirectories);
        foreach (var v in paths)
        {
            string path = v.Replace(LuaConfig.LUA_LIBRARY_SOURCE_PATH, LuaConfig.LUA_LIBRARY_PACK_PATH) + ".txt";
            string pathParent = Directory.GetParent(path).ToString();
            if (Directory.Exists(pathParent))
                Directory.Delete(pathParent, true);
        }
        AssetDatabase.Refresh();
    }
}

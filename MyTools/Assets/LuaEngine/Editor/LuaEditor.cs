/***************************************************************************

文件名（File Name）    :  LuaEditor.cs

作者（Author）         :  whf

创建时间（Create Time）:  2016-3-20 18:41:55

****************************************************************************/
using UnityEngine;
using System.Collections;
using UnityEditor;
using LuaInterface;
using System.IO;

public class LuaEditor : Editor
{
    [MenuItem("Lua/Create Library")]
    public static void CreateLibrary()
    {
        if (!Directory.Exists(LuaConfig.LUA_LIBRARY_PACK_PATH))
            Directory.CreateDirectory(LuaConfig.LUA_LIBRARY_PACK_PATH);
        string[] paths = Directory.GetFiles(LuaConfig.LUA_LIBRARY_SOURCE_PATH, "*.lua", SearchOption.AllDirectories);
        foreach (var v in paths)
        {
            string path = LuaConfig.LUA_LIBRARY_PACK_PATH + "/" + Path.GetFileName(v) + ".txt";
            Debug.Log("create: " + path);
            File.WriteAllBytes(path, File.ReadAllBytes(v));
        }
        AssetDatabase.Refresh();
    }
}

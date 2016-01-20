using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using System.IO;

public class LuaJitTools : EditorWindow
{
    private static string rootPath = Application.dataPath + "/LuaEngine/";
    private static string binScriptPath = Application.dataPath + "/LuaEngine/Scripts/Bin/";
    private static string sourceScriptPath = Application.dataPath + "/LuaEngine/Scripts/Source/";
    private static string zipPath = Application.dataPath + "/LuaEngine/Scripts/Zip/";


    [MenuItem("Lua/工具/编译字节码(加密)", false, 1)]
    public static void CMDBulidBytes()
    {
        string workPath = sourceScriptPath.Replace("/", "\\");
        string savePath = binScriptPath.Replace("/", "\\");
        string buildBat = rootPath + "Tools/LuaJitBulid/Build.bat";

        System.Diagnostics.Process process = new System.Diagnostics.Process();
        process.StartInfo.FileName = buildBat;
        process.StartInfo.UseShellExecute = true;
        //这里相当于传参数 
        process.StartInfo.Arguments = string.Format("{0} {1}", workPath, savePath);
        Debug.Log(process.StartInfo.Arguments);
        process.Start();
        process.WaitForExit();
        process.Close();
    }

    /// <summary>
    /// 需忽略文件列表
    /// </summary>
    private static string[] filters = new string[] { ".meta", ".DS_Store" };
    [MenuItem("Lua/工具/压缩Lua字节码文件(zip)", false, 2)]
    public static void ZipBinLua()
    {
        ZipHelper.CompressFolder(binScriptPath, zipPath, "Hotfix.zip", filters);
        AssetDatabase.Refresh();
    }

    [MenuItem("Lua/工具/压缩Lua原文件(zip)", false, 3)]
    public static void ZipSourceLua()
    {
        ZipHelper.CompressFolder(sourceScriptPath, zipPath, "Hotfix.zip", filters);
        AssetDatabase.Refresh();
    }

    [MenuItem("Lua/工具/解压zip至缓存路径", false, 4)]
    public static void DecompressToCache()
    {
        ZipHelper.UnZip(zipPath + "Hotfix.zip", Application.temporaryCachePath, true);
        AssetDatabase.Refresh();
    }

    [MenuItem("Lua/工具/解压zip至编辑器", false, 4)]
    public static void DecompressToEditor()
    {
        ZipHelper.UnZip(zipPath + "Hotfix.zip", zipPath, true);
        AssetDatabase.Refresh();
    }

}
//using UnityEngine;
//using System.Collections;
//using UnityEditor;

//public static class LuaEditor
//{
//    private static string staticPath = Application.dataPath + "/LuaEngine/Scripts/";
//    private static string compressSavePath = Application.dataPath + "/LuaEngine/Scripts/Zip/";

//    private static string[] filters = new string[]{".meta", ".DS_Store"};

//    [MenuItem("Lua/Package Zip/Bin", false, 15)]
//    public static void ZipBinLua()
//    {
//        string path = staticPath + "Bin";
//        Compress(path);
//    }

//    [MenuItem("Lua/Package Zip/Source", false, 15)]
//    public static void ZipSourceLua()
//    {
//        string path = staticPath + "Source";
//        Compress(path);
//    }

//    [MenuItem("Lua/Package Zip/Uncompress", false, 15)]
//    public static void Uncompress()
//    {
//        string file = compressSavePath + "Hotfix_0.zip";
//        string uncompressSavePath = Application.temporaryCachePath;
//        ZipHelper.UnZip(file, uncompressSavePath, true);
//        Debug.Log("解压成功！保存路径：" + uncompressSavePath);
//    }

//    public static void Compress(string folter)
//    {
//        ZipHelper.CompressFolder(folter, compressSavePath, "Hotfix_0.zip", filters);
//        Debug.Log("压缩成功！保存路径: " + compressSavePath);
//    }
//}

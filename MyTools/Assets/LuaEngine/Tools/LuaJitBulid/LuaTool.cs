//using UnityEngine;
//using System.Collections;
//using UnityEditor;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Xml;

//public class LuaJitTool : EditorWindow {


//    static string sourceDirectory = Application.dataPath + "/StreamingAssets/";
//    static string destDirectory = "E:/AssetbundlesData/";

//    static string luajitPath = Application.dataPath + "/LuaEngine/Tools/LuaJitBulid/luajit.exe";
//    static string luajitDir = GetPlatformPath(BuildTarget.StandaloneWindows)+"/Lua";

//    static List<string> paths = new List<string>();
//    static List<string> files = new List<string>();

//    static List<string> dumpNameCheckList = new List<string>();
//    public static string getFolder(string path)
//    {
//        int startIndex = path.IndexOf("Assets/Lua") + "Assets/Lua".Length;
//        return path.Substring(startIndex)+"/";
//    }

//    [MenuItem("AssetBundle/1. LuaJit转换")]
//    public static void BuildProtobufFile()
//    {
//        string dir = Application.dataPath + "/Lua/";
//        paths.Clear(); files.Clear(); Recursive(dir);
//        dumpNameCheckList.Clear();

//        foreach (string f in files)
//        {
//            string name = Path.GetFileName(f);
//            string path = Path.GetDirectoryName(f);
//            string destPath = luajitDir + getFolder(path);
//            if (!Directory.Exists(destPath))
//            {
//                Directory.CreateDirectory(destPath);
//            }

//            string ext = Path.GetExtension(f);
//            if (!ext.Equals(".lua")) continue;
//            //检测同名lua类
//            if (!dumpNameCheckList.Contains(name))
//            {
//                dumpNameCheckList.Add(name);
//            }
//            else
//            {
//                EditorUtility.ClearProgressBar();
//                if (EditorUtility.DisplayDialog("错误！", "有重名文件：" + name, "结束", "继续"))
//                {
//                    return;
//                }
//            }
//            ProcessStartInfo info = new ProcessStartInfo();
//            info.FileName = luajitPath;
//            info.Arguments = " -b " + f + " " + destPath +"/"+ name;
//            UnityEngine.Debug.Log(info.Arguments);
//            info.WindowStyle = ProcessWindowStyle.Hidden;
//            info.CreateNoWindow = true;
//            info.UseShellExecute = true;
//            info.WorkingDirectory = dir;
//            info.ErrorDialog = true;
//            //UnityEngine.Debug.Log(info.FileName + " " + info.Arguments);

//            Process pro = Process.Start(info);
//            pro.WaitForExit();
            
//            EditorUtility.DisplayProgressBar("LuaJit", "正在生成:"+name+" 到 "+luajitDir+getFolder(path)+name, files.IndexOf(f)/(float)files.Count);
//        }

//        AssetDatabase.Refresh();
//        EditorUtility.ClearProgressBar();
//        EditorUtility.DisplayDialog("LuaJit", "LuaJit打包成功！", "确定");
//    }

//    /// <summary>
//    /// 遍历目录及其子目录
//    /// </summary>
//    static void Recursive(string path)
//    {
//        string[] names = Directory.GetFiles(path);
//        string[] dirs = Directory.GetDirectories(path);
//        foreach (string filename in names)
//        {
//            string ext = Path.GetExtension(filename);
//            if (ext.Equals(".lua"))
//                files.Add(filename.Replace('\\', '/'));
//        }
//        foreach (string dir in dirs)
//        {
//            paths.Add(dir.Replace('\\', '/'));
//            Recursive(dir);
//        }
//    }

//    [MenuItem("AssetBundle/3.拷贝LUA到服务器")]
//    public static void copyLuaToServer()
//    {
//        copyDirectory(sourceDirectory, destDirectory);
//    }

//    /** 创建资源 */
//    [MenuItem("AssetBundle/2.打包LUA")]
//    public static void createAssetBundle_XML()
//    {
//        BuildTarget target= BuildTarget.StandaloneWindows;
//        XmlDocument XmlDoc = new XmlDocument();
//        XmlElement XmlRoot = XmlDoc.CreateElement("AllLua");
//        XmlDoc.AppendChild(XmlRoot);

//        //获取在Project视图中选择的所有游戏对象
//        Object[] SelectedAsset = Selection.GetFiltered(typeof(object), SelectionMode.DeepAssets);



//        //遍历所有的游戏对象
//        foreach (Object obj in SelectedAsset)
//        {
//            string sourcePath = AssetDatabase.GetAssetPath(obj);
//            if (sourcePath.EndsWith(".lua") && !sourcePath.Contains("AllLua.xml"))
//            {

//                //这一段是把bytecode文件打包成assetbundle，目前我没有到，所以先屏蔽掉。打开可直接使用
//                //string targetPath = sourcePath + ".assetbundle";
//                //uint crc;
//                //BuildPipeline.BuildAssetBundle(obj, null, targetPath, out crc, BuildAssetBundleOptions.CollectDependencies, target);

//                XmlElement xmlGroupNode = XmlDoc.CreateElement("lua");
//                XmlRoot.AppendChild(xmlGroupNode);
//                xmlGroupNode.SetAttribute("name", obj.name + ".lua");
//                //xmlGroupNode.SetAttribute("crc", crc.ToString());
//                xmlGroupNode.SetAttribute("url", getDownloadUrl(target, sourcePath));
//                xmlGroupNode.SetAttribute("type", "3");
//            }
//        }

//        XmlDoc.Save(GetPlatformPath(target) + "Lua/luafiles.xml");
//        XmlRoot = null;
//        XmlDoc = null;
//        AssetDatabase.Refresh();
//    }

//    /// <summary>
//    /// 获取下载路径
//    /// </summary>
//    /// <param name="target"></param>
//    /// <param name="path"></param>
//    /// <returns></returns>
//    public static string getDownloadUrl(UnityEditor.BuildTarget target, string path)
//    {
//        string savePath = null;
//        switch (target)
//        {
//            case BuildTarget.StandaloneWindows:
//                {
//                    savePath = path.Substring(path.IndexOf("Windows32") + "Windows32".Length + 1);
//                }
//                break;
//            case BuildTarget.Android:
//                {
//                    savePath = path.Substring(path.IndexOf("Android") + "Android".Length + 1);
//                }
//                break;
//            case BuildTarget.iPhone:
//                {
//                    savePath = path.Substring(path.IndexOf("IOS") + "IOS".Length + 1);
//                }
//                break;
//        }
//        return savePath;
//    }
//    public static string GetPlatformPath(UnityEditor.BuildTarget target)
//    {
//        string savePath = null;
//        switch (target)
//        {
//            case BuildTarget.StandaloneWindows:
//                {
//                    savePath = Application.dataPath + "/StreamingAssets/Windows32/";
//                }
//                break;
//            case BuildTarget.Android:
//                {
//                    savePath = Application.dataPath + "/StreamingAssets/Android/";
//                }
//                break;
//            case BuildTarget.iPhone:
//                {
//                    savePath = Application.dataPath + "/StreamingAssets/IOS/";
//                }
//                break;
//        }
//        DirectoryInfo info = Directory.CreateDirectory(savePath);

//        if (Directory.Exists(info.FullName) == false)
//            Directory.CreateDirectory(info.FullName);

//        return info.FullName;
//    }

//    /// <summary>
//    /// 拷贝所有资源文件到服务器目录
//    /// </summary>
//    /// <param name="source"></param>
//    /// <param name="dest"></param>
//    public static void copyDirectory(string sourceDirectory, string destDirectory)
//    {
//        if (!Directory.Exists(sourceDirectory))
//        {
//            EditorUtility.DisplayDialog("", "源目录不存在！！！", "OK");
//        }
//        if (!Directory.Exists(destDirectory))
//        {
//            Directory.CreateDirectory(destDirectory);
//        }

//        //拷贝文件
//        copyFile(sourceDirectory, destDirectory);
//        //拷贝子目录       

//        //获取所有子目录名称
//        string[] directionName = Directory.GetDirectories(sourceDirectory);
//        foreach (string directionPath in directionName)
//        {
//            //根据每个子目录名称生成对应的目标子目录名称
//            string directionPathTemp = destDirectory + "\\" + directionPath.Substring(sourceDirectory.Length);
//            //递归下去
//            copyDirectory(directionPath, directionPathTemp);
//        }
//    }
//    /// <summary>
//    /// 拷贝文件
//    /// </summary>
//    /// <param name="sourceDirectory"></param>
//    /// <param name="destDirectory"></param>
//    public static void copyFile(string sourceDirectory, string destDirectory)
//    {
//        //获取所有文件名称
//        string[] fileName = Directory.GetFiles(sourceDirectory);
//        foreach (string filePath in fileName)
//        {
//            if (filePath.EndsWith(".meta") || filePath.Contains(".xml.assetbundle"))//不拷贝meta文件
//            {
//                continue;
//            }
//            //根据每个文件名称生成对应的目标文件名称
//            string filePathTemp = destDirectory + "\\" + filePath.Substring(sourceDirectory.Length + 1);
//            //若不存在，直接复制文件；若存在，覆盖复制
//            if (File.Exists(filePathTemp))
//            {
//                File.Copy(filePath, filePathTemp, true);
//            }
//            else
//            {
//                File.Copy(filePath, filePathTemp);
//            }
//        }
//    }
//}

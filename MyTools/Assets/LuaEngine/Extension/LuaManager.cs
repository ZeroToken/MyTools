using System.Collections.Generic;
using LuaInterface;
using System.IO;
using UnityEngine;
/// <summary>
/// 单例模式
/// </summary>
public class LuaManager 
{
	public LuaManager()
	{
		luaState = new LuaState();
        AddSearchPath(luaPath);
		defaultScripts.Add("MainEntry");
		for (int i = 0; i < defaultScripts.Count; i++)
		{
			DoRequire(defaultScripts[i]);
		}
	}
	
    private static LuaManager _instance;
    public static LuaManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LuaManager();
            }
            return _instance;
        }
    }
	/// <summary>
	/// Gets the state of the lua.
	/// </summary>
	/// <value>The state of the lua.</value>
	public LuaState luaState { get; private set; }

	private List<string> defaultScripts = new List<string>();

    public enum FileGetFrom
    {
        Default,
        EditorPathBin,
        EditorPathSource,
        TemporaryCachePathBin,
        TemporaryCachePathSource,
    }
	/// <summary>
	/// Gets the lua path.
	/// </summary>
	/// <value>The lua path.</value>
    private string luaPath
    {
        get
        {
#if UNITY_EDITOR
    #if LUA_SOURCE
            return Application.dataPath + "/LuaEngine/Scripts/Source/";
    #elif LUA_BIN
            return Application.dataPath + "/LuaEngine/Scripts/Bin/";
    #endif
#else
    #if LUA_SOURCE
            return Application.temporaryCachePath + "/Source/";
    #elif LUA_BIN
            return Application.temporaryCachePath + "/Bin/" 
    #endif
#endif
        }
    }

	/// <summary>
	/// Gets the lua object.
	/// </summary>
	/// <returns>The lua object.</returns>
	/// <param name="objectName">Object name.</param>
	/// <param name="args">Arguments.</param>
    public LuaTable GetLuaObject(string objectName, params object[] args)
    {
		try
		{
			LuaTable lc = GetModule(objectName);
			if (lc == null) 
			{
				Debug.LogWarning("error: not found lua object: " + objectName);
				return null;
			}
			else
			{
				object lcNew = lc["New"];
				if(lcNew == null)
				{
					Debug.LogError("error: 'New' function was not found");
					return null;
				}
					
				if(!(lcNew is LuaFunction))
				{
					Debug.LogError("error: 'New' Type was not function");
					return null;
				}
				return (lcNew as LuaFunction).Call(args)[0] as LuaTable;
			}
		}
		catch(System.Exception ex)
		{
			Debug.LogError("error: " + ex.Message);
			return null;
		}
    }


    /// <summary>
    /// 获取Lua中的模块
    /// 返回一个Table对象作为模块
    /// </summary>
    /// <param name="luaFileText">Lua文件中的文本内容</param>
    /// <returns></returns>
    public LuaTable GetModule(string module)
    {
        try
        {
            return DoRequire(module)[0] as LuaTable;
        }
        catch(System.Exception ex)
        {
            Debug.LogError(string.Format("error : {0}", ex.Message));
            return null;
        }
    }
	

	public object[] DoFile(string fileName)
	{
		string filePath = luaPath + fileName + ".lua";
		if(File.Exists(filePath))
		{
			try
			{
                return luaState == null ? null : luaState.DoFile(filePath);
			}
			catch(System.Exception ex)
			{
				Debug.LogError(ex.Message);
			}
		}
		else
			Debug.Log(string.Format("error: '{0}' not found", fileName));
		return null;
	}

	public object[] DoBytes(byte[] buff)
	{
		try
		{
			if(luaState != null)
			{
				if(LuaDLL.luaL_loadbuffer(luaState.L, buff, buff.Length, "chunk") == 0)
				{
					int oldTop = LuaDLL.lua_gettop(luaState.L);
					if(LuaDLL.lua_pcall(luaState.L, 0, -1, 0) == 0)
						return luaState.translator.popValues(luaState.L, oldTop);
				}
			}
			return null;
		}
		catch(System.Exception ex)
		{
			Debug.LogError("error: " + ex.Message);
			return null;
		}
	}

	public object[] DoString(string doString)
	{
        return luaState == null ? null : luaState.DoString(doString);
	}

	public object[] DoRequire(string luaScript)
	{
		try
		{
			if(luaScript.Contains(".lua")) luaScript = luaScript.Split('.')[0];
			return DoString(string.Format("return require '{0}'", luaScript));
		}
		catch(System.Exception ex)
		{
			Debug.LogError("error: " + ex.Message);
			return null;
		}

	}

    /// <summary>
    /// 添加Lua文件搜索路径
    /// </summary>
    public void AddSearchPath(string rootPath)
    {
        string dostring = @"
            function AddSearchPath( path )
	            package.path = package.path..';'..path;
            end";
        luaState.DoString(dostring);
        if (Directory.Exists(rootPath))
        {
            try
            {
                List<string> luaDirectory = GetLuaDirectory(rootPath);
                foreach (var v in luaDirectory)
                {
                    //            Debug.Log("add lua search path : " + v);
                    DoString(string.Format("AddSearchPath('{0}')", v));
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("error: " + ex.Message);
            }

        }
        else
            Debug.LogError("error: Does not exist: " + rootPath);
    }

    /// <summary>
    /// 根据Lua项目根目录获取全部包含子目录的集合
    /// </summary>
    /// <param name="parent">根目录</param>
    /// <returns>返回结果列表</returns>
    public List<string> GetLuaDirectory(string parent)
    {
        List<string> luaDirectory = new List<string>();
        GetEndDirectories(parent, luaDirectory);
        return luaDirectory;
    }

    /// <summary>
    /// 查找指定目录下的所有末级子目录
    /// </summary>
    /// <param name="rootPath">根目录</param>
    /// <param name="dirList">查找结果列表</param>
    private void GetEndDirectories(string rootPath, List<string> dirList)
    {
        if (!dirList.Contains(rootPath)) dirList.Add(rootPath + "?.lua");
        //获取当前目录下的所有文件和文件夹
        string[] filenames = Directory.GetFileSystemEntries(rootPath);
        //遍历所有文件和文件夹
        foreach (var v in filenames)
        {
            //当当前目录下存在文件夹时，遍历该文件夹下的文件
            if (Directory.Exists(v))
                GetEndDirectories(v + "/", dirList);
        }
    }
}

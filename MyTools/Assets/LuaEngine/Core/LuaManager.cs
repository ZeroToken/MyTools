using LuaEngine;
using System.IO;
using UnityEngine;
using System.Collections.Generic;



namespace LuaEngine
{
    public class LuaManager
    {
        private static LuaManager mInstance;
        public static LuaManager Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = new LuaManager();
                return mInstance;
            }
        }

        public LuaManager()
        {
            mLuastate = new LuaState();
            foreach (var script in LuaConfig.DefaultScripts)
                this.DoResourceFile(script);
            this.AddGlobalFunction("AddSearchPath", luaState.GetFunction("AddSearchPath"));
            this.AddSearchPath(LuaConfig.ScriptAssetsPath);

        }

        public LuaState mLuastate;
        public LuaState luaState
        {
            get
            {
                if (mLuastate == null)
                    mLuastate = new LuaState();
                return mLuastate;
            }
        }

        private Dictionary<string, LuaFunction> mGlobalFunction = new Dictionary<string, LuaFunction>();
        public void AddGlobalFunction(string key, LuaFunction function)
        {
            if (!mGlobalFunction.ContainsKey(key))
                mGlobalFunction.Add(key, function);
            else
                mGlobalFunction[key] = function;
        }
        public LuaFunction GetGlobalFunction(string name)
        {
            if (mGlobalFunction.ContainsKey(name))
                return mGlobalFunction[name];
            return null;
        }



        public LuaTable New(string objectName, params object[] args)
        {
            try
            {
                LuaTable luaClass = Require(objectName);
                if(luaClass != null)
                {
                    LuaFunction createFunc = luaClass["New"] as LuaFunction;
                    if (createFunc != null)
                    {
                        object[] returns = createFunc.Call(args);
                        if(returns.Length > 0)
                            return returns[0] as LuaTable;
                    }
                }
                Debug.LogError("Lua Error: Create " + objectName + "Failed");
                return null;
            }
            catch (System.Exception ex)
            {
                Debug.LogError("error: " + ex.Message);
                return null;
            }
        }

        public LuaTable Require(string luaScript)
        {
            object[] returns = DoRequire(luaScript);
            if (returns != null && returns.Length > 0)
                return returns[0] as LuaTable;
            return null;
        }

        public object[] DoRequire(string luaScript)
        {
            try
            {
                luaScript = Path.GetFileNameWithoutExtension(luaScript);
                return this.luaState.DoString(string.Format("return require '{0}'", luaScript));
            }
            catch (System.Exception requireEx)
            {
                Debug.LogWarning("Lua Warning: " + requireEx.Message);
                luaScript = Path.GetFileNameWithoutExtension(luaScript);
                object[] returns = this.DoResourceFile(luaScript + ".lua");
                if (returns == null)
                    returns = new object[] { luaState.GetTable(luaScript) };
                return returns;
            }

        }

        /// <summary>
        /// 添加Lua文件搜索路径
        /// </summary>
        public void AddSearchPath(string rootPath)
        {
            if (Directory.Exists(rootPath))
            {
                string[] paths = Directory.GetDirectories(rootPath);
                foreach (var path in paths)
                    this.DoAddSearchPath(path.Replace("\\", "/") + "/?.lua");
                this.DoAddSearchPath(rootPath + "/?.lua");
            }
            else
                Debug.LogError("Lua error: Does not exist " + rootPath);
        }

        public bool DoAddSearchPath(string path)
        {
            //Debug.Log("add search path: " + path);
            LuaFunction addSearchPathFunc = this.GetGlobalFunction("AddSearchPath");
            if (addSearchPathFunc == null)
            {
                addSearchPathFunc = this.luaState.GetFunction("AddSearchPath");
                if(addSearchPathFunc != null)
                    this.AddGlobalFunction("AddSearchPath", addSearchPathFunc);
            }
            if (addSearchPathFunc != null)
            {
                addSearchPathFunc.Call(path);
                return true;
            }
            return false;
        }

        public string LoadScript(string fileName)
        {
            TextAsset textAsset = Resources.Load("Lua/" + fileName, typeof(TextAsset)) as TextAsset;
            if (textAsset != null)
            {
                return textAsset.text;
            }
                
            else
            {
                Debug.LogWarning(string.Format("Lua Warning: {0} not loaded", fileName));
                return string.Empty;
            }        
        }

        public object[] DoResourceFile(string fileName)
        {
            return luaState.DoString(LoadScript(fileName));
        }
    }

}


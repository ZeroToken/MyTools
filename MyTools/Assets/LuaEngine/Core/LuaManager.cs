using LuaInterface;
using System.IO;
using UnityEngine;
using System.Collections.Generic;



namespace LuaInterface
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

        private LuaScriptMgr mScriptMgr;

        public LuaManager()
        {
            mScriptMgr = new LuaScriptMgr();
            foreach (var script in LuaConfig.DefaultScripts)
                this.luaState.DoString(LuaStatic.LoadScriptText(script));
            this.AddGlobalFunction("AddSearchPath", luaState.GetFunction("AddSearchPath"));
            this.AddSearchPath(LuaConfig.ScriptAssetsPath);
            mScriptMgr.Start();
        }

        private LuaState mLuastate;
        public LuaState luaState
        {
            get
            {
                if (mLuastate == null)
                    mLuastate = mScriptMgr.lua;
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
                Debug.LogError(requireEx.Message);
                return null;
            }

        }

        /// <summary>
        /// 添加Lua文件搜索路径
        /// </summary>
        public void AddSearchPath(string rootPath)
        {
            if (Directory.Exists(rootPath))
            {
                this.DoAddSearchPath(rootPath + "/?.lua");
                string[] paths = Directory.GetDirectories(rootPath);
                foreach (var path in paths)
                    this.DoAddSearchPath(path.Replace("\\", "/") + "/?.lua");
            }
            else
                Debug.LogError("Lua error: Does not exist " + rootPath);
        }

        public bool DoAddSearchPath(string path)
        {
            LuaFunction addSearchPathFunc = this.GetGlobalFunction("AddSearchPath");
            if (addSearchPathFunc == null)
            {
                this.luaState.DoString(@"
                        function AddSearchPath(path)
                            package.path = package.path..';'..path;
                        end");
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
    }

}


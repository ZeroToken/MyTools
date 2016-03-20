using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LuaInterface
{
    public class LuaConfig
    {
        public static string ScriptAssetsPath
        {
            get
            {
#if UNITY_EDITOR
                return Application.dataPath + "/LuaEngine/Scripts";
#else
                return Application.temporaryCachePath + "/LuaScripts";
#endif
            }
        }

        private static List<string> mDefaultScripts;
        public static List<string> DefaultScripts
        {
            get
            {
                if (mDefaultScripts == null)
                {
                    mDefaultScripts = new List<string>();
                    mDefaultScripts.Add("Functions.lua");
                }
                return mDefaultScripts;
            }
        }
        /// <summary>
        /// Lua库源文件路径
        /// </summary>
        public static string LUA_LIBRARY_SOURCE_PATH
        {
            get
            {
                return Application.dataPath + "/uLua/Lua";
            }
        }
        /// <summary>
        /// Lua库打包路径
        /// </summary>
        public static string LUA_LIBRARY_PACK_PATH
        {
            get
            {
                return Application.dataPath + "/LuaEngine/Resources/Lua";
            }
        }

    }
}


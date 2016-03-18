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
    }
}


using UnityEngine;
using System.Collections;
using System.IO;

public class ScriptNoteCreate : UnityEditor.AssetModificationProcessor
{

    public static void OnWillCreateAsset(string path)
    {
        path = path.Replace(".meta", "");
        if (path.EndsWith(".cs"))
        {
            string allText = File.ReadAllText(path);
            allText = allText.Replace("#AuthorName#", System.Environment.UserName).Replace("#CreateTime#",
                System.DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Day + " " +
                System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute + ":" + System.DateTime.Now.Second);
            File.WriteAllText(path, allText);
        }

    }
}
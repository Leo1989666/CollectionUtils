#if UNITY_EDITOR

using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class QuickMenuBarToolEditor
{
    [MenuItem("DontShootingAlone/Utils/OpenPersistentDataPath")]
    public static void Util_OpenPersistentDataPath()
    {
        string path = Application.persistentDataPath;
        EditorUtility.RevealInFinder(path);
    }
    
    [MenuItem("DontShootingAlone/Utils/DeleteSaveFile")]
    public static void Util_DeleteSaveFile()
    {
        string path = Application.persistentDataPath + "/savegamedata/";
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }
}
#endif
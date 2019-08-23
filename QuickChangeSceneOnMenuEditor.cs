#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class QuickChangeSceneOnMenuEditor
{
    public static string MockupScenePath = "Assets/_Dev/_Mockup/MockupScene.unity";

    static QuickChangeSceneOnMenuEditor()
    {
        EditorApplication.playmodeStateChanged += LoadLastOpenedScene;
    }

    public static void openSceneWithSaveConfirm(string scenePath)
    {
        // Refresh first to cause compilation and include new assets
        AssetDatabase.Refresh();
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene(scenePath);
    }

    [MenuItem("Tools/Object Count")]
    public static void CountObject()
    {
        int count = 0, total = 0;
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            count = GetChildCount(Selection.gameObjects[i]);
            Debug.Log("GO:" + Selection.gameObjects[i].name + " has " + count + " game objects!");
            total += count;
        }

        Debug.Log("Total:" + total);
    }

    private static int GetChildCount(GameObject gameObject)
    {
        int count = 1;

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            count += GetChildCount(gameObject.transform.GetChild(i).gameObject);
        }
        return count;
    }

    #region Quick search scene
    [MenuItem("DontShootingAlone/Mockup Scene")]
    public static void QuickLoadSceneMockup()
    {
        openSceneWithSaveConfirm(MockupScenePath);
    }
    #endregion

    // pref IDs
    private const string LAST_OPENED_SCENE = "Game.LastOpenedScene";
    private const string PLAYED_USING_RUN_UTILS = "Game.PlayedUsingRunUtils";

    // bool states
    private static bool aboutToRun = false;

    [MenuItem("DontShootingAlone/Run Game")]
    public static void Run()
    {
        //SceneSetup[] setups = EditorSceneManager.GetSceneManagerSetup();
        //if (setups.Length > 0)
        //{
        //    EditorPrefs.SetString(LAST_OPENED_SCENE, setups[0].path);
        //}

        //EditorPrefs.SetBool(PLAYED_USING_RUN_UTILS, true);
        //aboutToRun = true;

        //openSceneWithSaveConfirm(LoadingScenePath);

        //EditorApplication.isPlaying = true;
    }

    private static void LoadLastOpenedScene()
    {
        if (EditorApplication.isPlaying || EditorApplication.isCompiling)
        {
            // changed to playing or compiling
            // no need to do anything
            return;
        }

        if (!EditorPrefs.GetBool(PLAYED_USING_RUN_UTILS))
        {
            // this means that normal play mode might have been used
            return;
        }

        // We added this check because this method is still invoked while EditorApplication.isPlaying is false
        // We only load the last opened scene when the aboutToRun flag is "consumed"
        if (aboutToRun)
        {
            aboutToRun = false;
            return;
        }

        // at this point, the scene has stopped playing
        // so we load the last opened scene
        string lastScene = EditorPrefs.GetString(LAST_OPENED_SCENE);
        if (!string.IsNullOrEmpty(lastScene))
        {
            EditorSceneManager.OpenScene(lastScene);
        }

        EditorPrefs.SetBool(PLAYED_USING_RUN_UTILS, false); // reset flag
    }

}
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class QuickStart : Editor
{
    [MenuItem("Tools/Open Interior Scene")]
    private static void OpenInteriorScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/InteriorScene.unity", OpenSceneMode.Single);
    }

    [MenuItem("Tools/Open Overworld Scene")]
    private static void OpenOverworldScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/OverworldScene.unity", OpenSceneMode.Single);
    }

    [MenuItem("Tools/Open Battle Scene")]
    private static void OpenBattleScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/BattleScene.unity", OpenSceneMode.Single);
    }

    [MenuItem("Tools/Open Title Screen Scene")]
    private static void OpenTitleScreenScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/TitleScreenScene.unity", OpenSceneMode.Single);
    }

    [MenuItem("Tools/Open Hunting Scene")]
    private static void OpenHuntingScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/HuntingScene.unity", OpenSceneMode.Single);
    }
}

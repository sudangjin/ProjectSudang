using UnityEditor;
using UnityEditor.SceneManagement;

public static class SceneShortcut
{
    [MenuItem("Shortcuts/Open Lobby Scene %#F1")] // Ctrl+Shift+F1
    public static void OpenLobbyScene()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Lobby.unity");
        }
    }

    [MenuItem("Shortcuts/Open InGame Scene %#F2")] // Ctrl+Shift+F2
    public static void OpenInGameScene()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/Scenes/InGame.unity");
        }
    }
}

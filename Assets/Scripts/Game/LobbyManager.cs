using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public void OnStartGame()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("InGame");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "InGame")
        {
            var gameSessionManager = FindObjectOfType<GameSessionManager>();
            if (gameSessionManager != null)
            {
                gameSessionManager.Init();
            }

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}

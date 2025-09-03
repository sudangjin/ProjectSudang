using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private UIGauge loadingGauge;
    [SerializeField] private GameObject gameStartButton;

    private void Start()
    {
        gameStartButton.SetActive(false);
        loadingGauge.SetVisibility(true);

        StartCoroutine(LoadGameData());
    }

    private IEnumerator LoadGameData()
    {
        yield return DataManager.Instance.InitAsync(progress => {
            loadingGauge.UpdateValue(progress, 1f);
            loadingGauge.SetProgress(progress * 100f);
        });

        loadingGauge.SetVisibility(false);
        gameStartButton.SetActive(true);
    }

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
                gameSessionManager.Init(1);
            }

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}

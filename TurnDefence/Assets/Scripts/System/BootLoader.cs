using UnityEngine;
using UnityEngine.SceneManagement;

public class BootLoader : MonoBehaviour
{
    [SerializeField] private string startSceneName = "Lobby";

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name != startSceneName)
        {
            SceneManager.LoadScene(startSceneName);
        }
    }
}
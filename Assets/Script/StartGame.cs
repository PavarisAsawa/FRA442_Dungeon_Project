using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    public Button startButton; // Assign the button in the Inspector

    void Start()
    {
        startButton.onClick.AddListener(StartGameScene);
    }

    void StartGameScene()
    {
        SceneManager.LoadScene("DungeonScenes"); // Replace "YourSceneName" with the actual scene name
    }
}

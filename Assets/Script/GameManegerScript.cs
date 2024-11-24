using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    public GameObject gameOverCanvasPrefab; // Assign the Game Over Canvas Prefab in the Inspector

    private static GameManagerScript instance;

    void Awake()
    {
        // Singleton pattern to ensure only one GameManagerScript exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keep the GameManagerScript across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate GameManagerScripts
        }
    }

    public static GameManagerScript Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("GameManagerScript is not initialized!");
            }
            return instance;
        }
    }

    // Function to show Game Over screen
    public void ShowGameOver()
    {
        if (gameOverCanvasPrefab != null)
        {
            Instantiate(gameOverCanvasPrefab); // Spawn the Game Over Canvas

            // Make the cursor visible
            Cursor.lockState = CursorLockMode.None; // Unlock the cursor
            Cursor.visible = true; // Make the cursor visible
        }
        else
        {
            Debug.LogWarning("Game Over Canvas Prefab is not assigned in the Inspector!");
        }
    }

    // Function to restart the game
    public void RestartGame()
    {
        //Time.timeScale = 1; // Resume the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload the current scene

        // Restore cursor settings for gameplay
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center
        Cursor.visible = false; // Hide the cursor
    }

    // Function to go back to the Start Game scene
    public void GoToStartGame()
    {
        //Time.timeScale = 1; // Resume the game
        SceneManager.LoadScene("StartGame"); // Replace "StartGame" with your scene name

        // Restore cursor settings for gameplay
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Show the cursor
    }
}

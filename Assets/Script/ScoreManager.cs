using UnityEngine;
using UnityEngine.UI; // For standard UI Text (use TMPro if using TextMeshPro)
using TMPro;
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance; // Singleton instance

    

    public TextMeshProUGUI scoreText;
    private int score = 0; // Player's score

    void Awake()
    {
        // Singleton pattern to ensure a single instance of ScoreManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateScoreUI(); // Initialize the score display
    }

    // Method to increase the score
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    // Update the score UI
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score " + score;
        }
        else
        {
            Debug.LogWarning("ScoreText is not assigned!");
        }
    }
}

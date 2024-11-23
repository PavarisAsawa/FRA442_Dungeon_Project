using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveLevelUIManager : MonoBehaviour
{
    public Image waveImage; // The UI Image for the wave level
    public Sprite[] waveSprites; // Array of sprites for each wave
    public RectTransform levelRect; // RectTransform for the level image

    // Default sizes for normal and boss waves
    public Vector2 normalSize = new Vector2(30, 30);
    public Vector2 bossSize = new Vector2(100, 50);

    public void SetWave(int waveNumber)
    {
        if (waveNumber - 1 < 0 || waveNumber - 1 >= waveSprites.Length)
        {
            Debug.LogWarning("Wave number is out of bounds!");
            return;
        }

        // Update the wave image sprite
        waveImage.sprite = waveSprites[waveNumber - 1];
        waveImage.enabled = true;

        // Resize the RectTransform based on the wave
        if (waveNumber == 5) // Boss wave
        {
            levelRect.sizeDelta = bossSize; // Set size for boss wave
        }
        else
        {
            levelRect.sizeDelta = normalSize; // Set size for normal waves
        }
    }
}

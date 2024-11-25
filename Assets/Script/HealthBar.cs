using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Player player; // Reference to the Player script
    public Slider slider; // Reference to the UI slider

    void Start()
    {
        UpdateMaxHealth();
    }

    public void UpdateMaxHealth()
    {
       slider.maxValue = 100; // Keep the slider max value at 100
        slider.value = MapHealthToRange(player.playerHealth, player.playerMaxHealth); // Map current health
    }

    public void SetHealth()
    {
        slider.value = MapHealthToRange(player.playerHealth, player.playerMaxHealth); // Update current value
    }

    void Update()
    {
        SetHealth(); // Continuously sync the slider with the player's current health
    }
    private float MapHealthToRange(float currentHealth, float maxHealth)
    {
        return (currentHealth / maxHealth) * 100; // Normalize to the 0-100 range
    }
}

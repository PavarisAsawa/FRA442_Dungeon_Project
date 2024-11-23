using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Player player;
    public Slider slider;

    public void SetMaxHealt(int health)
    {
        slider.maxValue = 100;
        slider.value = player.playerHealth;
    }

    public void SetHealth()
    {
        slider.value = player.playerHealth;
    }

    void Update() {
        SetHealth();
    }
}

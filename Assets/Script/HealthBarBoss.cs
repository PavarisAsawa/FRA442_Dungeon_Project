using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBarBoss : MonoBehaviour
{
    public Slider slider;
    public DragonAi Boss;
    // Start is called before the first frame update
    void Start()
    {
        slider.maxValue = Boss.maxHealth;
        slider.value = Boss.dragonHealth;
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = Boss.dragonHealth;
        if (Boss != null)
        {
            slider.value = Boss.dragonHealth;

            // Destroy the health bar if the boss is dead
            if (Boss.dragonHealth <= 0)
            {
                Destroy(gameObject); // Destroy this health bar
            }
        }
    }

}

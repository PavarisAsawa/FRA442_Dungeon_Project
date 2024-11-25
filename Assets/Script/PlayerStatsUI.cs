using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI speedText;
    public Player player;

    void Update()
    {
        if (player != null)
        {
            healthText.text = "Health: " + player.playerHealth;
            damageText.text = "Damage: " + player.AttackDamage;
            speedText.text = "Speed: " + player.runSpeed;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public WaveLevelUIManager waveUI;
    private int currentWave = 0;

    void StartWave()
    {
        currentWave++; // Increment the wave number
        waveUI.SetWave(currentWave); // Update the wave UI
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) waveUI.SetWave(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) waveUI.SetWave(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) waveUI.SetWave(3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) waveUI.SetWave(4);
        if (Input.GetKeyDown(KeyCode.Alpha5)) waveUI.SetWave(5);
    }

}

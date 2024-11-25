using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // Singleton for global access
    public AudioClip backgroundMusic;    // Background music clip
    public AudioClip gameOverMusic;      // Game over music clip
    public AudioClip victoryMusic;        // Victory sound
    [Range(0f, 1f)] public float volume = 0.5f; // Volume for the music

    private AudioSource audioSource;

    void Awake()
    {
        // Ensure only one AudioManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
            SetupAudioSource();           // Set up the audio source and play music
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void SetupAudioSource()
    {
        // Add AudioSource component at runtime
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;          // Loop the background music
        audioSource.playOnAwake = false; // Avoid auto-play; we control it manually
        audioSource.volume = volume;
        audioSource.Play();               // Play the music
    }

    public void PlayGameOverMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();            // Stop the current music
            audioSource.clip = gameOverMusic;
            audioSource.loop = false;      // Game over sound usually doesn't loop
            audioSource.Play();            // Play the game over music
        }
    }
    public void PlayVictoryMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();            // Stop the current music
            audioSource.clip = victoryMusic;
            audioSource.loop = false;      // Victory music usually doesn't loop
            audioSource.Play();            // Play the victory sound
        }
    }
    public void AdjustVolume(float newVolume)
    {
        volume = Mathf.Clamp(newVolume, 0f, 1f);
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }
}

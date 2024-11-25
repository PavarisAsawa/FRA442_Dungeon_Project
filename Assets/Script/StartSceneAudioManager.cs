using UnityEngine;

public class StartSceneAudioManager : MonoBehaviour
{
    public AudioClip startSceneMusic; // The music to play in the Start Scene
    [Range(0f, 1f)] public float volume = 0.5f; // Volume of the music

    private AudioSource audioSource;

    void Start()
    {
        SetupAudioSource();
        PlayStartSceneMusic();
    }

    void SetupAudioSource()
    {
        // Add an AudioSource component dynamically
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = startSceneMusic;
        audioSource.loop = true;          // Loop the start scene music
        audioSource.playOnAwake = false; // Prevent auto-play before setup
        audioSource.volume = volume;
    }

    public void PlayStartSceneMusic()
    {
        if (audioSource != null && startSceneMusic != null)
        {
            audioSource.Play(); // Start playing the music
        }
    }

    public void StopStartSceneMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop(); // Stop the music
        }
    }
}

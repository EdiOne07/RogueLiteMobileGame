using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }

    private AudioSource m_AudioSource;
    private AudioSource musicSource;

    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        m_AudioSource = GetComponent<AudioSource>();
        musicSource = transform.GetChild(0).GetComponent<AudioSource>();

        LoadVolume("soundVolume", 1, 1f, m_AudioSource);
        LoadVolume("musicVolume", 0.3f, 1f, musicSource);
    }

    public void PlaySound(AudioClip _sound)
    {
        m_AudioSource.PlayOneShot(_sound);
    }

    public void ChangeSoundVolume(float change)
    {
        ChangeSourceVolume(1, "soundVolume", change, m_AudioSource);
    }

    public void ChangeMusicVolume(float change)
    {
        ChangeSourceVolume(0.3f, "musicVolume", change, musicSource);
    }

    private void ChangeSourceVolume(float baseVolume, string volumeName, float change, AudioSource audioSource)
    {
        float currentVolume = PlayerPrefs.GetFloat(volumeName, 1);
        currentVolume += change;

        // Clamp or loop the value between 0 and 1
        if (currentVolume > 1) currentVolume = 0;
        else if (currentVolume < 0) currentVolume = 1;

        float finalVolume = currentVolume * baseVolume;
        audioSource.volume = finalVolume;
        PlayerPrefs.SetFloat(volumeName, currentVolume);
    }

    private void LoadVolume(string volumeName, float baseVolume, float defaultVolume, AudioSource audioSource)
    {
        float savedVolume = PlayerPrefs.GetFloat(volumeName, defaultVolume);
        audioSource.volume = savedVolume * baseVolume;
    }
}

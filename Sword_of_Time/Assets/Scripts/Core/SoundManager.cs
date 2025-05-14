using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }
    private AudioSource m_AudioSource;
    private AudioSource musicSource;
    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
        musicSource=transform.GetChild(0).GetComponent<AudioSource>();
        instance = this;
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != null && instance != this) { 
            Destroy(gameObject);
        }
        ChangeMusicVolume(0.2f);
        ChangeSoundVolume(0.2f);
    }
    public void PlaySound(AudioClip _sound)
    {
        m_AudioSource.PlayOneShot(_sound);
    }
    public void ChangeSoundVolume(float change)
    {
        ChangeSourceVolume(1, "soundVolume", change, m_AudioSource);
    }
    public void ChangeMusicVolume(float change) {
        ChangeSourceVolume(0.3f, "musicVolume", change, musicSource);
    }
    public void ChangeSourceVolume(float baseVolume, string volumeName,float change,AudioSource audioSource)
    {
        float currentVolume = PlayerPrefs.GetFloat(volumeName,1);
        currentVolume += change;
        if (currentVolume > 1)
        {
            currentVolume = 0;
        }else if(currentVolume < 0)
        {
            currentVolume=1;
        }
        float finalVolume=currentVolume * baseVolume;
        audioSource.volume = finalVolume;
        PlayerPrefs.SetFloat (volumeName,currentVolume);
    }
}

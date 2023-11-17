using System;
using UnityEngine;

public class AudioManager :MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource m_SoundEffectAudioSource;
    private AudioSource m_BackGroundEffectAudioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            m_SoundEffectAudioSource = gameObject.AddComponent<AudioSource>();
            m_BackGroundEffectAudioSource = gameObject.AddComponent<AudioSource>();
            m_BackGroundEffectAudioSource.volume = 0.2f;
            m_BackGroundEffectAudioSource.loop = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(string soundPath)
    {
        AudioClip audioClip = Resources.Load<AudioClip>(soundPath);

        if (audioClip != null)
        {
            m_SoundEffectAudioSource.PlayOneShot(audioClip);
        }
    }
    
    public void PlayBackgroundMusic(string musicPath)
    {
        AudioClip audioClip = Resources.Load<AudioClip>(musicPath);

        if (audioClip != null)
        {
            m_BackGroundEffectAudioSource.clip = audioClip;
            m_BackGroundEffectAudioSource.Play();
        }
    }
    
    public void StopBackgroundMusic()
    {
        m_BackGroundEffectAudioSource.Stop();
    }
}
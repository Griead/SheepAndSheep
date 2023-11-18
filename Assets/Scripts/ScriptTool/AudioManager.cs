using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager :MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource m_SoundEffectAudioSource;
    private AudioSource m_BackGroundEffectAudioSource;

    private Dictionary<string, AudioClip> m_AudioClipDict;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            m_SoundEffectAudioSource = gameObject.AddComponent<AudioSource>();
            m_BackGroundEffectAudioSource = gameObject.AddComponent<AudioSource>();
            m_BackGroundEffectAudioSource.volume = 0.2f;
            m_BackGroundEffectAudioSource.loop = true;
            m_AudioClipDict = new Dictionary<string, AudioClip>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private AudioClip GetAudioClip(string soundPath)
    {
        AudioClip audioClip = null;
        if (!m_AudioClipDict.ContainsKey(soundPath))
        {
            audioClip = Resources.Load<AudioClip>(soundPath);
            m_AudioClipDict.Add(soundPath, audioClip);
        }
        else
        {
            audioClip = m_AudioClipDict[soundPath];
        }

        return audioClip;
    }

    public void PlaySound(string soundPath)
    {
        var audioClip = GetAudioClip(soundPath);

        if (audioClip != null)
        {
            m_SoundEffectAudioSource.PlayOneShot(audioClip);
        }
    }
    
    public void PlayBackgroundMusic(string musicPath)
    {
        var audioClip = GetAudioClip(musicPath);

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
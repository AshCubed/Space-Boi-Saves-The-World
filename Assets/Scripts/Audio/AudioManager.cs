using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioMixerGroup audioMixerMainGroup;
    public AudioMixerGroup audioMixersUi;
    public AudioMixerGroup audioMixersMusic;
    public AudioMixerGroup audioMixersSfx;
    [Space(10f)]
    public Sound[] sounds;

    public static AudioManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        //DontDestroyOnLoad(gameObject);

        //Creates audio sources for every sound in the list
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

             if (s.soundType == Sound.SoundType.MainTitle)
            {
                s.source.outputAudioMixerGroup = audioMixerMainGroup;
            }
            else if (s.soundType == Sound.SoundType.Ui)
            {
                s.source.outputAudioMixerGroup = audioMixersUi;
            }
            else if (s.soundType == Sound.SoundType.Music)
            {
                s.source.outputAudioMixerGroup = audioMixersMusic;
            }
            else if (s.soundType == Sound.SoundType.SFX)
            {
                s.source.outputAudioMixerGroup = audioMixersSfx;
            }
        }
    }

    public void PlaySounds(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            //Debug.Log("Sound: " + name + " not found!");
            return;
        }

        if (!s.source.isPlaying)
        {
            //Debug.Log("Sound: " + name + " Playing!");
            s.source.Play();
        }
    }

    public void ChangeVolume(string name, float amnt)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            //Debug.Log("Sound: " + name + " not found!");
            return;
        }

        if (s.source.volume != amnt)
        {
            //Debug.Log("Sound: " + name + " Volume Adujusted to " + amnt);
            s.source.volume = amnt;
        }
    }

    public void StopSounds(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            // Debug.Log("Sound: " + name + " not found!");
            return;
        }
        if (s.source.isPlaying)
        {
            s.source.Stop();
        }
    }

    public void StopAll()
    {
        foreach (Sound s in sounds)
        {
            s.source.Stop();
        }
    }

    public Sound GetSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        return s;
    }
}
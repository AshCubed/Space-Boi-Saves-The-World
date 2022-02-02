using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    public enum SoundType {MainTitle, Ui, Music, SFX };

    public SoundType soundType;

    [Range(0f, 1f)]
    public float volume = .5f;
    [Range(.1f, 3f)]
    public float pitch = 1f;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}
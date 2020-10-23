using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
/// <summary>
/// This class manages audio. It places an audio source on the main camera
/// in the scene that is used to play sound effects. Audio clips are loaded
/// from the Resources folder into the game through this class.
/// </summary>
public static class AudioManager
{
    //=====================================================================
    #region Class variables
    //=====================================================================
    /// <summary>
    /// The audio audio source to play sounds in the scene.
    /// </summary>
    private static AudioSource _audioSource;
    /// <summary>
    /// Dictionary of AudioClipNames and their related audio clip.
    /// </summary>
    private static Dictionary<AudioClipName, AudioClip> _audioClips =
        new Dictionary<AudioClipName, AudioClip>();
    #endregion

    //=====================================================================
    #region Class methods
    //=====================================================================
    /// <summary>
    /// Initializes the AudioManager. To be called once on initial load.
    /// </summary>
    public static void Init()
    {
        InitAudioSource();
        InitAudioClips();
    }

    /// <summary>
    /// Initializes the audio source on the main camera in the scene. To
    /// be called every scene change.
    /// </summary>
    public static void InitAudioSource()
    {
        AudioSource audioSource = Camera.main.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Camera.main.gameObject.AddComponent<AudioSource>();
            audioSource = Camera.main.GetComponent<AudioSource>();
        }
        _audioSource = audioSource;
    }

    /// <summary>
    /// Plays the audio clip with the given name.
    /// </summary>
    /// <param name="name">Name of the audio clip to play.</param>
    /// <param name="volumeScale">Volume at which the clip plays.</param>
    public static void Play(AudioClipName name, float volumeScale)
    {
        _audioSource.PlayOneShot(_audioClips[name], volumeScale);
    }
    #endregion

    //=====================================================================
    #region Private methods
    //=====================================================================
    /// <summary>
    /// Initializes all AudioClips into the game.
    /// </summary>
    private static void InitAudioClips()
    {
        _audioClips.Add(AudioClipName.PlayerDeath, 
            Resources.Load<AudioClip>("Audio/sampleExplode"));
        _audioClips.Add(AudioClipName.DialogueLetterSound, 
            Resources.Load<AudioClip>("Audio/sampleBlip"));
    }
    #endregion
}
}

using System; // For events
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// This class manages all audio clips. Handles loading of all audio clips from
    /// resources. Handles all audio clip retrieval.
    /// </summary>
    public class AudioClipManager
    {
        //TODO: Change this to be serializable class that allows for inputing clips in editor
        //=========================================================================
        #region Instance variables
        //=========================================================================
        private Dictionary<MusicClipName, AudioClip[]> _musicClips = new Dictionary<MusicClipName, AudioClip[]>();
        private Dictionary<EventClipName, AudioClip> _eventClips = new Dictionary<EventClipName, AudioClip>();
        private Dictionary<AmbientClipName, AudioClip[]> _ambientClips = new Dictionary<AmbientClipName, AudioClip[]>();
        private Dictionary<SFXClipName, AudioClip> _sfxClips = new Dictionary<SFXClipName, AudioClip>();
        #endregion
        //=========================================================================
        #region Constructor
        //=========================================================================
        public AudioClipManager()
        {
            Initialize();
        }
        #endregion
        //=========================================================================
        #region Audio Clip Retrieval
        //=========================================================================
        /// <summary>
        /// Retrieves the audio clips associated with a music clip name enum.
        /// </summary>
        /// <param name="clipName"> The music clip enumeration name</param>
        /// <returns>An array of audio clips associated. Index 0 = Main, Index 1 = Aux.</returns>
        public AudioClip[] GetMusicClipPair(MusicClipName clipName)
        {
            return _musicClips[clipName];
        }
        /// <summary>
        /// Retrieves the audio clip associated with a event clip name enum.
        /// </summary>
        /// <param name="clipName">The event clip enumeration name</param>
        /// <returns>The audio clip associated</returns>
        public AudioClip GetEventClip(EventClipName clipName)
        {
            return _eventClips[clipName];
        }
        /// <summary>
        /// Retrieves the audio clips associated with a ambient clip name enum.
        /// </summary>
        /// <param name="clipName">The ambient clip enumeration name</param>
        /// <returns>An array of audio clips associated. Index 0 = Main, Index 1 = Aux.</returns>
        public AudioClip[] GetAmbientClipPair(AmbientClipName clipName)
        {
            return _ambientClips[clipName];
        }
        /// <summary>
        /// Retrieves the audio clip associated with a sfx clip name enum.
        /// </summary>
        /// <param name="clipName">The sfx clip enumeration name</param>
        /// <returns>The audio clip associated</returns>
        public AudioClip GetSFXClip(SFXClipName clipName)
        {
            return _sfxClips[clipName];
        }
        #endregion
        //=========================================================================
        #region Initialization
        //=========================================================================
        public void Initialize()
        {
            InitializeMusicClips();
            InitializeEventClips();
            InitializeAmbientClips();
            InitializeSFXClips();
        }
        private void InitializeMusicClips()
        {
            //TODO: Populate
            _musicClips.Add(MusicClipName.SOS, 
                GenerateAudioClipPair(Resources.Load<AudioClip>("Audio/Debug/SOS"), Resources.Load<AudioClip>("Audio/Debug/SOS")));
            _musicClips.Add(MusicClipName.SwordShovelsCave,
                GenerateAudioClipPair(Resources.Load<AudioClip>("Audio/Temp/Music/SwordsShovels-Cave"), Resources.Load<AudioClip>("Audio/Temp/Music/SwordsShovels-Cave_Aux")));
            _musicClips.Add(MusicClipName.Scifi,
                GenerateAudioClipPair(Resources.Load<AudioClip>("Audio/Temp/Music/Track2_Main"), Resources.Load<AudioClip>("Audio/Temp/Music/Track2_Aux")));
            Debug.Log("AudioClipManager.InitializeMusicClips loaded music audio clips successfully.");
        }

        private void InitializeEventClips()
        {
            //TODO: Populate
            _eventClips.Add(EventClipName.SOS, Resources.Load<AudioClip>("Audio/Debug/SOS"));
            _eventClips.Add(EventClipName.Death, Resources.Load<AudioClip>("Audio/Temp/Music/DeathSong"));
            _eventClips.Add(EventClipName.Victory, Resources.Load<AudioClip>("Audio/Temp/Music/VictorySong"));
            Debug.Log("AudioClipManager.InitializeEventClips loaded event audio clips successfully.");
        }

        private void InitializeAmbientClips()
        {
            //TODO: Populate
            _ambientClips.Add(AmbientClipName.SOS,
                GenerateAudioClipPair(Resources.Load<AudioClip>("Audio/Debug/SOS"), Resources.Load<AudioClip>("Audio/Debug/SOS")));
            _ambientClips.Add(AmbientClipName.Rainforest,
                GenerateAudioClipPair(Resources.Load<AudioClip>("Audio/Temp/SFX/Ambiance"), Resources.Load<AudioClip>("Audio/Temp/SFX/SuperWind")));
            _ambientClips.Add(AmbientClipName.Wind,
                GenerateAudioClipPair(Resources.Load<AudioClip>("Audio/Temp/SFX/SuperWind"), null));
            Debug.Log("AudioClipManager.InitializeAmbientClips loaded ambient audio clips successfully.");
        }

        private void InitializeSFXClips()
        {
            //TODO: Populate
            _sfxClips.Add(SFXClipName.SOS, Resources.Load<AudioClip>("Audio/Debug/SOS"));
            _sfxClips.Add(SFXClipName.Bomb, Resources.Load<AudioClip>("Audio/Temp/SFX/Bomb"));
            _sfxClips.Add(SFXClipName.Splash, Resources.Load<AudioClip>("Audio/Temp/SFX/Splash001"));
            _sfxClips.Add(SFXClipName.DeathSound, Resources.Load<AudioClip>("Audio/SFX/lose-4"));
            _sfxClips.Add(SFXClipName.Typing, Resources.Load<AudioClip>("Audio/SFX/sampleBlip"));
            _sfxClips.Add(SFXClipName.GrenadeExplosion, Resources.Load<AudioClip>("Audio/SFX/sampleExplode"));
            Debug.Log("AudioClipManager.InitializeSFXClips loaded sfx audio clips successfully.");
        }
        
        private AudioClip[] GenerateAudioClipPair(AudioClip mainClip, AudioClip auxClip)
        {
            return new AudioClip[2] { mainClip, auxClip };
        }
        #endregion
    }
}